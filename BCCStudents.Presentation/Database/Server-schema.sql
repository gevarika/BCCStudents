-- phpMyAdmin SQL Dump
-- version 5.2.2
-- https://www.phpmyadmin.net/
--
-- Host: localhost:3306
-- Generation Time: May 13, 2026 at 11:39 PM
-- Server version: 8.0.45-cll-lve
-- PHP Version: 8.4.20

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `bccenter_SchoolManagement`
--

-- --------------------------------------------------------

--
-- Table structure for table `FailedPayments`
--

DROP TABLE IF EXISTS `FailedPayments`;
CREATE TABLE `FailedPayments` (
  `Id` int NOT NULL,
  `RowNumber` int NOT NULL,
  `PaymentDate` datetime NOT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `PersonalId` bigint DEFAULT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `Reason` varchar(255) NOT NULL,
  `CreatedAt` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `FailedStudentImports`
--

DROP TABLE IF EXISTS `FailedStudentImports`;
CREATE TABLE `FailedStudentImports` (
  `Id` int NOT NULL,
  `SheetName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Excel sheet-ის სახელი',
  `RowNumber` int NOT NULL DEFAULT '0' COMMENT 'Excel-ში მწკრივის ნომერი',
  `FirstName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LastName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Age` int DEFAULT NULL,
  `ParentName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `PhoneNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Id_Numb` bigint DEFAULT NULL,
  `Address` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TuitionFee` decimal(10,2) DEFAULT NULL,
  `Discount` double DEFAULT NULL,
  `DateOfPayment` datetime DEFAULT NULL,
  `RegistrationDate` datetime DEFAULT NULL,
  `StudentCode` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SubGroup` int DEFAULT NULL,
  `GroupId` int DEFAULT NULL COMMENT 'ჯგუფის ID, რომელშიც უნდა დაემატებინა',
  `GroupName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'ჯგუფის სახელი (Excel sheet-ის სახელიდან)',
  `ErrorMessage` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'შეცდომის აღწერა',
  `ImportDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'როდის მოხდა იმპორტი',
  `User_Id` int DEFAULT NULL COMMENT 'ვინ გააკეთა იმპორტი',
  `ExcelFileName` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Excel ფაილის სახელი'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='წარუმატებელი სტუდენტების იმპორტები';

-- --------------------------------------------------------

--
-- Table structure for table `Groups`
--

DROP TABLE IF EXISTS `Groups`;
CREATE TABLE `Groups` (
  `Id` int NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Price` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Teacher` varchar(255) DEFAULT NULL,
  `ContractTemplatePath` varchar(500) DEFAULT NULL,
  `Status` tinyint(1) NOT NULL DEFAULT '1',
  `StudentCount` int NOT NULL DEFAULT '0',
  `MaxStudents` int DEFAULT NULL COMMENT 'ჯგუფში დაშვებული მოსწავლეების მაქსიმალური რაოდენობა',
  `UpdatedAt` datetime DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `ImportedFilesLog`
--

DROP TABLE IF EXISTS `ImportedFilesLog`;
CREATE TABLE `ImportedFilesLog` (
  `Id` int NOT NULL,
  `FilePath` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'ფაილის სრული გზა',
  `FileName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'ფაილის სახელი',
  `FileHash` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'ფაილის MD5 hash დუპლიკატების თავიდან აცილებისთვის',
  `FileSize` bigint NOT NULL COMMENT 'ფაილის ზომა ბაიტებში',
  `ImportedAt` datetime NOT NULL COMMENT 'იმპორტის თარიღი',
  `ImportedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'ვინ გააკეთა იმპორტი',
  `ComputerName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'რომელი კომპიუტერიდან გაკეთდა იმპორტი',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP COMMENT 'ჩანაწერის შექმნის თარიღი'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='იმპორტირებული ფაილების ტრეკინგი ავტომატური აღმოჩენისთვის';

-- --------------------------------------------------------

--
-- Table structure for table `ImportedPaymentsLog`
--

DROP TABLE IF EXISTS `ImportedPaymentsLog`;
CREATE TABLE `ImportedPaymentsLog` (
  `Id` int NOT NULL,
  `PaymentDate` datetime NOT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `PersonalId` bigint DEFAULT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `ImportSource` varchar(100) DEFAULT NULL,
  `CreatedAt` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `ParentUsers`
--

DROP TABLE IF EXISTS `ParentUsers`;
CREATE TABLE `ParentUsers` (
  `Id` int NOT NULL,
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Password` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `FullName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `PhoneNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `CreatedAt` datetime NOT NULL,
  `LastLogin` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `Payments`
--

DROP TABLE IF EXISTS `Payments`;
CREATE TABLE `Payments` (
  `Id` int NOT NULL,
  `StudentId` int DEFAULT NULL,
  `GroupId` int NOT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `PaymentDate` datetime NOT NULL,
  `PaymentStatus` varchar(50) DEFAULT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `PayerName` varchar(255) DEFAULT NULL,
  `PersonalId` bigint DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `PendingStudentGroups`
--

DROP TABLE IF EXISTS `PendingStudentGroups`;
CREATE TABLE `PendingStudentGroups` (
  `Id` int NOT NULL,
  `StudentId` int NOT NULL,
  `GroupId` int NOT NULL,
  `CreatedAt` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `PendingStudents`
--

DROP TABLE IF EXISTS `PendingStudents`;
CREATE TABLE `PendingStudents` (
  `Id` int NOT NULL,
  `FirstName` varchar(255) DEFAULT NULL,
  `LastName` varchar(255) DEFAULT NULL,
  `Age` int DEFAULT NULL,
  `ParentName` varchar(255) DEFAULT NULL,
  `PhoneNumber` varchar(50) DEFAULT NULL,
  `Id_Numb` bigint DEFAULT NULL,
  `Address` varchar(500) DEFAULT NULL,
  `StudentCode` varchar(25) DEFAULT NULL,
  `IdCardPath` varchar(500) DEFAULT NULL,
  `AdditionalDocsPath` varchar(500) DEFAULT NULL,
  `user_id` int DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `StudentStatus` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CreatedAt` datetime DEFAULT NULL,
  `Info` varchar(255) DEFAULT NULL,
  `SocialStatus` varchar(50) DEFAULT NULL,
  `registration_uuid` varchar(64) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `PendingStudentSubGroups`
--

DROP TABLE IF EXISTS `PendingStudentSubGroups`;
CREATE TABLE `PendingStudentSubGroups` (
  `Id` int NOT NULL,
  `StudentId` int NOT NULL,
  `GroupId` int NOT NULL,
  `SubGroupId` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `StudentGroups`
--

DROP TABLE IF EXISTS `StudentGroups`;
CREATE TABLE `StudentGroups` (
  `Id` int NOT NULL,
  `StudentId` int NOT NULL,
  `GroupId` int NOT NULL,
  `PaymentStatus` varchar(50) DEFAULT NULL,
  `DateOfPayment` datetime DEFAULT NULL,
  `Price` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Discount` double NOT NULL DEFAULT '0',
  `Status` tinyint(1) NOT NULL DEFAULT '1',
  `UpdatedAt` datetime DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `Students`
--

DROP TABLE IF EXISTS `Students`;
CREATE TABLE `Students` (
  `Id` int NOT NULL,
  `FirstName` text NOT NULL,
  `LastName` text NOT NULL,
  `Age` int NOT NULL,
  `ParentName` text NOT NULL,
  `PhoneNumber` text NOT NULL,
  `Id_Numb` bigint NOT NULL,
  `Address` text NOT NULL,
  `RegistrationDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `StudentCode` varchar(25) DEFAULT NULL,
  `Status` tinyint(1) DEFAULT NULL,
  `IdCardPath` text,
  `AdditionalDocsPath` text,
  `user_id` bigint UNSIGNED NOT NULL,
  `Balance` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Info` text,
  `UpdatedAt` datetime DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `SocialStatus` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `StudentSubGroups`
--

DROP TABLE IF EXISTS `StudentSubGroups`;
CREATE TABLE `StudentSubGroups` (
  `Id` int NOT NULL,
  `StudentId` int NOT NULL,
  `GroupId` int NOT NULL,
  `SubGroupId` int NOT NULL,
  `PaymentStatus` varchar(50) DEFAULT NULL,
  `DateOfPayment` datetime DEFAULT NULL,
  `Price` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Discount` double NOT NULL DEFAULT '0',
  `Status` tinyint(1) NOT NULL DEFAULT '1',
  `UpdatedAt` datetime DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `SubGroups`
--

DROP TABLE IF EXISTS `SubGroups`;
CREATE TABLE `SubGroups` (
  `Id` int NOT NULL,
  `GroupId` int NOT NULL,
  `Name` varchar(255) NOT NULL,
  `TuitionFee` decimal(10,2) NOT NULL DEFAULT '0.00',
  `StudentCount` int NOT NULL DEFAULT '0',
  `MaxStudents` int DEFAULT '0' COMMENT 'მოსწავლეების დასაშვები რაოდენობა ქვეჯგუფში',
  `Status` tinyint(1) NOT NULL DEFAULT '1',
  `UpdatedAt` datetime DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `SystemConfig`
--

DROP TABLE IF EXISTS `SystemConfig`;
CREATE TABLE `SystemConfig` (
  `Id` int NOT NULL,
  `Key` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'String',
  `Description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `ApplicationLogs`
--

DROP TABLE IF EXISTS `ApplicationLogs`;
CREATE TABLE `ApplicationLogs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `LogGuid` char(36) NOT NULL,
  `SourceType` varchar(32) NOT NULL,
  `Category` varchar(64) DEFAULT NULL,
  `Level` varchar(16) NOT NULL,
  `Operation` varchar(128) DEFAULT NULL,
  `Status` varchar(32) DEFAULT NULL,
  `UserId` int DEFAULT NULL,
  `Username` varchar(128) DEFAULT NULL,
  `MachineName` varchar(128) DEFAULT NULL,
  `PermissionScope` varchar(64) DEFAULT NULL,
  `Message` text,
  `Details` text,
  `Exception` text,
  `SourceContext` varchar(128) DEFAULT NULL,
  `CreatedAt` datetime(3) NOT NULL,
  `SyncedToServerAt` datetime(3) DEFAULT NULL,
  `Origin` varchar(16) NOT NULL DEFAULT 'Local',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `uk_log_guid` (`LogGuid`),
  KEY `idx_created` (`CreatedAt`),
  KEY `idx_user_created` (`UserId`,`CreatedAt`),
  KEY `idx_category` (`Category`,`CreatedAt`),
  KEY `idx_unsynced` (`SyncedToServerAt`,`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `Users`
--

DROP TABLE IF EXISTS `Users`;
CREATE TABLE `Users` (
  `Id` int NOT NULL,
  `Username` varchar(255) NOT NULL,
  `FullName` varchar(255) DEFAULT NULL,
  `Email` varchar(255) DEFAULT NULL,
  `Password` varchar(255) DEFAULT NULL,
  `Role` varchar(50) DEFAULT NULL,
  `CreatedAt` datetime DEFAULT NULL,
  `LastLogin` datetime DEFAULT NULL,
  `UpdatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Permissions` text COMMENT 'JSON: {"CanImport": true, "CanDelete": false, ...}'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `FailedPayments`
--
ALTER TABLE `FailedPayments`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `FailedStudentImports`
--
ALTER TABLE `FailedStudentImports`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `idx_sheet_row` (`SheetName`,`RowNumber`),
  ADD KEY `idx_import_date` (`ImportDate`),
  ADD KEY `idx_user_id` (`User_Id`),
  ADD KEY `idx_group_id` (`GroupId`);

--
-- Indexes for table `Groups`
--
ALTER TABLE `Groups`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `ux_groups_name` (`Name`),
  ADD KEY `idx_groups_updated` (`UpdatedAt`,`Id`),
  ADD KEY `idx_groups_deleted` (`IsDeleted`,`UpdatedAt`,`Id`);

--
-- Indexes for table `ImportedFilesLog`
--
ALTER TABLE `ImportedFilesLog`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `UK_FileHash` (`FileHash`),
  ADD KEY `IDX_ComputerName` (`ComputerName`),
  ADD KEY `IDX_ImportedAt` (`ImportedAt`),
  ADD KEY `IDX_ImportedBy` (`ImportedBy`);

--
-- Indexes for table `ImportedPaymentsLog`
--
ALTER TABLE `ImportedPaymentsLog`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `ParentUsers`
--
ALTER TABLE `ParentUsers`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Email` (`Email`),
  ADD KEY `idx_email` (`Email`);

--
-- Indexes for table `Payments`
--
ALTER TABLE `Payments`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `idx_payments_updated` (`UpdatedAt`,`Id`),
  ADD KEY `idx_payments_deleted` (`IsDeleted`,`UpdatedAt`,`Id`);

--
-- Indexes for table `PendingStudentGroups`
--
ALTER TABLE `PendingStudentGroups`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `idx_pending_student_groups_student_id` (`StudentId`),
  ADD KEY `idx_pending_student_groups_group_id` (`GroupId`);

--
-- Indexes for table `PendingStudents`
--
ALTER TABLE `PendingStudents`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `idx_pending_registration_uuid` (`registration_uuid`);

--
-- Indexes for table `PendingStudentSubGroups`
--
ALTER TABLE `PendingStudentSubGroups`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `idx_pending_student_subgroups_student_id` (`StudentId`),
  ADD KEY `idx_pending_student_subgroups_group_id` (`GroupId`),
  ADD KEY `idx_pending_student_subgroups_subgroup_id` (`SubGroupId`);

--
-- Indexes for table `StudentGroups`
--
ALTER TABLE `StudentGroups`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_student_group_student` (`StudentId`),
  ADD KEY `fk_student_group_group` (`GroupId`),
  ADD KEY `idx_studentgroups_updated` (`UpdatedAt`,`Id`),
  ADD KEY `idx_studentgroups_deleted` (`IsDeleted`,`UpdatedAt`,`Id`);

--
-- Indexes for table `Students`
--
ALTER TABLE `Students`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `ux_students_code` (`StudentCode`),
  ADD KEY `idx_students_updated` (`UpdatedAt`,`Id`),
  ADD KEY `idx_students_deleted` (`IsDeleted`,`UpdatedAt`,`Id`);

--
-- Indexes for table `StudentSubGroups`
--
ALTER TABLE `StudentSubGroups`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_ssg_student` (`StudentId`),
  ADD KEY `fk_ssg_group` (`GroupId`),
  ADD KEY `fk_ssg_subgroup` (`SubGroupId`),
  ADD KEY `idx_studentsubgroups_updated` (`UpdatedAt`,`Id`),
  ADD KEY `idx_studentsubgroups_deleted` (`IsDeleted`,`UpdatedAt`,`Id`);

--
-- Indexes for table `SubGroups`
--
ALTER TABLE `SubGroups`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_subgroups_group` (`GroupId`),
  ADD KEY `idx_subgroups_updated` (`UpdatedAt`,`Id`),
  ADD KEY `idx_subgroups_deleted` (`IsDeleted`,`UpdatedAt`,`Id`);

--
-- Indexes for table `SystemConfig`
--
ALTER TABLE `SystemConfig`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Key` (`Key`),
  ADD KEY `idx_Key` (`Key`),
  ADD KEY `idx_Type` (`Type`);

--
-- Indexes for table `Users`
--
ALTER TABLE `Users`
  ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `FailedPayments`
--
ALTER TABLE `FailedPayments`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `FailedStudentImports`
--
ALTER TABLE `FailedStudentImports`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `Groups`
--
ALTER TABLE `Groups`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `ImportedFilesLog`
--
ALTER TABLE `ImportedFilesLog`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `ImportedPaymentsLog`
--
ALTER TABLE `ImportedPaymentsLog`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `ParentUsers`
--
ALTER TABLE `ParentUsers`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `Payments`
--
ALTER TABLE `Payments`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `PendingStudentGroups`
--
ALTER TABLE `PendingStudentGroups`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `PendingStudents`
--
ALTER TABLE `PendingStudents`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `PendingStudentSubGroups`
--
ALTER TABLE `PendingStudentSubGroups`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `StudentGroups`
--
ALTER TABLE `StudentGroups`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `Students`
--
ALTER TABLE `Students`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `StudentSubGroups`
--
ALTER TABLE `StudentSubGroups`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `SubGroups`
--
ALTER TABLE `SubGroups`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `SystemConfig`
--
ALTER TABLE `SystemConfig`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `PendingStudentGroups`
--
ALTER TABLE `PendingStudentGroups`
  ADD CONSTRAINT `fk_pending_student_groups_group` FOREIGN KEY (`GroupId`) REFERENCES `Groups` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_pending_student_groups_student` FOREIGN KEY (`StudentId`) REFERENCES `PendingStudents` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `PendingStudentSubGroups`
--
ALTER TABLE `PendingStudentSubGroups`
  ADD CONSTRAINT `fk_pending_student_subgroups_group` FOREIGN KEY (`GroupId`) REFERENCES `Groups` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_pending_student_subgroups_student` FOREIGN KEY (`StudentId`) REFERENCES `PendingStudents` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_pending_student_subgroups_subgroup` FOREIGN KEY (`SubGroupId`) REFERENCES `SubGroups` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
