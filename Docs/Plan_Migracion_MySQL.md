# Plan de Migración a MySQL

Autor: Equipo de modernización
Fecha: Actual

## 1. Objetivo
Migrar la persistencia principal (Cuentas, Personajes, Almacén, Subastas, Correo) a MySQL manteniendo compatibilidad con el formato binario actual durante una fase transitoria (dual-write y migración automática). Se preserva la arquitectura de rendimiento actual (mutación en memoria + flush periódico con un escritor) para evitar contención.

## 2. Enfoque técnico

- **ORM recomendado**: EF Core + `Pomelo.EntityFrameworkCore.MySql` (alternativa: Dapper + `MySqlConnector`).
- **Modelo de escritura**: conservar un **único escritor** (flush por lotes) al principio; más adelante, opcionalmente pasar a write-on-action.
- **Compatibilidad**: lectura desde binario y escritura a MySQL en paralelo durante migración; posterior cutover a MySQL.
- **Seguridad**: usuario de DB con privilegios mínimos, SSL/TLS, secretos fuera del repo.

## 3. Cambios en configuración (`Server/Settings.cs`)

- Nuevos parámetros:
  - `public static string PersistenceBackend = "Binary"; // Binary | MySQL`
  - `public static bool EnableDualWrite = true;`
  - `public static bool EnableAutoMigration = true;`
  - `public static string MySqlConnectionString = "server=localhost;port=3306;database=crystalmir2;user=appuser;password=***;SslMode=Required;";`
  - Opcional: `public static int FlushIntervalSeconds = 60;`

## 4. Paquetes NuGet

- EF Core y proveedor MySQL:
  - `Microsoft.EntityFrameworkCore`
  - `Microsoft.EntityFrameworkCore.Design` (para migrations)
  - `Pomelo.EntityFrameworkCore.MySql`
- Alternativa ligera (sin EF):
  - `Dapper`
  - `MySqlConnector`

## 5. Estructura de proyecto (`Server.Persistence/`)

- `Db/GameDbContext.cs` (EF Core, MySQL)
- `Entities/` (POCOs): `AccountEntity`, `CharacterEntity`, `StorageItemEntity`, `AuctionEntity`, `MailEntity`...
- `Repositories/Interfaces/`: `IAccountRepository`, `ICharacterRepository`, `IStorageRepository`, `IAuctionRepository`, `IMailRepository`
- `Repositories/MySql/`: implementaciones EF Core
- `Repositories/Binary/`: adaptadores que usan la serialización existente
- `MigrationRunner/`: migración automática al boot

## 6. Esquema MySQL (DDL inicial)

```sql
CREATE TABLE Accounts (
  Id INT AUTO_INCREMENT PRIMARY KEY,
  AccountID VARCHAR(50) NOT NULL UNIQUE,
  PasswordHash VARCHAR(255) NOT NULL,
  Salt VARBINARY(64) NOT NULL,
  UserName VARCHAR(50) NOT NULL,
  BirthDate DATETIME NULL,
  SecretQuestion VARCHAR(100) NULL,
  SecretAnswer VARCHAR(100) NULL,
  Email VARCHAR(100) NULL,
  CreationIP VARCHAR(45) NOT NULL,
  CreationDate DATETIME NOT NULL,
  Banned TINYINT(1) NOT NULL DEFAULT 0,
  BanReason VARCHAR(200) NULL,
  ExpiryDate DATETIME NULL,
  LastIP VARCHAR(45) NULL,
  LastDate DATETIME NULL,
  WrongPasswordCount INT NOT NULL DEFAULT 0,
  RequirePasswordChange TINYINT(1) NOT NULL DEFAULT 0,
  AdminAccount TINYINT(1) NOT NULL DEFAULT 0,
  Gold INT UNSIGNED NOT NULL DEFAULT 0,
  Credit INT UNSIGNED NOT NULL DEFAULT 0,
  HasExpandedStorage TINYINT(1) NOT NULL DEFAULT 0,
  ExpandedStorageExpiryDate DATETIME NULL,
  INDEX IX_Accounts_LastDate (LastDate)
) ENGINE=InnoDB;

CREATE TABLE Characters (
  Id INT AUTO_INCREMENT PRIMARY KEY,
  AccountId INT NOT NULL,
  Name VARCHAR(50) NOT NULL,
  Class TINYINT NOT NULL,
  Gender TINYINT NOT NULL,
  Level INT NOT NULL DEFAULT 1,
  Deleted TINYINT(1) NOT NULL DEFAULT 0,
  DeleteDate DATETIME NULL,
  CreationDate DATETIME NOT NULL,
  LastLoginDate DATETIME NULL,
  LastLogoutDate DATETIME NULL,
  -- añadir más columnas de CharacterInfo según necesitemos
  CONSTRAINT FK_Characters_Accounts FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE,
  UNIQUE KEY UX_Characters_Name (Name),
  INDEX IX_Characters_AccountId (AccountId)
) ENGINE=InnoDB;

CREATE TABLE StorageItems (
  Id INT AUTO_INCREMENT PRIMARY KEY,
  AccountId INT NOT NULL,
  Slot INT NOT NULL,
  ItemData LONGBLOB NOT NULL, -- serialización binaria de UserItem inicialmente
  CONSTRAINT FK_StorageItems_Accounts FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE,
  UNIQUE KEY UX_StorageItems_Account_Slot (AccountId, Slot)
) ENGINE=InnoDB;

CREATE TABLE Auctions (
  Id INT AUTO_INCREMENT PRIMARY KEY,
  SellerAccountId INT NOT NULL,
  ItemData LONGBLOB NOT NULL,
  StartingPrice BIGINT NOT NULL,
  CurrentBid BIGINT NULL,
  CreatedAt DATETIME NOT NULL,
  ExpiresAt DATETIME NOT NULL,
  Status TINYINT NOT NULL,
  CONSTRAINT FK_Auctions_Accounts FOREIGN KEY (SellerAccountId) REFERENCES Accounts(Id)
) ENGINE=InnoDB;

CREATE TABLE Mail (
  Id INT AUTO_INCREMENT PRIMARY KEY,
  ToAccountId INT NOT NULL,
  FromAccountId INT NULL,
  Subject VARCHAR(100) NOT NULL,
  Body TEXT NOT NULL,
  SentAt DATETIME NOT NULL,
  ReadFlag TINYINT(1) NOT NULL DEFAULT 0,
  CONSTRAINT FK_Mail_ToAccount FOREIGN KEY (ToAccountId) REFERENCES Accounts(Id),
  CONSTRAINT FK_Mail_FromAccount FOREIGN KEY (FromAccountId) REFERENCES Accounts(Id)
) ENGINE=InnoDB;
```

