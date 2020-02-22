-- phpMyAdmin SQL Dump
-- version 4.9.1
-- https://www.phpmyadmin.net/
--
-- Host: db
-- Generation Time: Feb 21, 2020 at 09:22 PM
-- Server version: 5.7.28
-- PHP Version: 7.2.22

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";

--
-- Database: `CodeMoocNet`
--

-- --------------------------------------------------------

--
-- Table structure for table `Badges`
--

CREATE TABLE `Badges` (
  `Email` varchar(512) CHARACTER SET latin1 COLLATE latin1_general_ci NOT NULL,
  `Type` varchar(32) CHARACTER SET latin1 COLLATE latin1_general_ci NOT NULL,
  `Year` year(4) NOT NULL DEFAULT '2019',
  `IssueTimestamp` datetime NOT NULL,
  `EvidenceToken` varchar(64) CHARACTER SET latin1 COLLATE latin1_general_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `Donations`
--

CREATE TABLE `Donations` (
  `Name` varchar(128) NOT NULL,
  `Surname` varchar(128) NOT NULL,
  `Email` varchar(512) CHARACTER SET latin1 COLLATE latin1_general_ci NOT NULL,
  `Year` year(4) NOT NULL,
  `Amount` smallint(5) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `Emails`
--

CREATE TABLE `Emails` (
  `Email` varchar(512) CHARACTER SET latin1 COLLATE latin1_general_ci NOT NULL,
  `RegistrationID` int(10) UNSIGNED NOT NULL,
  `IsPrimary` bit(1) DEFAULT b'0',
  `AssociationTimestamp` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `PignaNotebookRegistrations`
--

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

-- --------------------------------------------------------

--
-- Table structure for table `Registrations`
--

CREATE TABLE `Registrations` (
  `ID` int(10) UNSIGNED NOT NULL,
  `Name` varchar(128) NOT NULL,
  `Surname` varchar(128) NOT NULL,
  `Birthday` date NOT NULL,
  `Birthplace` varchar(128) NOT NULL,
  `FiscalCode` char(16) CHARACTER SET latin1 COLLATE latin1_general_ci NOT NULL,
  `AddressStreet` varchar(128) NOT NULL,
  `AddressCity` varchar(64) NOT NULL,
  `AddressCap` char(5) CHARACTER SET latin1 COLLATE latin1_general_ci NOT NULL,
  `AddressCountry` varchar(64) NOT NULL,
  `PasswordSchema` char(10) NOT NULL,
  `PasswordHash` char(64) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `Category` varchar(16) NOT NULL,
  `HasAttendedMooc` bit(1) DEFAULT b'0',
  `HasCompletedMooc` bit(1) DEFAULT b'0',
  `RegistrationTimestamp` datetime NOT NULL,
  `ConfirmationSecret` char(10) NOT NULL,
  `ConfirmationTimestamp` datetime DEFAULT NULL,
  `PasswordResetSecret` char(10) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `Badges`
--
ALTER TABLE `Badges`
  ADD PRIMARY KEY (`Email`,`Type`,`Year`),
  ADD KEY `Issue_idx` (`IssueTimestamp`),
  ADD KEY `Lookup_idx` (`Type`,`Year`,`EvidenceToken`);

--
-- Indexes for table `Donations`
--
ALTER TABLE `Donations`
  ADD PRIMARY KEY (`Email`,`Year`),
  ADD KEY `FullName_idx` (`Surname`,`Name`);

--
-- Indexes for table `Emails`
--
ALTER TABLE `Emails`
  ADD PRIMARY KEY (`Email`),
  ADD KEY `Registration_fk` (`RegistrationID`);

--
-- Indexes for table `PignaNotebookRegistrations`
--
ALTER TABLE `PignaNotebookRegistrations`
  ADD PRIMARY KEY (`RegistrationID`);

--
-- Indexes for table `Registrations`
--
ALTER TABLE `Registrations`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `FullName_idx` (`Surname`,`Name`),
  ADD KEY `Address_idx` (`AddressCountry`,`AddressCity`),
  ADD KEY `FiscalCode_idx` (`FiscalCode`),
  ADD KEY `RegistrationTimestamp_idx` (`RegistrationTimestamp`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `Registrations`
--
ALTER TABLE `Registrations`
  MODIFY `ID` int(10) UNSIGNED NOT NULL AUTO_INCREMENT;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `Emails`
--
ALTER TABLE `Emails`
  ADD CONSTRAINT `Registration_fk` FOREIGN KEY (`RegistrationID`) REFERENCES `Registrations` (`ID`);

--
-- Constraints for table `PignaNotebookRegistrations`
--
ALTER TABLE `PignaNotebookRegistrations`
  ADD CONSTRAINT `PignaNotebookRegistration_RegistrationIDs` FOREIGN KEY (`RegistrationID`) REFERENCES `Registrations` (`ID`);
COMMIT;
