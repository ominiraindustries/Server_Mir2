# Informe de Persistencia y Concurrencia

Autor: Equipo de modernización
Fecha: Actual

## 1. Resumen

Este informe evalúa cómo el servidor persiste datos actualmente (binario en disco), el modelo de concurrencia efectivo, y la viabilidad de migrar a SQLite o MySQL. Concluimos que:

- El servidor usa un patrón de "single-writer" y volcado periódico: las escrituras a disco se hacen en serie y por lotes, mientras las mutaciones ocurren en memoria.
- Con este patrón, SQLite no debería provocar bloqueos significativos si mantenemos un hilo escritor único (y activamos WAL). MySQL también es viable si se prevé multi-instancia.
- Es factible migrar gradualmente entidades como Cuentas, Personajes y Almacén a SQL, manteniendo por ahora datos estáticos (Mapas, Items, NPCs) en binario.

## 2. Evidencias en el código

- `Server/MirDatabase/AccountInfo.cs`
  - Serialización binaria con longitud de sal variable:
    - Escritura: `writer.Write(Salt.Length); writer.Write(Salt);` en `AccountInfo.Save(BinaryWriter)`.
    - Lectura: `Salt = reader.ReadBytes(reader.ReadInt32());` en constructor `AccountInfo(BinaryReader)`.
  - Esto facilita migración porque no hay suposición rígida del tamaño de la sal.

- `Server/MirEnvir/Envir.cs`
  - Guardado periódico y único de cuentas:
    - `BeginSaveAccounts()`/`SaveAccounts()`: usan `Saving` para evitar escritores concurrentes y hacen swap atómico de archivo (`*.n` -> actual -> `*.o`).
    - `SaveAccounts(Stream)`: recorre listas (`AccountList`, `HeroList`, `Auctions`, etc.) y llama `Save(writer)` en serie.
  - Otras escrituras asíncronas por archivo (guilds/goods), un archivo a la vez con `BeginWrite`:
    - `SaveGuilds(bool forced = false)` -> `EndSaveGuildsAsync`
    - `SaveGoods(bool forced = false)` -> `EndSaveGoodsAsync`
  - Guardado de base de datos general: `SaveDB()` escribe recursos (MapInfo, ItemInfo, MonsterInfo, NPCInfo, QuestInfo, MagicInfo, GameShop, ConquestInfo, RespawnTick) de forma secuencial a un único archivo.
  - Concurrencia en login: `lock (AccountLock)` al vincular `account.Connection` y estado relacionado. Mutaciones como `WrongPasswordCount`, `LastDate`, `LastIP` se mantienen en memoria.

- `Server/MirNetwork/MirConnection.cs`
  - `Login(C.Login)` solo enruta a `Envir.Login(p, this)`. La lógica intensiva de estado está en `Envir`.

Conclusión técnica: el IO a disco ya es __serializado__ y por lotes; el servidor no depende de múltiples escritores concurrentes a disco.

## 3. Implicaciones para SQLite/MySQL

- Manteniendo el patrón actual (mutaciones en memoria + flush periódico mediante un escritor único), __SQLite es adecuado__:
  - Activar modo WAL (Write-Ahead Logging) y `busy_timeout` evita bloqueos en lecturas concurrentes mientras se escribe.
  - Las escrituras periódicas pueden agruparse en transacciones grandes.
- Si cambiáramos a "escritura por cada login/acción":
  - SQLite podría sufrir más contención; aún así, con transacciones cortas y WAL suele ir bien para cargas moderadas.
  - MySQL/MariaDB es preferible si esperamos alta concurrencia de escrituras y/o múltiples instancias del servidor.

## 4. Alcance de migración y prioridades

- __Alta prioridad (beneficio inmediato)__
  - __Accounts__ (cuentas de usuario): credenciales, flags de seguridad, últimos accesos.
  - __Characters__ (personajes): estado esencial y metadatos.
  - __StorageItems__ (almacén por cuenta): items por slot.
  - __Auctions__ (subastas): ya está serializado con IDs; ideal para consultas y administración.
  - __Mail__ (si existe estructura en DB binaria actual).

- __Media prioridad__
  - __Guilds__: hoy se guardan por archivo; llevar a SQL mejoraría integridad y consultas, aunque hay IO asíncrono existente.
  - __QuestProgress__ por personaje.

- __Baja prioridad (estático o voluminoso)__
  - __MapInfo, ItemInfo, MonsterInfo, NPCInfo, MagicInfo, GameShop, ConquestInfo, RespawnTick__:
    - Cargados al inicio, cambian poco en runtime. Pueden seguir en binario inicialmente.
    - Migrarlos a SQL solo si se requiere edición dinámica y consultas administrativas.

## 5. Esquema de tablas propuesto

- __Accounts__
  - `Id` (PK, int)
  - `AccountID` (nvarchar(50), UNIQUE)
  - `PasswordHash` (nvarchar(255)) — soporta prefijo `PBKDF2$sha256$...`
  - `Salt` (varbinary(64))
  - `UserName` (nvarchar(50))
  - `BirthDate` (datetime)
  - `SecretQuestion` (nvarchar(100)), `SecretAnswer` (nvarchar(100))
  - `Email` (nvarchar(100))
  - `CreationIP` (varchar(45)), `CreationDate` (datetime)
  - `Banned` (bit), `BanReason` (nvarchar(200)), `ExpiryDate` (datetime)
  - `LastIP` (varchar(45)), `LastDate` (datetime)
  - `WrongPasswordCount` (int)
  - `RequirePasswordChange` (bit)
  - `AdminAccount` (bit)
  - `Gold` (int unsigned), `Credit` (int unsigned)
  - `HasExpandedStorage` (bit), `ExpandedStorageExpiryDate` (datetime)

