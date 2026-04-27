using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Presentation
{
    public partial class FailedStudentsForm : Form
    {
        private readonly IStudentService _studentService;
        private List<Student> students = new List<Student>();
        public FailedStudentsForm(IStudentService studentService)
        {
            InitializeComponent();
            _studentService = studentService;
            FormTitleHelper.SetTitle(this, "მოსწავლის დამატების შეცდომა");
            LoadData(); // მონაცემების ჩატვირთვა
        }
        // JSON-დან მონაცემების ჩატვირთვა DataGridView-ში
        private void LoadData()
        {
            students = _studentService.LoadStudentsFromJson();
            dataGridViewStudents.DataSource = new BindingSource { DataSource = students };
            /*students = StudentJsonHelper.LoadStudents();
            dataGridViewStudents.DataSource = new BindingSource { DataSource = students };*/
        }

        private void btnDeleteStudent_Click(object sender, EventArgs e)
        {
            if (dataGridViewStudents.SelectedRows.Count > 0)
            {
                var selectedIndex = dataGridViewStudents.SelectedRows[0].Index;
                _studentService.DeleteStudentFromJson(students, selectedIndex);
                LoadData();
            }
            /*if (dataGridViewStudents.SelectedRows.Count > 0)
            {
                var selectedIndex = dataGridViewStudents.SelectedRows[0].Index;
                students.RemoveAt(selectedIndex);
                StudentJsonHelper.SaveStudents(students);
                LoadData();
            }*/
        }

        private void btnAddStudent_Click(object sender, EventArgs e)
        {
            MessageBox.Show("მონაცემების დამატება ბაზაში", "კოდი დასამატებელია");


            LoadData();
        }

        private void dataGridViewStudents_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            _studentService.SaveStudentsToJson(students);
            //StudentJsonHelper.SaveStudents(students);
        }
    }
}

