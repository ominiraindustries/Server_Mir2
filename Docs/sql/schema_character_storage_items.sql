-- Ensure database exists and is selected
CREATE DATABASE IF NOT EXISTS crystal_mir2
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;
USE crystal_mir2;

-- CharacterStorageItems: personal storage/bank items
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
