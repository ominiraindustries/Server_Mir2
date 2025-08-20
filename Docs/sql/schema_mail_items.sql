-- Ensure database exists and is selected
CREATE DATABASE IF NOT EXISTS crystal_mir2
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;
USE crystal_mir2;

-- MailItems: attachments for a mail message
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
