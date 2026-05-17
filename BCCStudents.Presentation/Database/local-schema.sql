-- MySQL dump 10.13  Distrib 9.3.0, for Win64 (x86_64)
--
-- Host: localhost    Database: bccstudents_local
-- ------------------------------------------------------
-- Server version	9.3.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `failedpayments`
--

DROP TABLE IF EXISTS `failedpayments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `failedpayments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RowNumber` int NOT NULL,
  `PaymentDate` datetime NOT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `PersonalId` bigint DEFAULT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `Reason` varchar(255) NOT NULL,
  `CreatedAt` datetime NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `failedstudentimports`
--

DROP TABLE IF EXISTS `failedstudentimports`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `failedstudentimports` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `SheetName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Excel sheet-ის სახელი',
  `RowNumber` int NOT NULL DEFAULT '0' COMMENT 'Excel-ში მწკრივის ნომერი',
  `FirstName` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LastName` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Age` int DEFAULT NULL,
  `ParentName` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `PhoneNumber` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Id_Numb` bigint DEFAULT NULL,
  `Address` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TuitionFee` decimal(10,2) DEFAULT NULL,
  `Discount` double DEFAULT NULL,
  `DateOfPayment` datetime DEFAULT NULL,
  `RegistrationDate` datetime DEFAULT NULL,
  `StudentCode` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `SubGroup` int DEFAULT NULL,
  `GroupId` int DEFAULT NULL COMMENT 'ჯგუფის ID, რომელშიც უნდა დაემატებინა',
  `GroupName` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'ჯგუფის სახელი (Excel sheet-ის სახელიდან)',
  `ErrorMessage` text COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'შეცდომის აღწერა',
  `ImportDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'როდის მოხდა იმპორტი',
  `User_Id` int DEFAULT NULL COMMENT 'ვინ გააკეთა იმპორტი',
  `ExcelFileName` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Excel ფაილის სახელი',
  PRIMARY KEY (`Id`),
  KEY `idx_sheet_row` (`SheetName`,`RowNumber`),
  KEY `idx_import_date` (`ImportDate`),
  KEY `idx_user_id` (`User_Id`),
  KEY `idx_group_id` (`GroupId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='წარუმატებელი სტუდენტების იმპორტები';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `groups`
--

