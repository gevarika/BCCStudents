using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// სტუდენტების იმპორტის სერვისის ინტერფეისი
    /// </summary>
    public interface IImportService
    {
        /// <summary>
        /// Excel ფაილიდან სტუდენტების იმპორტი
        /// </summary>
        /// <param name="fileStream">Excel ფაილის Stream</param>
        /// <param name="configuration">მეპინგის კონფიგურაცია</param>
        /// <param name="progress">პროგრესის რეპორტირება (current, total, status message)</param>
        /// <returns>იმპორტის შედეგი</returns>
        Task<ImportResult> ImportStudentsAsync(
            Stream fileStream,
            ImportMappingConfiguration configuration,
            IProgress<(int current, int total, string status)> progress = null);

        /// <summary>
        /// Excel ფაილიდან sheet-ების სიის მიღება
        /// </summary>
        /// <param name="fileStream">Excel ფაილის Stream</param>
        /// <returns>Sheet-ების სახელების სია</returns>
        Task<List<string>> GetExcelSheetsAsync(Stream fileStream);

        /// <summary>
        /// Excel sheet-ის header-ების მიღება
        /// </summary>
        /// <param name="fileStream">Excel ფაილის Stream</param>
        /// <param name="sheetName">Sheet-ის სახელი</param>
        /// <returns>Header-ების სახელების სია</returns>
        Task<List<string>> GetSheetHeadersAsync(Stream fileStream, string sheetName);

        /// <summary>
        /// Excel sheet-ის პრევიუ მონაცემების მიღება (მაქსიმუმ N მწკრივი)
        /// </summary>
        /// <param name="fileStream">Excel ფაილის Stream</param>
        /// <param name="sheetName">Sheet-ის სახელი</param>
        /// <param name="maxRows">მაქსიმალური მწკრივების რაოდენობა პრევიუსთვის</param>
        /// <returns>DataTable პრევიუსთვის</returns>
        Task<System.Data.DataTable> GetSheetPreviewAsync(Stream fileStream, string sheetName, int maxRows = 100);
    }
}
