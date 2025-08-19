# Mapa de Campos Binario → MySQL

Este documento mapea los campos principales de las clases binarias actuales a columnas MySQL. Se basa en:
- `Server/MirDatabase/AccountInfo.cs`
- `Server/MirDatabase/CharacterInfo.cs`
- `Shared/Data/ItemData.cs` (clases `UserItem`, `ItemInfo` y subestructuras)
- `Server/MirDatabase/AuctionInfo.cs`
- `Server/MirEnvir/MailInfo.cs`

Notas generales:
- Tiempos (`DateTime`) se guardan en MySQL como `DATETIME` (UTC recomendado a nivel de aplicación).
- `bool` → `TINYINT(1)`.
- `uint` → `INT UNSIGNED` (o `BIGINT UNSIGNED` si pudiera exceder 4,294,967,295).
- Arrays y colecciones complejas que cambian de tamaño (inventarios, items adjuntos, etc.) se modelan como tablas hijas. Para simplificar la Fase 1 podemos almacenar las estructuras `UserItem` como BLOB binario (`LONGBLOB`) con la serialización existente.

---

## Accounts (AccountInfo)
Fuente: `AccountInfo`

- Index (int) → `Id INT AUTO_INCREMENT PRIMARY KEY`
- AccountID (string) → `AccountID VARCHAR(50) NOT NULL UNIQUE`
- Password (string con prefijo PBKDF2$...) → `PasswordHash VARCHAR(255) NOT NULL`
- Salt (byte[]) → `Salt VARBINARY(64) NOT NULL`
- RequirePasswordChange (bool) → `RequirePasswordChange TINYINT(1) NOT NULL DEFAULT 0`
- UserName (string) → `UserName VARCHAR(50) NOT NULL`
- BirthDate (DateTime) → `BirthDate DATETIME NULL`
- SecretQuestion/Answer (string) → `SecretQuestion VARCHAR(100) NULL`, `SecretAnswer VARCHAR(100) NULL`
- EMailAddress (string) → `Email VARCHAR(100) NULL`
- CreationIP (string) → `CreationIP VARCHAR(45) NOT NULL`
- CreationDate (DateTime) → `CreationDate DATETIME NOT NULL`
- Banned (bool) → `Banned TINYINT(1) NOT NULL DEFAULT 0`
- BanReason (string) → `BanReason VARCHAR(200) NULL`
- ExpiryDate (DateTime) → `ExpiryDate DATETIME NULL`
- LastIP (string) → `LastIP VARCHAR(45) NULL`
- LastDate (DateTime) → `LastDate DATETIME NULL`
- HasExpandedStorage (bool) → `HasExpandedStorage TINYINT(1) NOT NULL DEFAULT 0`
- ExpandedStorageExpiryDate (DateTime) → `ExpandedStorageExpiryDate DATETIME NULL`
- Gold (uint) → `Gold INT UNSIGNED NOT NULL DEFAULT 0`
- Credit (uint) → `Credit INT UNSIGNED NOT NULL DEFAULT 0`
- AdminAccount (bool) → `AdminAccount TINYINT(1) NOT NULL DEFAULT 0`
- WrongPasswordCount (int) → `WrongPasswordCount INT NOT NULL DEFAULT 0`

Colecciones y relaciones:
- Characters → tabla `Characters` (1:N)
- Storage (UserItem[ ]) → tabla `StorageItems` (1:N) por slot
- Auctions (LinkedList<AuctionInfo>) → tabla `Auctions` (1:N) por cuenta vendedora
- Mail (en `CharacterInfo`, no a nivel de cuenta)

Índices sugeridos:
- `UNIQUE(AccountID)`
- `INDEX(LastDate)`

---

## Characters (CharacterInfo)
Fuente: `CharacterInfo`

