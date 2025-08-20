-- Ensure database exists and is selected
CREATE DATABASE IF NOT EXISTS crystal_mir2
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;
USE crystal_mir2;

-- Mail: headers and content (attachments in MailItems)
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
