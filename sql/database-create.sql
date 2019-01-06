-- MySQL Script handcrafted

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

-- -----------------------------------------------------
-- Schema CodyMaze
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `CodeMoocNet` DEFAULT CHARACTER SET utf8;
USE `CodeMoocNet`;

-- -----------------------------------------------------
-- Table `CodyMaze`.`Moves`
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
  `AddressCap` CHAR(6) NOT NULL COLLATE latin1_general_ci,
  `AddressCountry` VARCHAR(64) NOT NULL,
  `Email` VARCHAR(512) NOT NULL COLLATE latin1_general_ci,
  `PasswordSchema` CHAR(10) NOT NULL,
  `PasswordHash` BINARY(70) NOT NULL,
  `Category` VARCHAR(16) NOT NULL,
  `HasAttendedMooc` BIT(1) DEFAULT b'0',
  `HasCompletedMooc` BIT(1) DEFAULT b'0',
  `RegistrationTimestamp` DATETIME NOT NULL,
  `ConfirmationSecret` CHAR(10) NOT NULL,
  `ConfirmationTimestamp` DATETIME DEFAULT NULL,
  PRIMARY KEY (`ID`),
  INDEX `FullName_idx` (`Surname`, `Name`),
  INDEX `Email_idx` (`Email`),
  INDEX `Address_idx` (`AddressCountry`, `AddressCity`),
  INDEX `RegistrationTimestamp_idx` (`RegistrationTimestamp`)
)
ENGINE = InnoDB;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