- __Characters__
  - `Id` (PK)
  - `AccountId` (FK -> Accounts.Id, INDEX)
  - Campos principales de `CharacterInfo` (Name, Class, Gender, Level, etc.)
  - `Deleted` (bit), `DeleteDate` (datetime)
  - `CreationDate` (datetime), `LastLoginDate` (datetime), `LastLogoutDate` (datetime)

- __StorageItems__
  - `Id` (PK)
  - `AccountId` (FK -> Accounts.Id, INDEX)
  - `Slot` (int)
  - Campos de `UserItem` (itemId, durabilidad, stats, etc.; considerar normalizar a `Items` si compensa)

- __Auctions__
  - `Id` (PK)
  - `SellerAccountId` (FK)
  - `Item` (estructura/normalización)
  - `Price`/`Bid`, fechas, estado

- __Mail__ (si aplica)
  - `Id` (PK)
  - `ToAccountId` (FK), `FromAccountId` (FK)
  - `Subject`, `Body`, `SentAt`, `Read`

- __Guilds__ (si se migra)
  - `Id` (PK), `Name` (UNIQUE), `LeaderCharacterId` (FK)
  - Miembros, rango, almacenado en tablas hijas

Notas:
- `Salt` soporta longitud 24B (legacy) o 32B (nuevo) con `varbinary(64)`.
- Índices por `AccountID`, `AccountId` en personajes, y campos de consulta frecuentes.

## 6. Cambios por archivo (refactor a repositorios)

- __Nuevo proyecto/carpeta__: `Server.Persistence/`
  - `Repositories/`
    - Interfaces: `IAccountRepository`, `ICharacterRepository`, `IStorageRepository`, `IAuctionRepository`, ...
    - Implementación binaria: `BinaryAccountRepository` (envoltura de `AccountInfo.Save/ctor`), etc.
    - Implementación SQL: `SqlAccountRepository` (EF Core o Dapper), etc.
  - `Db/`
    - EF Core: `GameDbContext` (SQLite/MySQL) + Migrations.
    - o Dapper: `DbConnectionFactory` y scripts SQL.

- __`Server/MirEnvir/Envir.cs`__
  - Reemplazar accesos directos a colecciones/lectura/escritura binaria por repositorios (al menos para Cuentas y Personajes).
  - Punto de migración: al iniciar, si `EnableAutoMigration`, leer desde repositorio binario e insertar en SQL si faltan.
  - Mantener `lock (AccountLock)` para estado en memoria. Las escrituras en SQL se harán por una tarea única de persistencia (igual que hoy con binario) o en puntos controlados.

- __`Server/MirDatabase/*.cs`__
  - Mantener clases de dominio (`*Info`).
  - Ir deprecando responsabilidades de IO binario moviéndolas a `Binary*Repository`.

- __`Server/Settings.cs`__
  - Añadir:
    - `PersistenceBackend: Binary | SQLite | MySQL`
    - `DatabasePath` (SQLite) o `ConnectionString` (MySQL)
    - `EnableDualWrite: bool`, `EnableAutoMigration: bool`
    - Opcional: `UseWAL: bool`, `BusyTimeoutMs: int` (SQLite)

## 7. Concurrencia con SQL

- __Modelo recomendado__: mantener __único escritor__ para flush periódico (igual que hoy). Esto minimiza contención con SQLite.
- __SQLite__:
  - Activar WAL: concurrencia de lecturas mientras escribe.
  - Usar transacciones por lote de guardado.
  - Configurar `busy_timeout`.
- __MySQL__:
  - Permite múltiples escritores; útil si escalas a varias instancias o si cambias a "write-on-action".

## 8. Migración: plan por fases

1) __Fase 1__: agregar capa de repositorios y `SqlAccountRepository` + esquema `Accounts`, `Characters`, `StorageItems`.
2) __Fase 2__: migración automática al boot (idempotente), con `EnableDualWrite=true` durante validación.
3) __Fase 3__: cambiar `PersistenceBackend` a SQL. Mantener binario como respaldo/lectura deshabilitada.
4) __Fase 4__: evaluar mover `Auctions`, `Mail`, `Guilds`.

## 9. Qué migrar ahora y qué dejar

- __Migrar ahora__:
  - `Accounts`, `Characters`, `StorageItems`, `Auctions`, `Mail` (si aplica).
- __Dejar temporalmente en binario__:
  - `MapInfo`, `ItemInfo`, `MonsterInfo`, `NPCInfo`, `QuestInfo`, `MagicInfo`, `GameShop`, `ConquestInfo`, `RespawnTick` (datos más estáticos; menor beneficio inmediato).

## 10. Conclusión y recomendación

- El código actual escribe en __serie__ y por lotes; no hay dependencia de IO altamente concurrente a disco. Por tanto, __SQLite es suficiente__ si mantenemos este patrón (con WAL) y simplifica despliegue.
- Si se anticipa multi-instancia o mucha escritura online, diseñar desde ya con __MySQL__.
- Recomendación: implementar capa de repositorios y empezar con __SQLite__ + migración gradual; mantener opción de cambiar a MySQL sin tocar el dominio.
