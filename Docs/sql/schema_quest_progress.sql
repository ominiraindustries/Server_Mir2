-- Ensure database exists and is selected
CREATE DATABASE IF NOT EXISTS crystal_mir2
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;
USE crystal_mir2;

-- QuestProgress: progress/state per quest (binary payload initially)
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