Notas:
- `Salt VARBINARY(64)` permite 24B (legacy) o 32B (nuevo).
- `ItemData LONGBLOB` permite guardar `UserItem` tal cual al inicio. Luego podemos normalizar items si compensa.

## 7. Mapeo de entidades (ejemplo)

- `AccountInfo` -> `Accounts`
  - `AccountID`, `Password` (-> `PasswordHash` con prefijo `PBKDF2$sha256$...`), `Salt` (VARBINARY), `WrongPasswordCount`, `RequirePasswordChange`, `AdminAccount`, `Gold`, `Credit`...
- `CharacterInfo` -> `Characters` (subset inicial + extensiones graduales)
- `UserItem[] Storage` -> `StorageItems` (fila por slot, `ItemData` como blob)
- `AuctionInfo` -> `Auctions` (campos mínimos + `ItemData`)

## 8. Interfaces de repositorio (propuestas)

```csharp
public interface IAccountRepository {
  Task<AccountEntity?> GetByAccountIDAsync(string accountID, CancellationToken ct);
  Task<AccountEntity> AddAsync(AccountEntity entity, CancellationToken ct);
  Task UpdateAsync(AccountEntity entity, CancellationToken ct);
  Task<bool> ExistsAsync(string accountID, CancellationToken ct);
}

public interface ICharacterRepository {
  Task<IReadOnlyList<CharacterEntity>> GetByAccountIdAsync(int accountId, CancellationToken ct);
  Task<CharacterEntity> AddAsync(CharacterEntity entity, CancellationToken ct);
  Task UpdateAsync(CharacterEntity entity, CancellationToken ct);
}

public interface IStorageRepository {
  Task<IReadOnlyList<StorageItemEntity>> GetByAccountIdAsync(int accountId, CancellationToken ct);
  Task UpsertAsync(StorageItemEntity entity, CancellationToken ct);
}
```

## 9. Integración con el servidor (pasos)

1. **Introducir capa de repositorios** en el servidor y factorizar `Envir` para usar `IAccountRepository` en `Login/ChangePassword/HTTPLogin` (lectura), manteniendo en memoria la mutación del estado.
2. **Flush periódico**:
   - Tarea única (como hoy) que persiste cambios acumulados a MySQL usando transacciones.
   - Parámetro `FlushIntervalSeconds` configurable.
3. **Migración automática (EnableAutoMigration=true)**:
   - En el arranque, listar cuentas desde repositorio binario y hacer `Upsert` en MySQL si no existen.
   - Validar conteos y sanidad (ej: nº de personajes por cuenta) y registrar resultados.
4. **Dual-Write (EnableDualWrite=true)**:
   - Durante la fase de validación: cuando se modifiquen cuentas/personajes/storage en memoria, persistir en binario y MySQL.
5. **Cutover**:
   - Cambiar `PersistenceBackend = "MySQL"` y desactivar lectura de binario (conservar como backup).

## 10. Concurrencia y rendimiento

- **Single-writer**: conservar un único flujo de escritura por lote minimiza contención en MySQL.
- **Transacciones**: agrupar persistencia en `BEGIN; ... COMMIT;` con tamaño de lote razonable.
- **Índices**: `AccountID` único, índices por `AccountId` en `Characters` y `StorageItems`.
- **Conexiones**: usar `DbContextPool` (EF Core) o `MySqlConnector` con pool.

## 11. Seguridad

- Usuario `appuser` con permisos limitados a `crystalmir2` (no SUPER).
- Conexión con `SslMode=Required` y certificados válidos si es remoto.
- No almacenar secretos en texto plano; usar variables de entorno o secrets.

## 12. Pruebas

- **Unitarias**: repositorios (CRUD básico), mapeo `AccountInfo`/`CharacterInfo`.
- **Integración**: migración idempotente, dual-write, cutover.
- **Carga**: múltiples logins simultáneos, flush concurrente, ver latencia.

## 13. Rollback

- Mantener archivos binarios como fuente de verdad hasta validar migración.
- Si algo falla, desactivar `PersistenceBackend=MySQL` y `EnableDualWrite=false` temporalmente.

## 14. Roadmap

- Fase 1: Repositorios + Accounts/Characters/Storage en MySQL.
- Fase 2: Auctions/Mail.
- Fase 3: Evaluar Guilds y datos estáticos.
- Fase 4: Opcional write-on-action y normalización de `UserItem`.
