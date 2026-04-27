-- ცხრილი წარუმატებელი სტუდენტების იმპორტების შესანახად
CREATE TABLE IF NOT EXISTS FailedStudentImports (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    SheetName VARCHAR(255) NULL COMMENT 'Excel sheet-ის სახელი',
    RowNumber INT NOT NULL DEFAULT 0 COMMENT 'Excel-ში მწკრივის ნომერი',
    
    -- სტუდენტის მონაცემები (რაც Excel-იდან იკითხება)
    FirstName VARCHAR(255) NULL,
    LastName VARCHAR(255) NULL,
    Age INT NULL,
    ParentName VARCHAR(255) NULL,
    PhoneNumber VARCHAR(50) NULL,
    Id_Numb BIGINT NULL,
    Address VARCHAR(500) NULL,
    TuitionFee DECIMAL(10, 2) NULL,
    Discount DOUBLE NULL,
    DateOfPayment DATETIME NULL,
    RegistrationDate DATETIME NULL,
    StudentCode VARCHAR(50) NULL,
    SubGroup INT NULL,
    GroupId INT NULL COMMENT 'ჯგუფის ID, რომელშიც უნდა დაემატებინა',
    GroupName VARCHAR(255) NULL COMMENT 'ჯგუფის სახელი (Excel sheet-ის სახელიდან)',
    
    -- შეცდომის ინფორმაცია
    ErrorMessage TEXT NOT NULL COMMENT 'შეცდომის აღწერა',
    
    -- იმპორტის მეტადატა
    ImportDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'როდის მოხდა იმპორტი',
    User_Id INT NULL COMMENT 'ვინ გააკეთა იმპორტი',
    ExcelFileName VARCHAR(500) NULL COMMENT 'Excel ფაილის სახელი',
    
    -- ინდექსები
    INDEX idx_sheet_row (SheetName, RowNumber),
    INDEX idx_import_date (ImportDate),
    INDEX idx_user_id (User_Id),
    INDEX idx_group_id (GroupId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='წარუმატებელი სტუდენტების იმპორტები';
