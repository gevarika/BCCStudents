using BCCStudents.Domain.Entities;

namespace BCCStudents.Presentation
{
    public partial class StatusChangeGroupsForm : Form
    {
        public int? SelectedGroupId { get; private set; } // null ნიშნავს ყველა ჯგუფს

        public StatusChangeGroupsForm(List<Group> groups)
        {
            InitializeComponent();
            cmbGroups.DataSource = groups;
            cmbGroups.DisplayMember = "Name";
            cmbGroups.ValueMember = "Id";
            cmbGroups.SelectedIndex = -1;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cmbGroups.SelectedIndex >= 0)
                SelectedGroupId = ((Group)cmbGroups.SelectedItem).Id;
            else
                SelectedGroupId = null; // ყველა ჯგუფი
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnAllGroups_Click(object sender, EventArgs e)
        {
            SelectedGroupId = null;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
