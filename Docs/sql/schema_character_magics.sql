-- Ensure database exists and is selected
CREATE DATABASE IF NOT EXISTS crystal_mir2
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;
USE crystal_mir2;

-- CharacterMagics: learned spells per character
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
