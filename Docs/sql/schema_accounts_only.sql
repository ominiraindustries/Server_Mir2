-- Schema MySQL/MariaDB minimal para Crystal Mir2 (SOLO Accounts)
-- Motor y charset recomendados
CREATE DATABASE IF NOT EXISTS crystal_mir2
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;
USE crystal_mir2;

-- Asegurar SQL mode seguro
SET sql_mode = 'STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- Tabla: Accounts (solo lo necesario para migrar cuentas)
CREATE TABLE IF NOT EXISTS Accounts (
  Id INT NOT NULL AUTO_INCREMENT,
  AccountID VARCHAR(50) NOT NULL,
  PasswordHash VARCHAR(255) NOT NULL,
  Salt VARBINARY(64) NOT NULL,
  RequirePasswordChange TINYINT(1) NOT NULL DEFAULT 0,
  UserName VARCHAR(50) NOT NULL,
  BirthDate DATETIME NULL,
  SecretQuestion VARCHAR(100) NULL,
  SecretAnswer VARCHAR(100) NULL,
  Email VARCHAR(100) NULL,
  CreationIP VARCHAR(45) NOT NULL,
  CreationDate DATETIME NOT NULL,
  Banned TINYINT(1) NOT NULL DEFAULT 0,
  BanReason VARCHAR(200) NULL,
  ExpiryDate DATETIME NULL,
  LastIP VARCHAR(45) NULL,
  LastDate DATETIME NULL,
  HasExpandedStorage TINYINT(1) NOT NULL DEFAULT 0,
  ExpandedStorageExpiryDate DATETIME NULL,
  Gold INT UNSIGNED NOT NULL DEFAULT 0,
  Credit INT UNSIGNED NOT NULL DEFAULT 0,
  AdminAccount TINYINT(1) NOT NULL DEFAULT 0,
  WrongPasswordCount INT NOT NULL DEFAULT 0,
  PRIMARY KEY (Id),
  UNIQUE KEY UX_Accounts_AccountID (AccountID),
  KEY IX_Accounts_LastDate (LastDate)
) ENGINE=InnoDB ROW_FORMAT=DYNAMIC;
