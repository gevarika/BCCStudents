-- SystemConfig ცხრილის შექმნა სისტემური კონფიგურაციების შესანახად
-- ინახავს: სწავლის დაწყების თარიღს, გადახდის თარიღს, დასვენებების თარიღებს

CREATE TABLE IF NOT EXISTS `SystemConfig` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Key` VARCHAR(100) NOT NULL UNIQUE,
    `Value` TEXT NOT NULL,
    `Type` VARCHAR(50) NOT NULL DEFAULT 'String',
    `Description` VARCHAR(255) NULL,
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    INDEX `idx_Key` (`Key`),
    INDEX `idx_Type` (`Type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- კომენტარები
-- Key-ის მაგალითები:
-- "StudyStartDate" - სწავლის დაწყების თარიღი (Value: "2024-09-01")
-- "DefaultPaymentDate" - ნაგულისხმევი გადახდის თარიღი (Value: "2024-10-01")
-- "Vacation_2024_Start" - დასვენების დაწყების თარიღი 2024 (Value: "2024-12-20")
-- "Vacation_2024_End" - დასვენების დასრულების თარიღი 2024 (Value: "2025-01-10")
-- ან შეგვიძლია JSON ფორმატში: "Vacation_2024" -> {"StartDate": "2024-12-20", "EndDate": "2025-01-10"}
