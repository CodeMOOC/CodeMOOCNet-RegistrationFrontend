-- MySQL Script handcrafted

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

-- -----------------------------------------------------
-- Schema CodeMoocNet
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `CodeMoocNet` DEFAULT CHARACTER SET utf8;
USE `CodeMoocNet`;

-- -----------------------------------------------------
-- Table `CodeMoocNet`.`Registrations`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `CodeMoocNet`.`Registrations` (
  `ID` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(128) NOT NULL,
  `Surname` VARCHAR(128) NOT NULL,
  `Birthday` DATE NOT NULL,
  `Birthplace` VARCHAR(128) NOT NULL,
  `FiscalCode` CHAR(16) NOT NULL COLLATE latin1_general_ci,
  `AddressStreet` VARCHAR(128) NOT NULL,
  `AddressCity` VARCHAR(64) NOT NULL,
  `AddressCap` CHAR(5) NOT NULL COLLATE latin1_general_ci,
  `AddressCountry` VARCHAR(64) NOT NULL,
  `Email` VARCHAR(512) NOT NULL COLLATE latin1_general_ci,
  `PasswordSchema` CHAR(10) NOT NULL,
  `PasswordHash` VARBINARY(128) NOT NULL,
  `Category` VARCHAR(16) NOT NULL,
  `HasAttendedMooc` BIT(1) DEFAULT b'0',
  `HasCompletedMooc` BIT(1) DEFAULT b'0',
  `RegistrationTimestamp` DATETIME NOT NULL,
  `ConfirmationSecret` CHAR(10) NOT NULL,
  `ConfirmationTimestamp` DATETIME DEFAULT NULL,
  
  PRIMARY KEY (`ID`),
  INDEX `FullName_idx` (`Surname`, `Name`),
  INDEX `Address_idx` (`AddressCountry`, `AddressCity`),
  INDEX `FiscalCode_idx` (`FiscalCode`),
  INDEX `Email_idx` (`Email`),
  INDEX `RegistrationTimestamp_idx` (`RegistrationTimestamp`)
)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `CodeMoocNet`.`Registrations`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `CodeMoocNet`.`Donations` (
  `Name` VARCHAR(128) NOT NULL,
  `Surname` VARCHAR(128) NOT NULL,
  `Email` VARCHAR(512) NOT NULL COLLATE latin1_general_ci,
  `Year` YEAR(4) NOT NULL,
  `Amount` SMALLINT UNSIGNED NOT NULL,

  PRIMARY KEY (`Email`, `Year`),
  INDEX `FullName_idx` (`Surname`, `Name`)
)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `CodeMoocNet`.`Badges`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `CodeMoocNet`.`Badges` (
  `Email` VARCHAR(512) NOT NULL COLLATE latin1_general_ci,
  `Type` VARCHAR(32) NOT NULL COLLATE latin1_general_ci,
  `IssueTimestamp` DATETIME NOT NULL,
  `EvidenceToken` VARCHAR(64) NOT NULL COLLATE latin1_general_ci,

  PRIMARY KEY (`Email`, `Type`),
  INDEX `Issue_idx` (`IssueTimestamp`),
  INDEX `Lookup_idx` (`Type`, `EvidenceToken`)
)
ENGINE = InnoDB;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
