-- Ensure database exists and is selected
CREATE DATABASE IF NOT EXISTS crystal_mir2
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;
USE crystal_mir2;

-- CharacterBuffs: active buffs per character (binary payload initially)
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
