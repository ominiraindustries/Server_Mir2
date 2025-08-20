-- Ensure database exists and is selected
CREATE DATABASE IF NOT EXISTS crystal_mir2
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;
USE crystal_mir2;

-- CharacterVariables: quest flags / key-value variables per character
CREATE TABLE IF NOT EXISTS `CharacterVariables` (
  `CharacterId` INT NOT NULL,
  `VarKey` VARCHAR(64) NOT NULL,
  `VarValue` VARCHAR(255) NULL,
  `UpdatedAt` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`CharacterId`, `VarKey`),
  CONSTRAINT `FK_CharacterVariables_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
