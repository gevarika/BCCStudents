using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Interfaces;
using ClosedXML.Excel;

namespace BCCStudents.Application.Services
{
    public class StudentExportService : IStudentExportService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IStudentGroupRepository _studentGroupRepository;

        public StudentExportService(
            IStudentRepository studentRepository,
            IGroupRepository groupRepository,
            IStudentGroupRepository studentGroupRepository)
        {
            _studentRepository = studentRepository;
            _groupRepository = groupRepository;
            _studentGroupRepository = studentGroupRepository;
        }

        public void ExportStudentsToExcel(string filePath)
        {
            var groups = _groupRepository.GetAllGroups();
            var students = _studentRepository.GetAllStudents();

            using (var workbook = new XLWorkbook())
            {
                foreach (var group in groups)
                {
                    var worksheet = workbook.Worksheets.Add(group.Name);

                    // სათაურები
                    worksheet.Cell(1, 1).Value = "Student Code";
                    worksheet.Cell(1, 2).Value = "First Name";
                    worksheet.Cell(1, 3).Value = "Last Name";
                    worksheet.Cell(1, 4).Value = "Age";
                    worksheet.Cell(1, 5).Value = "Parent Name";
                    worksheet.Cell(1, 6).Value = "Phone Number";
                    worksheet.Cell(1, 7).Value = "Id_Numb";
                    worksheet.Cell(1, 8).Value = "Address";
                    worksheet.Cell(1, 9).Value = "RegistrationDate";
                    worksheet.Cell(1, 10).Value = "DateOfPayment";
                    worksheet.Cell(1, 11).Value = "TuitionFee";
                    worksheet.Cell(1, 12).Value = "Group";
                    worksheet.Cell(1, 13).Value = "SubGroup";
                    worksheet.Cell(1, 14).Value = "Discount";
                    worksheet.Cell(1, 15).Value = "Status";
                    worksheet.Cell(1, 16).Value = "PaymentStatus";
                    worksheet.Cell(1, 17).Value = "IdCardPath";
                    worksheet.Cell(1, 18).Value = "AdditionalDocsPath";
                    worksheet.Cell(1, 19).Value = "User_id";
                    worksheet.Cell(1, 20).Value = "Balance";

                    // ამ ჯგუფის აქტიური მოსწავლეების მიღება
                    var studentGroupLinks = _studentGroupRepository.GetActiveByGroupId(group.Id);
                    var studentIdsInGroup = studentGroupLinks.Select(sg => sg.StudentId).ToHashSet();

                    // მოსწავლეების ფილტრაცია
                    var studentsInGroup = students.Where(s => studentIdsInGroup.Contains(s.Id));

                    int row = 2;
                    foreach (var student in studentsInGroup)
                    {
                        // მოსწავლის ჯგუფის ინფორმაცია
                        var studentGroupInfo = studentGroupLinks.FirstOrDefault(sg => sg.StudentId == student.Id);

                        worksheet.Cell(row, 1).Value = student.StudentCode;
                        worksheet.Cell(row, 2).Value = student.FirstName;
                        worksheet.Cell(row, 3).Value = student.LastName;
                        worksheet.Cell(row, 4).Value = student.Age;
                        worksheet.Cell(row, 5).Value = student.ParentName;
                        worksheet.Cell(row, 6).Value = student.PhoneNumber;
                        worksheet.Cell(row, 7).Value = student.Id_Numb;
                        worksheet.Cell(row, 8).Value = student.Address;
                        worksheet.Cell(row, 9).Value = student.RegistrationDate;
                        worksheet.Cell(row, 10).Value = studentGroupInfo?.DateOfPayment;
                        worksheet.Cell(row, 11).Value = studentGroupInfo?.Price ?? 0;
                        worksheet.Cell(row, 12).Value = group.Name;
                        worksheet.Cell(row, 13).Value = ""; // TODO: SubGroup
                        worksheet.Cell(row, 14).Value = studentGroupInfo?.Discount ?? 0;
                        worksheet.Cell(row, 15).Value = student.Status;
                        worksheet.Cell(row, 16).Value = studentGroupInfo?.PaymentStatus;
                        worksheet.Cell(row, 17).Value = student.IdCardPath;
                        worksheet.Cell(row, 18).Value = student.AdditionalDocsPath;
                        worksheet.Cell(row, 19).Value = student.User_Id;
                        worksheet.Cell(row, 20).Value = student.Balance;
                        row++;
                    }
                }

                workbook.SaveAs(filePath);
            }
        }
    }
}



