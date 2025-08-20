-- Ensure database exists and is selected
CREATE DATABASE IF NOT EXISTS crystal_mir2
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;
USE crystal_mir2;

-- Heroes: optional persistence for heroes/pets (binary blob initially)
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
