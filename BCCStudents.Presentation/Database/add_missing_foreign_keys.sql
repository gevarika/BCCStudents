-- =============================================================================
-- Missing FOREIGN KEY links (StudentGroups / StudentSubGroups / SubGroups)
-- =============================================================================
-- Run against your School Management database (e.g. bccenter_SchoolManagement
-- on server, bccstudents_local locally).
--
-- Before ALTER: fix any rows returned by the orphan checks (or delete them).
-- If a constraint already exists, MySQL will error — safe to ignore that line.
--
-- Table name casing: this file uses PascalCase (`Students`, `Groups`, ...).
-- If your instance uses lowercase tables (`students`, `groups`), replace names
-- in ALTER TABLE / REFERENCES accordingly.
-- =============================================================================

-- USE `bccenter_SchoolManagement`;

SET NAMES utf8mb4;

/* ---- 1) Orphan checks (must return 0 rows before adding FKs) ---- */

-- StudentGroups.StudentId must exist in Students
SELECT sg.*
FROM StudentGroups sg
LEFT JOIN Students s ON s.Id = sg.StudentId
WHERE s.Id IS NULL;

-- StudentGroups.GroupId must exist in Groups
SELECT sg.*
FROM StudentGroups sg
LEFT JOIN `Groups` g ON g.Id = sg.GroupId
WHERE g.Id IS NULL;

-- StudentSubGroups
SELECT x.*
FROM StudentSubGroups x
LEFT JOIN Students s ON s.Id = x.StudentId
WHERE s.Id IS NULL;

SELECT x.*
FROM StudentSubGroups x
LEFT JOIN `Groups` g ON g.Id = x.GroupId
WHERE g.Id IS NULL;

SELECT x.*
FROM StudentSubGroups x
LEFT JOIN SubGroups sg ON sg.Id = x.SubGroupId
WHERE sg.Id IS NULL;

-- SubGroups.GroupId -> Groups
SELECT sg.*
FROM SubGroups sg
LEFT JOIN `Groups` g ON g.Id = sg.GroupId
WHERE g.Id IS NULL;

/*
 * Indexes: InnoDB უნდა ჰქონდეს FK სვეტ(ებ)ზე ინდექსი. უმეტერ სერვერს უკვე აქვს
 * (მაგ. fk_student_group_student). თუ ADD CONSTRAINT შეგიშლით „missing index“-ით:
 * დაამატე იგივე სახელის KEY რაც local-schema.sql-ში გაქვს, შემდეგ გაიმეორე ALTER.
 */


/* ---- 2) Foreign keys (matches typical local-schema.sql definitions) ---- */

ALTER TABLE StudentGroups
  ADD CONSTRAINT `fk_student_group_student`
    FOREIGN KEY (`StudentId`) REFERENCES `Students` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_student_group_group`
    FOREIGN KEY (`GroupId`) REFERENCES `Groups` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE;

ALTER TABLE StudentSubGroups
  ADD CONSTRAINT `fk_ssg_student`
    FOREIGN KEY (`StudentId`) REFERENCES `Students` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_ssg_group`
    FOREIGN KEY (`GroupId`) REFERENCES `Groups` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_ssg_subgroup`
    FOREIGN KEY (`SubGroupId`) REFERENCES `SubGroups` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE;

ALTER TABLE SubGroups
  ADD CONSTRAINT `fk_subgroups_group`
    FOREIGN KEY (`GroupId`) REFERENCES `Groups` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE;


-- =============================================================================
-- Optional: Pending* FKs (only if missing — uncomment if SHOW CREATE TABLE
-- PendingStudentGroups has no CONSTRAINT block)
-- =============================================================================
/*
ALTER TABLE PendingStudentGroups
  ADD CONSTRAINT `fk_pending_student_groups_student`
    FOREIGN KEY (`StudentId`) REFERENCES `PendingStudents` (`Id`)
    ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_pending_student_groups_group`
    FOREIGN KEY (`GroupId`) REFERENCES `Groups` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE;

ALTER TABLE PendingStudentSubGroups
  ADD CONSTRAINT `fk_pending_student_subgroups_student`
    FOREIGN KEY (`StudentId`) REFERENCES `PendingStudents` (`Id`)
    ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_pending_student_subgroups_group`
    FOREIGN KEY (`GroupId`) REFERENCES `Groups` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_pending_student_subgroups_subgroup`
    FOREIGN KEY (`SubGroupId`) REFERENCES `SubGroups` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE;
*/
