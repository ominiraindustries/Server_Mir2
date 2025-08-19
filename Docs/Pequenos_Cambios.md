# Informe: Pequeños cambios a aplicar

Este documento recopila cambios pequeños pero importantes que iremos aplicando al servidor. Sirve como lista de verificación y registro de decisiones.

---

## 1) Oro por personaje (en lugar de por cuenta)

- **Estado actual**
  - El oro se guarda en `Server/MirDatabase/AccountInfo.cs` como `uint Gold;`.
  - `Server/MirDatabase/CharacterInfo.cs` no tiene campo de oro.
  - Implicación: todos los personajes de la misma cuenta comparten el oro.

- **Objetivo**
  - Que cada personaje tenga su propio saldo de oro.

- **Impacto**
  - Modelo de datos: añadir `Gold` a personajes en la base de datos y a la clase `CharacterInfo`.
  - Lógica/UI: cambiar lecturas/escrituras de oro desde `AccountInfo` a `CharacterInfo`.
  - Migración de datos: decidir cómo transformar `Accounts.Gold` a valores por personaje.

- **Opciones de migración de datos**
  - A) Transferir todo el oro de la cuenta al personaje seleccionado al primer login.
  - B) Repartir equitativamente entre todos los personajes existentes de la cuenta.
  - C) Crear un “banco” de cuenta (campo nuevo) y permitir transferencias a personajes según necesidad.

- **Propuesta inicial (simple y segura)**
  - Opción C: mantener `Accounts.Gold` como "banco de cuenta" temporal, añadir `Characters.Gold` a 0 por defecto, y añadir comandos/UI para transferir del banco al personaje. Luego, en una fase 2, retirar o limitar `Accounts.Gold`.

- **Cambios de esquema SQL (propuestos)**
  - Añadir columna `Gold` a `Characters` y (opcional) renombrar `Accounts.Gold` a `BankGold` para evitar ambigüedades.

```sql
-- Añadir oro por personaje
ALTER TABLE Characters
  ADD COLUMN Gold BIGINT UNSIGNED NOT NULL DEFAULT 0 AFTER Level;

-- (Opcional) Renombrar oro de cuenta para actuar como banco
ALTER TABLE Accounts
  CHANGE COLUMN Gold BankGold BIGINT UNSIGNED NOT NULL DEFAULT 0;
```

- **Cambios en código (alto nivel)**
  - `Server/MirDatabase/CharacterInfo.cs`: añadir campo `uint Gold` (y su serialización binaria si sigue vigente) o mapear a DB si ya migrado.
  - `Server/MirDatabase/AccountInfo.cs`: si se adopta la opción C, renombrar referencias de `Gold` a `BankGold` en el acceso a datos (o mantener compatibilidad con alias).
  - Puntos donde se modifica el oro (loot, comercio, correos, scripts NPC): cambiar a usar `Character.Gold`.

- **Plan de transición**
  1. Añadir columna `Characters.Gold` y mantener `Accounts.Gold` (o `BankGold`).
  2. Cambiar lecturas/escrituras de oro en lógica de combate/loot a `Character.Gold`.
  3. Proveer comando o UI para transferencias desde `BankGold` → `Character.Gold`.
  4. Monitorear durante un período; si estable, retirar dependencia de `BankGold`.

- **Riesgos**
  - Inconsistencias si parte del código sigue usando oro de cuenta.
  - Migraciones parciales pueden dejar saldos "perdidos" si no se controlan bien las transferencias.

- **Rollback**
  - Mantener `BankGold` permite volver temporalmente a oro por cuenta sumando los `Character.Gold` a `BankGold` y deshabilitando las rutas por personaje.

- **Referencias**
  - `Server/MirDatabase/AccountInfo.cs`
  - `Server/MirDatabase/CharacterInfo.cs`
  - `Docs/sql/schema_mysql.sql`

---

## 2) Charset y Collation consistentes

- **Descripción**: Usar `utf8mb4` en base de datos y conexión; opcional `utf8mb4_es_0900_ai_ci` para colación en MySQL 8+.
- **Impacto**: Evita problemas con tildes/ñ y emojis; ordenación en español.
- **Acciones**:
  - Confirmar en `Docs/sql/schema_mysql.sql` `DEFAULT CHARSET=utf8mb4`.
  - Si procede, establecer colación a nivel de DB o tablas.
  - En conexión (`Settings`): `CharSet=utf8mb4`.

```sql
ALTER DATABASE crystalmir2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci;
```

## 3) Tipos numéricos sin signo

