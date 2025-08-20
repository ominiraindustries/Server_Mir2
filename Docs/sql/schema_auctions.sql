-- Ensure database exists and is selected
CREATE DATABASE IF NOT EXISTS crystal_mir2
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;
USE crystal_mir2;

-- Auctions (basic schema; adapt to AuctionInfo.cs if fields differ)
CREATE TABLE IF NOT EXISTS `Auctions` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `SellerCharacterId` INT NOT NULL,
  `ItemIndex` INT NOT NULL,
  `UniqueID` BIGINT UNSIGNED NOT NULL,
  `ItemData` LONGBLOB NOT NULL,
  `StartPrice` BIGINT UNSIGNED NOT NULL,
  `BuyoutPrice` BIGINT UNSIGNED NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ExpiresAt` DATETIME NOT NULL,
  `BuyerCharacterId` INT NULL,
  `FinalPrice` BIGINT UNSIGNED NULL,
  `Status` TINYINT NOT NULL DEFAULT 0, -- 0 Active, 1 Sold, 2 Cancelled, 3 Expired
  PRIMARY KEY (`Id`),
  KEY `IX_Auctions_Seller` (`SellerCharacterId`),
  KEY `IX_Auctions_Buyer` (`BuyerCharacterId`),
  CONSTRAINT `FK_Auctions_Seller` FOREIGN KEY (`SellerCharacterId`) REFERENCES `Characters`(`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Auctions_Buyer` FOREIGN KEY (`BuyerCharacterId`) REFERENCES `Characters`(`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
