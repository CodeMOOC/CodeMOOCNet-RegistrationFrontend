CREATE TABLE `PignaNotebookRegistrations` (
  `RegistrationID` int(10) UNSIGNED NOT NULL,
  `Email` varchar(512) CHARACTER SET latin1 COLLATE latin1_general_ci NOT NULL,
  `MeccanoCode` varchar(16) CHARACTER SET latin1 COLLATE latin1_general_ci NOT NULL,
  `SchoolName` varchar(256) NOT NULL,
  `SchoolAddress` varchar(512) NOT NULL,
  `SchoolCAP` char(5) CHARACTER SET latin1 COLLATE latin1_general_ci NOT NULL,
  `SchoolCity` varchar(64) NOT NULL,
  `SchoolProvince` varchar(32) NOT NULL,
  `Phone` varchar(32) CHARACTER SET latin1 COLLATE latin1_general_ci DEFAULT NULL,
  `RegisteredOn` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COMMENT='Registrations for Pigna notebook deliveries 2020';

ALTER TABLE `PignaNotebookRegistrations`
  ADD PRIMARY KEY (`RegistrationID`);

ALTER TABLE `PignaNotebookRegistrations`
  ADD CONSTRAINT `PignaNotebookRegistrations_RegistrationIDs` FOREIGN KEY (`RegistrationID`) REFERENCES `Registrations` (`ID`);
COMMIT;

ALTER TABLE `Registrations` CHANGE `PasswordHash` `PasswordHash` CHAR(64) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL; 