- **Descripción**: Usar `UNSIGNED` para cantidades no negativas (oro, conteos, precios).
- **Impacto**: Mayor rango útil y coherencia de dominio.
- **Acciones**:
  - Revisar `schema_mysql.sql` en columnas `Gold`, `Count`, `Price` y similares.

```sql
ALTER TABLE Auctions MODIFY Price BIGINT UNSIGNED NOT NULL;
```

## 4) Índices y unicidad clave

- **Descripción**: Añadir índices y restricciones únicas para búsquedas y consistencia.
- **Impacto**: Consultas más rápidas, evita duplicados.
- **Acciones**:
  - `Accounts(Username) UNIQUE`, `Accounts(Email) UNIQUE`.
  - `Characters(Name) UNIQUE`.

```sql
ALTER TABLE Accounts ADD UNIQUE KEY UX_Accounts_Username (Username);
ALTER TABLE Accounts ADD UNIQUE KEY UX_Accounts_Email (Email);
ALTER TABLE Characters ADD UNIQUE KEY UX_Characters_Name (Name);
```

## 5) Fechas en UTC

- **Descripción**: Guardar y tratar todas las fechas en UTC.
- **Impacto**: Evita errores por zonas horarias y DST.
- **Acciones**:
  - Usar `DATETIME`/`TIMESTAMP` como UTC en DB.
  - Normalizar en código a `DateTime.UtcNow` y `DateTimeKind.Utc`.

## 6) ON DELETE/UPDATE explícitos

- **Descripción**: Definir comportamiento de FKs (CASCADE/RESTRICT/SET NULL).
- **Impacto**: Integridad referencial predecible.
- **Acciones**:
  - Revisar todas las FKs en `schema_mysql.sql` y fijar reglas adecuadas.

```sql
ALTER TABLE Characters
  DROP FOREIGN KEY FK_Characters_AccountID,
  ADD CONSTRAINT FK_Characters_AccountID FOREIGN KEY (AccountID)
    REFERENCES Accounts(AccountID) ON DELETE CASCADE ON UPDATE CASCADE;
```

## 7) Cadena de conexión centralizada y endurecida

- **Descripción**: Centralizar conexión en `Settings` con opciones seguras/útiles.
- **Impacto**: Configuración consistente y segura.
- **Acciones**:
  - `CharSet=utf8mb4;SslMode=Required;DefaultCommandTimeout=30;TreatTinyAsBoolean=false`.
  - Variables de entorno/secretos para credenciales.

## 8) SQL parametrizado siempre

- **Descripción**: Evitar concatenación; usar parámetros (Dapper/ADO.NET/EF Core).
- **Impacto**: Mitiga inyecciones SQL y errores por formato.
- **Acciones**:
  - Revisar futuras consultas y plantillas.

## 9) Logging estructurado en persistencia

- **Descripción**: Añadir logs con contexto en operaciones de guardar/cargar.
- **Impacto**: Depuración y auditoría más fáciles.
- **Acciones**:
  - Puntos clave: login, guardado de personaje, subastas, correo.

## 10) Validaciones de longitudes y NOT NULL

- **Descripción**: Definir longitudes y `NOT NULL` + defaults sensatos.
- **Impacto**: Datos más limpios y menos errores por nulos.
- **Acciones**:
  - `Username` 32-64, `Email` 254, `Name` 32, `Message` TEXT.

```sql
ALTER TABLE Accounts MODIFY Username VARCHAR(64) NOT NULL;
ALTER TABLE Accounts MODIFY Email VARCHAR(254) NOT NULL;
```

## 11) Campos de auditoría

- **Descripción**: Añadir `CreatedAt` y `UpdatedAt` (UTC) donde aplique.
- **Impacto**: Seguimiento de cambios y diagnósticos.
- **Acciones**:
  - Añadir a `Accounts`, `Characters`, `Auctions`, etc.

```sql
ALTER TABLE Accounts ADD COLUMN CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE Accounts ADD COLUMN UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;
```

## 12) Compatibilidad y bandera para drops en DB

- **Descripción**: Poder alternar entre drops desde archivos y desde DB.
- **Impacto**: Migración gradual sin romper funcionalidad.
- **Acciones**:
  - Añadir flag en `Server/Settings.cs` (ej. `UseDatabaseDrops`).
  - Ruta de carga dual en `MonsterInfo.cs`.

## 13) Codificación de ficheros externos (UTF-8)

- **Descripción**: Asegurar lectura/escritura de TXT/INI en UTF-8.
- **Impacto**: Evita caracteres corruptos en drops/NPCs.
- **Acciones**:
  - Revisar `Shared/Helpers/FileIO.cs` y `Functions/IniReader.cs` para `Encoding.UTF8`.