Claves y metadatos principales:
- Index (int) → `Id INT AUTO_INCREMENT PRIMARY KEY`
- AccountInfo (FK) → `AccountId INT NOT NULL REFERENCES Accounts(Id)`
- Name (string) → `Name VARCHAR(50) NOT NULL UNIQUE`
- Level (ushort) → `Level INT NOT NULL`
- Class (enum byte) → `Class TINYINT NOT NULL`
- Gender (enum byte) → `Gender TINYINT NOT NULL`
- Hair (byte) → `Hair TINYINT NOT NULL`

Estado y control:
- CreationIP (string) → `CreationIP VARCHAR(45) NOT NULL`
- CreationDate (DateTime) → `CreationDate DATETIME NOT NULL`
- Banned (bool) → `Banned TINYINT(1) NOT NULL DEFAULT 0`
- BanReason (string) → `BanReason VARCHAR(200) NULL`
- ExpiryDate (DateTime) → `ExpiryDate DATETIME NULL`
- LastIP (string) → `LastIP VARCHAR(45) NULL`
- LastLogoutDate (DateTime) → `LastLogoutDate DATETIME NULL`
- LastLoginDate (DateTime) → `LastLoginDate DATETIME NULL`
- Deleted (bool) → `Deleted TINYINT(1) NOT NULL DEFAULT 0`
- DeleteDate (DateTime) → `DeleteDate DATETIME NULL`

Localización:
- CurrentMapIndex (int) → `CurrentMapIndex INT NOT NULL`
- CurrentLocation (Point:X,Y ints) → `CurrentX INT NOT NULL`, `CurrentY INT NOT NULL`
- Direction (byte) → `Direction TINYINT NOT NULL`
- BindMapIndex (int) → `BindMapIndex INT NOT NULL`
- BindLocation (Point) → `BindX INT NOT NULL`, `BindY INT NOT NULL`

Atributos y experiencia:
- HP, MP (int) → `HP INT NOT NULL`, `MP INT NOT NULL`
- Experience (long) → `Experience BIGINT NOT NULL`
- PKPoints (int) → `PKPoints INT NOT NULL DEFAULT 0`
- AMode, PMode (byte) → `AttackMode TINYINT NOT NULL`, `PetMode TINYINT NOT NULL`
- MentalState (byte), MentalStateLvl (byte) → `MentalState TINYINT NOT NULL`, `MentalStateLvl TINYINT NOT NULL`

Flags y permisos:
- AllowGroup/Trade/Observe (bool) → `AllowGroup TINYINT(1)`, `AllowTrade TINYINT(1)`, `AllowObserve TINYINT(1)`

Relaciones complejas (tablas hijas opcionales o BLOB en Fase 1):
- Inventory/Equipment/QuestInventory/Trade/Refine (`UserItem[]`) → Tablas hijas normalizadas por tipo + slot, o BLOB por conjunto. En Fase 1: mantener en binario actual y no migrar inventarios de personaje; priorizar `StorageItems` a nivel de cuenta.
- Magics (List<UserMagic>) → Tabla hija `CharacterMagics` (Fase posterior)
- Pets (List<PetInfo>) → Tabla hija `CharacterPets` (Fase posterior)
- Buffs (List<Buff>) → Tabla hija (Fase posterior)
- Poisons (List<Poison>) → No migrar inicialmente (runtime)
- Mail (List<MailInfo>) → Ver sección Mail
- Friends (List<FriendInfo>) → Tabla hija `CharacterFriends` (Fase posterior)
- Quests (CurrentQuests, CompletedQuests) → Tablas hijas (Fase posterior)
- Heroes (HeroInfo[]) → Fase posterior

Índices sugeridos:
- `INDEX(AccountId)`
- `UNIQUE(Name)`

---

## StorageItems (UserItem por cuenta y slot)
Fuente: `AccountInfo.Storage` y `UserItem`

- Id → `Id INT AUTO_INCREMENT PRIMARY KEY`
- AccountId → `AccountId INT NOT NULL REFERENCES Accounts(Id)`
- Slot (posición en array) → `Slot INT NOT NULL`
- ItemData (serialización binaria de `UserItem`) → `ItemData LONGBLOB NOT NULL`

