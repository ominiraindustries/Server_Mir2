-- Schema MySQL/MariaDB para Crystal Mir2 (SOLO Characters - core)
-- Requiere que la base y Accounts ya existan (ver schema_accounts_only.sql)
USE crystal_mir2;

-- Asegurar SQL mode seguro
SET sql_mode = 'STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- Tabla: Characters (estado principal del personaje)
CREATE TABLE IF NOT EXISTS Characters (
  Id INT NOT NULL AUTO_INCREMENT,
  AccountId INT NOT NULL,

  Name VARCHAR(20) NOT NULL,
  Level SMALLINT UNSIGNED NOT NULL DEFAULT 1,
  Class TINYINT UNSIGNED NOT NULL,
  Gender TINYINT UNSIGNED NOT NULL,
  Hair TINYINT UNSIGNED NOT NULL DEFAULT 0,
  GuildIndex INT NOT NULL DEFAULT -1,

  CreationIP VARCHAR(45) NOT NULL,
  CreationDate DATETIME NOT NULL,

  Banned TINYINT(1) NOT NULL DEFAULT 0,
  BanReason VARCHAR(200) NULL,
  ExpiryDate DATETIME NULL,

  ChatBanned TINYINT(1) NOT NULL DEFAULT 0,
  ChatBanExpiryDate DATETIME NULL,

  LastIP VARCHAR(45) NULL,
  LastLogoutDate DATETIME NULL,
  LastLoginDate DATETIME NULL,

  Deleted TINYINT(1) NOT NULL DEFAULT 0,
  DeleteDate DATETIME NULL,

  -- Marriage / Mentor
  Married INT NOT NULL DEFAULT 0,
  MarriedDate DATETIME NULL,
  Mentor INT NOT NULL DEFAULT 0,
  MentorDate DATETIME NULL,
  IsMentor TINYINT(1) NOT NULL DEFAULT 0,
  MentorExp BIGINT NOT NULL DEFAULT 0,

  -- Location
  CurrentMapIndex INT NOT NULL DEFAULT 0,
  CurrentLocationX INT NOT NULL DEFAULT 0,
  CurrentLocationY INT NOT NULL DEFAULT 0,
  Direction TINYINT UNSIGNED NOT NULL DEFAULT 0,
  BindMapIndex INT NOT NULL DEFAULT 0,
  BindLocationX INT NOT NULL DEFAULT 0,
  BindLocationY INT NOT NULL DEFAULT 0,

  -- Stats
  HP INT NOT NULL DEFAULT 0,
  MP INT NOT NULL DEFAULT 0,
  Experience BIGINT NOT NULL DEFAULT 0,
  Gold INT UNSIGNED NOT NULL DEFAULT 0,

  -- Modes / Flags
  AMode TINYINT UNSIGNED NOT NULL DEFAULT 0,
  PMode TINYINT UNSIGNED NOT NULL DEFAULT 0,
  AllowGroup TINYINT(1) NOT NULL DEFAULT 1,
  AllowTrade TINYINT(1) NOT NULL DEFAULT 1,
  AllowObserve TINYINT(1) NOT NULL DEFAULT 1,
  PKPoints INT NOT NULL DEFAULT 0,
  NewDay TINYINT(1) NOT NULL DEFAULT 0,
  Thrusting TINYINT(1) NOT NULL DEFAULT 0,
  HalfMoon TINYINT(1) NOT NULL DEFAULT 0,
  CrossHalfMoon TINYINT(1) NOT NULL DEFAULT 0,
  DoubleSlash TINYINT(1) NOT NULL DEFAULT 0,
  MentalState TINYINT UNSIGNED NOT NULL DEFAULT 0,
  MentalStateLvl TINYINT UNSIGNED NOT NULL DEFAULT 0,

  -- Misc counters
  PearlCount INT NOT NULL DEFAULT 0,
  CollectTime BIGINT NOT NULL DEFAULT 0,

  PRIMARY KEY (Id),
  UNIQUE KEY UX_Characters_Name (Name),
  KEY IX_Characters_Account (AccountId),
  KEY IX_Characters_LastLogin (LastLoginDate),
  CONSTRAINT FK_Characters_Accounts FOREIGN KEY (AccountId)
    REFERENCES Accounts (Id)
    ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE=InnoDB ROW_FORMAT=DYNAMIC;

-- NOTA: Estructuras complejas (Inventario, Equipo, Magias, Buffs, Mail, Amigos, Mascotas, Quests)
-- deberán ir en tablas separadas. Este esquema cubre el núcleo del CharacterInfo.
