-- Ensure database exists and is selected
CREATE DATABASE IF NOT EXISTS crystal_mir2
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;
USE crystal_mir2;

-- Friends: character friendships
CREATE TABLE IF NOT EXISTS `Friends` (
  `CharacterId` INT NOT NULL,
  `FriendCharacterId` INT NOT NULL,
  PRIMARY KEY (`CharacterId`, `FriendCharacterId`),
  CONSTRAINT `FK_Friends_Character` FOREIGN KEY (`CharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Friends_Friend` FOREIGN KEY (`FriendCharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Blocks: character blocks
CREATE TABLE IF NOT EXISTS `Blocks` (
  `CharacterId` INT NOT NULL,
  `BlockCharacterId` INT NOT NULL,
  PRIMARY KEY (`CharacterId`, `BlockCharacterId`),
  CONSTRAINT `FK_Blocks_Character` FOREIGN KEY (`CharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Blocks_Block` FOREIGN KEY (`BlockCharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
