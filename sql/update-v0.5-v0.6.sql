-- Clean up data
DELETE FROM `Registrations` WHERE `ConfirmationTimestamp` IS NULL;
DELETE FROM `Registrations` WHERE `ID` = 287;

-- Create new tables
CREATE TABLE IF NOT EXISTS `CodeMoocNet`.`Emails` (
  `Email` VARCHAR(512) NOT NULL COLLATE latin1_general_ci,
  `RegistrationID` INT UNSIGNED NOT NULL REFERENCES `Registrations` (`ID`),
  `IsPrimary` BIT(1) DEFAULT b'0',
  `AssociationTimestamp` DATETIME NOT NULL,
  
  PRIMARY KEY (`Email`),
  CONSTRAINT `Registration_fk` FOREIGN KEY `Registration_idx` (`RegistrationID`) REFERENCES `Registrations` (`ID`)
    ON UPDATE RESTRICT
    ON DELETE RESTRICT
)
ENGINE = InnoDB;

-- Migrate email data
INSERT INTO `Emails` (`Email`, `RegistrationID`, `IsPrimary`, `AssociationTimestamp`) SELECT DISTINCT `Email`, `ID`, b'1', `RegistrationTimestamp` FROM `Registrations`;

-- Drop old email data
ALTER TABLE `Registrations` DROP COLUMN `Email`;

-- Fix badge table
ALTER TABLE `Badges`
  DROP PRIMARY KEY,
  DROP INDEX `Lookup_idx`,
  ADD COLUMN `Year` YEAR(4) NOT NULL DEFAULT '2019' AFTER `Type`,
  ADD PRIMARY KEY (`Email`, `Type`, `Year`),
  ADD INDEX `Lookup_idx` (`Type`, `Year`, `EvidenceToken`);
UPDATE `Badges` SET `Type` = SUBSTRING(`Type`, 1, CHAR_LENGTH(`Type`) - 4);

-- Optimize step
OPTIMIZE TABLE `Registrations`;
OPTIMIZE TABLE `Emails`;
OPTIMIZE TABLE `Donations`;
OPTIMIZE TABLE `Badges`;