DROP TABLE IF EXISTS `groups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `groups` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) NOT NULL,
  `Price` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Teacher` varchar(255) DEFAULT NULL,
  `ContractTemplatePath` varchar(500) DEFAULT NULL,
  `Status` tinyint(1) NOT NULL DEFAULT '1',
  `StudentCount` int NOT NULL DEFAULT '0',
  `MaxStudents` int DEFAULT NULL COMMENT 'ჯგუფში დასამატებელი მოსწავლეების მაქსიმალური რაოდენობა',
  `UpdatedAt` datetime DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `ux_groups_name` (`Name`),
  KEY `idx_groups_updated` (`UpdatedAt`,`Id`),
  KEY `idx_groups_deleted` (`IsDeleted`,`UpdatedAt`,`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `importedfileslog`
--

DROP TABLE IF EXISTS `importedfileslog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `importedfileslog` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FilePath` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'ფაილის სრული გზა',
  `FileName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'ფაილის სახელი',
  `FileHash` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'ფაილის MD5 hash დუპლიკატების თავიდან აცილებისთვის',
  `FileSize` bigint NOT NULL COMMENT 'ფაილის ზომა ბაიტებში',
  `ImportedAt` datetime NOT NULL COMMENT 'იმპორტის თარიღი',
  `ImportedBy` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'ვინ გააკეთა იმპორტი',
  `ComputerName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'რომელი კომპიუტერიდან გაკეთდა იმპორტი',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP COMMENT 'ჩანაწერის შექმნის თარიღი',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_FileHash` (`FileHash`),
  KEY `IDX_ComputerName` (`ComputerName`),
  KEY `IDX_ImportedAt` (`ImportedAt`),
  KEY `IDX_ImportedBy` (`ImportedBy`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='იმპორტირებული ფაილების ტრეკინგი ავტომატური აღმოჩენისთვის';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `importedpaymentslog`
--

DROP TABLE IF EXISTS `importedpaymentslog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `importedpaymentslog` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PaymentDate` datetime NOT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `PersonalId` bigint DEFAULT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `ImportSource` varchar(100) DEFAULT NULL,
  `CreatedAt` datetime NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `payments`
--

DROP TABLE IF EXISTS `payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `StudentId` int DEFAULT NULL,
  `GroupId` int NOT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `PaymentDate` datetime NOT NULL,
  `PaymentStatus` varchar(50) DEFAULT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `PayerName` varchar(255) DEFAULT NULL,
  `PersonalId` bigint DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `idx_payments_updated` (`UpdatedAt`,`Id`),
  KEY `idx_payments_deleted` (`IsDeleted`,`UpdatedAt`,`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pendingstudentgroups`
--

DROP TABLE IF EXISTS `pendingstudentgroups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pendingstudentgroups` (
  `Id` int NOT NULL,
  `StudentId` int NOT NULL,
  `GroupId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `idx_pending_student_groups_student_id` (`StudentId`),
  KEY `idx_pending_student_groups_group_id` (`GroupId`),
  CONSTRAINT `fk_pending_student_groups_group` FOREIGN KEY (`GroupId`) REFERENCES `groups` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_pending_student_groups_student` FOREIGN KEY (`StudentId`) REFERENCES `pendingstudents` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pendingstudents`
--

DROP TABLE IF EXISTS `pendingstudents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pendingstudents` (
  `Id` int NOT NULL,
  `FirstName` varchar(255) DEFAULT NULL,
  `LastName` varchar(255) DEFAULT NULL,
  `Age` int DEFAULT NULL,
  `ParentName` varchar(255) DEFAULT NULL,
  `PhoneNumber` varchar(50) DEFAULT NULL,
  `Id_Numb` bigint DEFAULT NULL,
  `Address` varchar(500) DEFAULT NULL,
  `TuitionFee` decimal(10,2) DEFAULT NULL,
  `DiscountPercentage` decimal(10,2) DEFAULT NULL,
  `StudentCode` varchar(25) DEFAULT NULL,
  `IdCardPath` varchar(500) DEFAULT NULL,
  `AdditionalDocsPath` varchar(500) DEFAULT NULL,
  `user_id` int DEFAULT NULL,
  `CreatedAt` datetime DEFAULT NULL,
  `Info` varchar(255) DEFAULT NULL COMMENT 'დამატებითი ინფორმაცია',
  `SocialStatus` varchar(50) DEFAULT NULL COMMENT 'მოსწავლის სტატუსი',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pendingstudentsubgroups`
--

DROP TABLE IF EXISTS `pendingstudentsubgroups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pendingstudentsubgroups` (
  `Id` int NOT NULL,
  `StudentId` int NOT NULL,
  `GroupId` int NOT NULL,
  `SubGroupId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `idx_pending_student_subgroups_student_id` (`StudentId`),
  KEY `idx_pending_student_subgroups_group_id` (`GroupId`),
  KEY `idx_pending_student_subgroups_subgroup_id` (`SubGroupId`),
  CONSTRAINT `fk_pending_student_subgroups_group` FOREIGN KEY (`GroupId`) REFERENCES `groups` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_pending_student_subgroups_student` FOREIGN KEY (`StudentId`) REFERENCES `pendingstudents` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_pending_student_subgroups_subgroup` FOREIGN KEY (`SubGroupId`) REFERENCES `subgroups` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `studentgroups`
--

DROP TABLE IF EXISTS `studentgroups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `studentgroups` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `StudentId` int NOT NULL,
  `GroupId` int NOT NULL,
  `PaymentStatus` varchar(50) DEFAULT NULL,
  `DateOfPayment` datetime DEFAULT NULL,
  `Price` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Discount` double NOT NULL DEFAULT '0',
  `Status` tinyint(1) NOT NULL DEFAULT '1',
  `UpdatedAt` datetime DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `fk_student_group_student` (`StudentId`),
  KEY `fk_student_group_group` (`GroupId`),
  KEY `idx_studentgroups_updated` (`UpdatedAt`,`Id`),
  KEY `idx_studentgroups_deleted` (`IsDeleted`,`UpdatedAt`,`Id`),
  CONSTRAINT `fk_student_group_group` FOREIGN KEY (`GroupId`) REFERENCES `groups` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_student_group_student` FOREIGN KEY (`StudentId`) REFERENCES `students` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `students`
--

DROP TABLE IF EXISTS `students`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `students` (
  `Id` int NOT NULL AUTO_INCREMENT,
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
  `SocialStatus` varchar(50) DEFAULT NULL COMMENT 'მოსწავლის სოციალური სტატუსი',
  `IdCardPath` text,
  `AdditionalDocsPath` text,
  `user_id` bigint unsigned NOT NULL,
  `Balance` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Info` text,
  `UpdatedAt` datetime DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `ux_students_code` (`StudentCode`),
  KEY `idx_students_updated` (`UpdatedAt`,`Id`),
  KEY `idx_students_deleted` (`IsDeleted`,`UpdatedAt`,`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `studentsubgroups`
--

DROP TABLE IF EXISTS `studentsubgroups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `studentsubgroups` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `StudentId` int NOT NULL,
  `GroupId` int NOT NULL,
  `SubGroupId` int NOT NULL,
  `PaymentStatus` varchar(50) DEFAULT NULL,
  `DateOfPayment` datetime DEFAULT NULL,
  `Price` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Discount` double NOT NULL DEFAULT '0',
  `Status` tinyint(1) NOT NULL DEFAULT '1',
  `UpdatedAt` datetime DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `fk_ssg_student` (`StudentId`),
  KEY `fk_ssg_group` (`GroupId`),
  KEY `fk_ssg_subgroup` (`SubGroupId`),
  KEY `idx_studentsubgroups_updated` (`UpdatedAt`,`Id`),
  KEY `idx_studentsubgroups_deleted` (`IsDeleted`,`UpdatedAt`,`Id`),
  CONSTRAINT `fk_ssg_group` FOREIGN KEY (`GroupId`) REFERENCES `groups` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_ssg_student` FOREIGN KEY (`StudentId`) REFERENCES `students` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_ssg_subgroup` FOREIGN KEY (`SubGroupId`) REFERENCES `subgroups` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `subgroups`
--

DROP TABLE IF EXISTS `subgroups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `subgroups` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `GroupId` int NOT NULL,
  `Name` varchar(255) NOT NULL,
  `TuitionFee` decimal(10,2) NOT NULL DEFAULT '0.00',
  `StudentCount` int NOT NULL DEFAULT '0',
  `MaxStudents` int DEFAULT NULL COMMENT 'მოსწავლეების დასაშვები მაქსიმალური რაოდენობა',
  `Status` tinyint(1) NOT NULL DEFAULT '1',
  `UpdatedAt` datetime DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `fk_subgroups_group` (`GroupId`),
  KEY `idx_subgroups_updated` (`UpdatedAt`,`Id`),
  KEY `idx_subgroups_deleted` (`IsDeleted`,`UpdatedAt`,`Id`),
  CONSTRAINT `fk_subgroups_group` FOREIGN KEY (`GroupId`) REFERENCES `groups` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `syncmetadata`
--

DROP TABLE IF EXISTS `syncmetadata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `syncmetadata` (
  `Id` tinyint NOT NULL DEFAULT '1',
  `LastUpstreamSuccessAt` datetime DEFAULT NULL,
  `LastUpstreamFailedAt` datetime DEFAULT NULL,
  `LastUpstreamBackfillAt` datetime DEFAULT NULL,
  `PendingCount` int NOT NULL DEFAULT '0',
  `FailedCount` int NOT NULL DEFAULT '0',
  `LastError` text,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `syncoutbox`
--

DROP TABLE IF EXISTS `syncoutbox`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `syncoutbox` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `TableName` varchar(64) NOT NULL,
  `RecordId` int NOT NULL,
  `RecordKey` varchar(128) DEFAULT NULL,
  `Operation` varchar(16) NOT NULL,
  `PayloadJson` mediumtext,
  `OccurredAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Attempts` int NOT NULL DEFAULT '0',
  `LastError` text,
  `LastRetryTime` datetime DEFAULT NULL,
  `Status` tinyint NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `idx_outbox_status` (`Status`,`OccurredAt`),
  KEY `idx_outbox_table` (`TableName`,`OccurredAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `syncstate`
--

DROP TABLE IF EXISTS `syncstate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `syncstate` (
  `TableName` varchar(64) NOT NULL,
  `LastSyncedAt` datetime DEFAULT NULL,
  `LastSyncedId` int DEFAULT NULL,
  PRIMARY KEY (`TableName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `systemconfig`
--

DROP TABLE IF EXISTS `systemconfig`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `systemconfig` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Key` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Value` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `Type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'String',
  `Description` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Key` (`Key`),
  KEY `idx_Key` (`Key`),
  KEY `idx_Type` (`Type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(255) NOT NULL,
  `FullName` varchar(255) DEFAULT NULL,
  `Email` varchar(255) DEFAULT NULL,
  `Password` varchar(255) DEFAULT NULL,
  `Role` varchar(50) DEFAULT NULL,
  `CreatedAt` datetime DEFAULT NULL,
  `LastLogin` datetime DEFAULT NULL,
  `Permissions` text COMMENT 'JSON: {"CanImport": true, "CanDelete": false, ...}',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping routines for database 'bccstudents_local'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-05-08  0:39:50
