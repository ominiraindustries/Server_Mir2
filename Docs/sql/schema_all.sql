-- Crystal Mir2 - Full schema (MariaDB/MySQL)
-- Creates database and all tables in the correct dependency order.
-- Charset: utf8mb4

-- 1) Database
CREATE DATABASE IF NOT EXISTS crystal_mir2
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;
USE crystal_mir2;

-- 2) Recommended SQL mode
SET sql_mode = 'STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- 3) Accounts (root principal)
CREATE TABLE IF NOT EXISTS Accounts (
  Id INT NOT NULL AUTO_INCREMENT,
  AccountID VARCHAR(50) NOT NULL,
  PasswordHash VARCHAR(255) NOT NULL,
  Salt VARBINARY(64) NOT NULL,
  RequirePasswordChange TINYINT(1) NOT NULL DEFAULT 0,
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
  HasExpandedStorage TINYINT(1) NOT NULL DEFAULT 0,
  ExpandedStorageExpiryDate DATETIME NULL,
  Gold INT UNSIGNED NOT NULL DEFAULT 0,
  Credit INT UNSIGNED NOT NULL DEFAULT 0,
  AdminAccount TINYINT(1) NOT NULL DEFAULT 0,
  WrongPasswordCount INT NOT NULL DEFAULT 0,
  PRIMARY KEY (Id),
  UNIQUE KEY UX_Accounts_AccountID (AccountID),
  KEY IX_Accounts_LastDate (LastDate)
) ENGINE=InnoDB ROW_FORMAT=DYNAMIC;

-- 4) Characters (depends on Accounts)
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

  Married INT NOT NULL DEFAULT 0,
  MarriedDate DATETIME NULL,
  Mentor INT NOT NULL DEFAULT 0,
  MentorDate DATETIME NULL,
  IsMentor TINYINT(1) NOT NULL DEFAULT 0,
  MentorExp BIGINT NOT NULL DEFAULT 0,

  CurrentMapIndex INT NOT NULL DEFAULT 0,
  CurrentLocationX INT NOT NULL DEFAULT 0,
  CurrentLocationY INT NOT NULL DEFAULT 0,
  Direction TINYINT UNSIGNED NOT NULL DEFAULT 0,
  BindMapIndex INT NOT NULL DEFAULT 0,
  BindLocationX INT NOT NULL DEFAULT 0,
  BindLocationY INT NOT NULL DEFAULT 0,

  HP INT NOT NULL DEFAULT 0,
  MP INT NOT NULL DEFAULT 0,
  Experience BIGINT NOT NULL DEFAULT 0,
  Gold INT UNSIGNED NOT NULL DEFAULT 0,

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

-- 5) Character satellite tables (depend on Characters)

-- 5.1 CharacterItems
CREATE TABLE IF NOT EXISTS `CharacterItems` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `CharacterId` INT NOT NULL,
  `Container` TINYINT NOT NULL,
  `Slot` INT NOT NULL,
  `ItemIndex` INT NOT NULL,
  `UniqueID` BIGINT UNSIGNED NOT NULL,
  `ItemData` LONGBLOB NOT NULL,
  `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `IX_CharacterItems_Character` (`CharacterId`),
  KEY `IX_CharacterItems_Lookup` (`CharacterId`, `Container`, `Slot`),
  CONSTRAINT `FK_CharacterItems_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 5.2 CharacterMagics
CREATE TABLE IF NOT EXISTS `CharacterMagics` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `CharacterId` INT NOT NULL,
  `MagicId` INT NOT NULL,
  `Level` INT NOT NULL DEFAULT 0,
  `Experience` BIGINT NOT NULL DEFAULT 0,
  `Key` TINYINT NULL,
  `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_CharacterMagics` (`CharacterId`, `MagicId`),
  KEY `IX_CharacterMagics_Character` (`CharacterId`),
  CONSTRAINT `FK_CharacterMagics_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 5.3 CharacterBuffs
CREATE TABLE IF NOT EXISTS `CharacterBuffs` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `CharacterId` INT NOT NULL,
  `BuffData` LONGBLOB NOT NULL,
  `ExpiresAt` DATETIME NULL,
  `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `IX_CharacterBuffs_Character` (`CharacterId`),
  CONSTRAINT `FK_CharacterBuffs_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 5.4 CharacterVariables
CREATE TABLE IF NOT EXISTS `CharacterVariables` (
  `CharacterId` INT NOT NULL,
  `VarKey` VARCHAR(64) NOT NULL,
  `VarValue` VARCHAR(255) NULL,
  `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`CharacterId`, `VarKey`),
  CONSTRAINT `FK_CharacterVariables_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 5.5 CharacterStorageItems
CREATE TABLE IF NOT EXISTS `CharacterStorageItems` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `CharacterId` INT NOT NULL,
  `Slot` INT NOT NULL,
  `ItemIndex` INT NOT NULL,
  `UniqueID` BIGINT UNSIGNED NOT NULL,
  `ItemData` LONGBLOB NOT NULL,
  `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_CharStorage_Slot` (`CharacterId`, `Slot`),
  KEY `IX_CharacterStorage_Character` (`CharacterId`),
  CONSTRAINT `FK_CharacterStorageItems_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 5.6 QuestProgress