Restricciones:
- `UNIQUE(AccountId, Slot)` para evitar duplicados en un slot.

Opciones de normalización futura:
- Extraer campos clave de `UserItem` (UniqueID, ItemIndex, CurrentDura, MaxDura, Count, SoulBoundId, Identified, Cursed, GemCount, Awake/AddedStats compactados, etc.) a columnas explícitas si se requieren consultas/filtrado.

---

## Auctions (AuctionInfo)
Fuente: `AuctionInfo`

- Id → `Id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY` (alternativa: usar `AuctionID` original como `BIGINT UNSIGNED` no auto-incremental)
- AuctionID (ulong) → `AuctionID BIGINT UNSIGNED NOT NULL UNIQUE`
- SellerIndex (CharacterInfo.Index) → `SellerCharacterId INT NOT NULL` (FK opcional a `Characters.Id` si se migran personajes)
- SellerAccountId (derivable por join, opcional en Fase 1)
- Item (UserItem) → `ItemData LONGBLOB NOT NULL`
- ConsignmentDate (DateTime) → `ConsignmentDate DATETIME NOT NULL`
- Price (uint) → `Price INT UNSIGNED NOT NULL`
- ItemType (byte) → `ItemType TINYINT NOT NULL` (según `MarketItemType`)
- CurrentBid (uint) → `CurrentBid INT UNSIGNED NULL`
- CurrentBuyerIndex (int) → `CurrentBuyerCharacterId INT NULL`
- Expired (bool), Sold (bool) → `Expired TINYINT(1) NOT NULL DEFAULT 0`, `Sold TINYINT(1) NOT NULL DEFAULT 0`

Índices sugeridos:
- `INDEX(SellerCharacterId)`
- `INDEX(ConsignmentDate)`

---

## Mail y MailItems (MailInfo)
Fuente: `MailInfo` (en `Server.MirEnvir`)

- MailID (ulong) → `MailID BIGINT UNSIGNED NOT NULL UNIQUE`
- Sender (string) → `Sender VARCHAR(50) NOT NULL` (nombre personaje o sistema)
- RecipientIndex (CharacterInfo.Index) → `RecipientCharacterId INT NOT NULL`
- Message (string) → `Message TEXT NOT NULL`
- Gold (uint) → `Gold INT UNSIGNED NOT NULL DEFAULT 0`
- DateSent (DateTime) → `DateSent DATETIME NOT NULL`
- DateOpened (DateTime) → `DateOpened DATETIME NULL`
- Locked (bool) → `Locked TINYINT(1) NOT NULL DEFAULT 0`
- Collected (bool) → `Collected TINYINT(1) NOT NULL DEFAULT 0`
- CanReply (bool) → `CanReply TINYINT(1) NOT NULL DEFAULT 0`

Ítems adjuntos (lista `Items: List<UserItem>`) → tabla hija `MailItems`:
- Id → `Id INT AUTO_INCREMENT PRIMARY KEY`
- MailID (FK) → `MailID BIGINT UNSIGNED NOT NULL REFERENCES Mail(MailID) ON DELETE CASCADE`
- Ordinal (posición) → `Ordinal INT NOT NULL`
- ItemData → `ItemData LONGBLOB NOT NULL`

Índices:
- `INDEX(RecipientCharacterId)` en `Mail`
- `UNIQUE(MailID, Ordinal)` en `MailItems`

---

## Consideraciones adicionales

- `Salt VARBINARY(64)` cubre 24B (legacy) y 32B (recomendado) sin cambiar esquema.
- Usar `InnoDB` y `utf8mb4` para soporte completo de Unicode.
- Tiempos: guardar en UTC a nivel de aplicación para consistencia entre servidores.
- Futuras tablas (no migrar en Fase 1): `Guilds`, `QuestProgress`, `CharacterBuffs`, `CharacterPets`, `CharacterMagics`, `Friends`, `Heroes`. Se documentarán en fases posteriores.
