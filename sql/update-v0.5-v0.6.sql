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
  INDEX (`RegistrationID`),
  FOREIGN KEY (`RegistrationID`) REFERENCES `Registrations` (`ID`)
    ON UPDATE RESTRICT
    ON DELETE RESTRICT
)
ENGINE = InnoDB;

-- Migrate email data
INSERT INTO `Emails` (`Email`, `RegistrationID`, `IsPrimary`, `AssociationTimestamp`)
SELECT `Email`, `ID`, b'1', `RegistrationTimestamp` FROM `Registrations`;

-- Drop old email data
ALTER TABLE `Registrations` DROP COLUMN `Email`;
