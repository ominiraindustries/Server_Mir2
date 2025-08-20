-- Ensure database exists and is selected
CREATE DATABASE IF NOT EXISTS crystal_mir2
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;
USE crystal_mir2;

-- HeroItems: equipment/inventory for heroes
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
