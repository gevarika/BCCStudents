namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// სტუდენტების ექსპორტის სერვისის ინტერფეისი
    /// </summary>
    public interface IStudentExportService
    {
        /// <summary>
        /// ექსპორტირებს სტუდენტებს Excel ფაილში
        /// </summary>
        /// <param name="filePath">ფაილის გზა, სადაც უნდა შეინახოს Excel ფაილი</param>
        void ExportStudentsToExcel(string filePath);
    }
}

