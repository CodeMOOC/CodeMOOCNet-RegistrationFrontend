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
  `Name` VARCHAR(256) NOT NULL,
  `Surname` VARCHAR(256) NOT NULL,
  `Email` VARCHAR(256) NOT NULL,
  `Birthday` DATE NOT NULL,
  `Birthplace` VARCHAR(256) NOT NULL,
  `PhoneNumber` VARCHAR(256) DEFAULT NULL,
  `PasswordSchema` CHAR(10) NOT NULL,
  `PasswordHash` VARCHAR(1024) NOT NULL,
  `IsTeacher` BIT(1) DEFAULT b'0',
  `HasAttendedMooc` BIT(1) DEFAULT b'0',
  `RegistrationTimestamp` DATETIME NOT NULL,
  `ConfirmationSecret` CHAR(10) NOT NULL,
  `ConfirmationTimestamp` DATETIME DEFAULT NULL,
  PRIMARY KEY (`ID`),
  INDEX `FullName_idx` (`Surname`, `Name`),
  INDEX `Email_idx` (`Email`),
  INDEX `RegistrationTimestamp_idx` (`RegistrationTimestamp`)
)
ENGINE = InnoDB;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
