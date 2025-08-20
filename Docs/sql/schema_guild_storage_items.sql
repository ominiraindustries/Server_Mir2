-- Ensure database exists and is selected
CREATE DATABASE IF NOT EXISTS crystal_mir2
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;
USE crystal_mir2;

-- GuildStorageItems: guild shared storage items
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