CREATE TABLE IF NOT EXISTS `QuestProgress` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `CharacterId` INT NOT NULL,
  `QuestId` INT NOT NULL,
  `State` TINYINT NOT NULL,
  `ProgressData` LONGBLOB NULL,
  `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_QuestProgress` (`CharacterId`, `QuestId`),
  KEY `IX_QuestProgress_Character` (`CharacterId`),
  CONSTRAINT `FK_QuestProgress_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 6) Mail
CREATE TABLE IF NOT EXISTS `Mail` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `CharacterId` INT NOT NULL,
  `Sender` VARCHAR(24) NOT NULL,
  `Subject` VARCHAR(64) NOT NULL,
  `Body` TEXT NULL,
  `Gold` INT UNSIGNED NOT NULL DEFAULT 0,
  `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ReadAt` DATETIME NULL,
  `ClaimedAt` DATETIME NULL,
  `DeletedAt` DATETIME NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Mail_Character` (`CharacterId`),
  CONSTRAINT `FK_Mail_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `MailItems` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `MailId` BIGINT NOT NULL,
  `Slot` INT NOT NULL,
  `ItemIndex` INT NOT NULL,
  `UniqueID` BIGINT UNSIGNED NOT NULL,
  `ItemData` LONGBLOB NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_MailItems_Slot` (`MailId`, `Slot`),
  KEY `IX_MailItems_Mail` (`MailId`),
  CONSTRAINT `FK_MailItems_Mail` FOREIGN KEY (`MailId`) REFERENCES `Mail`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 7) Friends / Blocks (self-FKs to Characters)
CREATE TABLE IF NOT EXISTS `Friends` (
  `CharacterId` INT NOT NULL,
  `FriendCharacterId` INT NOT NULL,
  PRIMARY KEY (`CharacterId`, `FriendCharacterId`),
  CONSTRAINT `FK_Friends_Character` FOREIGN KEY (`CharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Friends_Friend` FOREIGN KEY (`FriendCharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `Blocks` (
  `CharacterId` INT NOT NULL,
  `BlockCharacterId` INT NOT NULL,
  PRIMARY KEY (`CharacterId`, `BlockCharacterId`),
  CONSTRAINT `FK_Blocks_Character` FOREIGN KEY (`CharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Blocks_Block` FOREIGN KEY (`BlockCharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 8) Auctions (FKs to Characters)
CREATE TABLE IF NOT EXISTS `Auctions` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `SellerCharacterId` INT NOT NULL,
  `ItemIndex` INT NOT NULL,
  `UniqueID` BIGINT UNSIGNED NOT NULL,
  `ItemData` LONGBLOB NOT NULL,
  `StartPrice` BIGINT UNSIGNED NOT NULL,
  `BuyoutPrice` BIGINT UNSIGNED NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ExpiresAt` DATETIME NOT NULL,
  `BuyerCharacterId` INT NULL,
  `FinalPrice` BIGINT UNSIGNED NULL,
  `Status` TINYINT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  KEY `IX_Auctions_Seller` (`SellerCharacterId`),
  KEY `IX_Auctions_Buyer` (`BuyerCharacterId`),
  CONSTRAINT `FK_Auctions_Seller` FOREIGN KEY (`SellerCharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Auctions_Buyer` FOREIGN KEY (`BuyerCharacterId`) REFERENCES `Characters`(`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 9) Heroes and HeroItems (optional feature)
CREATE TABLE IF NOT EXISTS `Heroes` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `CharacterId` INT NOT NULL,
  `Name` VARCHAR(24) NOT NULL,
  `Level` INT NOT NULL DEFAULT 1,
  `Class` TINYINT NOT NULL,
  `HeroData` LONGBLOB NOT NULL,
  `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `IX_Heroes_Character` (`CharacterId`),
  CONSTRAINT `FK_Heroes_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `HeroItems` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `HeroId` BIGINT NOT NULL,
  `Container` TINYINT NOT NULL,
  `Slot` INT NOT NULL,
  `ItemIndex` INT NOT NULL,
  `UniqueID` BIGINT UNSIGNED NOT NULL,
  `ItemData` LONGBLOB NOT NULL,
  `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `IX_HeroItems_Hero` (`HeroId`),
  KEY `IX_HeroItems_Lookup` (`HeroId`, `Container`, `Slot`),
  CONSTRAINT `FK_HeroItems_Heroes` FOREIGN KEY (`HeroId`) REFERENCES `Heroes`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 10) Guilds and GuildStorageItems
-- Base minimal schema for Guilds to support storage items and common metadata
CREATE TABLE IF NOT EXISTS `Guilds` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(32) NOT NULL,
  `LeaderCharacterId` INT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Gold` BIGINT UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_Guilds_Name` (`Name`),
  KEY `IX_Guilds_Leader` (`LeaderCharacterId`),
  CONSTRAINT `FK_Guilds_Leader` FOREIGN KEY (`LeaderCharacterId`) REFERENCES `Characters`(`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `GuildStorageItems` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `GuildId` INT NOT NULL,
  `Slot` INT NOT NULL,
  `ItemIndex` INT NOT NULL,
  `UniqueID` BIGINT UNSIGNED NOT NULL,
  `ItemData` LONGBLOB NOT NULL,
  `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_GuildStorage_Slot` (`GuildId`, `Slot`),
  KEY `IX_GuildStorage_Guild` (`GuildId`),
  CONSTRAINT `FK_GuildStorageItems_Guilds` FOREIGN KEY (`GuildId`) REFERENCES `Guilds`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
