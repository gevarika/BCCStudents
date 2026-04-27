namespace BCCStudents.Presentation
{
    public partial class ColumnManagementDialog : Form
    {
        private CheckedListBox checkedListBoxColumns;
        private Button btnOK;
        private Button btnCancel;
        private Button btnSelectAll;
        private Button btnDeselectAll;

        private Dictionary<string, bool> columnVisibility;
        private Dictionary<string, string> columnDisplayNames;
        // Dictionary display name -> column name-ისთვის
        private Dictionary<int, string> indexToColumnName = new Dictionary<int, string>();

        public Dictionary<string, bool> ResultColumnVisibility { get; private set; }

        public ColumnManagementDialog(Dictionary<string, bool> currentVisibility)
        {
            columnVisibility = new Dictionary<string, bool>(currentVisibility);
            InitializeComponent();
            InitializeDisplayNames();
            PopulateCheckedListBox();
        }

        private void InitializeDisplayNames()
        {
            columnDisplayNames = new Dictionary<string, string>
            {
                // StudentsEditForm-ის სვეტები
                { "Select", "Select (Checkbox)" },
                { "FirstName", "First Name" },
                { "LastName", "Last Name" },
                { "GroupName", "Group" },
                { "Age", "Age" },
                { "ParentName", "Parent Name" },
                { "PhoneNumber", "Phone Number" },
                { "Id_Numb", "Personal ID" },
                { "Address", "Address" },
                { "StudentCode", "Student Code" },
                { "RegistrationDate", "Registration Date" },
                { "DateOfPayment", "Payment Date" },
                { "TuitionFee", "Tuition Fee" },
                { "Discount", "Discount" },
                { "PaymentStatus", "Payment Status" },
                { "Status", "Status" },
                { "Balance", "Balance" },
                { "UpdatedAt", "Updated At" },
                // MainForm-ის dgvPayments სვეტები
                { "StudentID", "Student ID" },
                { "TotalPaid", "Total Paid" },
                { "AmountDue", "Amount Due" },
                { "NextPaymentDate", "Next Payment Date" }
            };
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form
            this.Text = "სვეტების მართვა";
            this.Size = new System.Drawing.Size(350, 550);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // CheckedListBox
            checkedListBoxColumns = new CheckedListBox();
            checkedListBoxColumns.Location = new System.Drawing.Point(12, 12);
            checkedListBoxColumns.Size = new System.Drawing.Size(310, 420);
            checkedListBoxColumns.CheckOnClick = true;
            this.Controls.Add(checkedListBoxColumns);

            // Select All Button
            btnSelectAll = new Button();
            btnSelectAll.Text = "ყველას მონიშვნა";
            btnSelectAll.Location = new System.Drawing.Point(12, 440);
            btnSelectAll.Size = new System.Drawing.Size(100, 30);
            btnSelectAll.Click += BtnSelectAll_Click;
            this.Controls.Add(btnSelectAll);

            // Deselect All Button
            btnDeselectAll = new Button();
            btnDeselectAll.Text = "ყველას გაუქმება";
            btnDeselectAll.Location = new System.Drawing.Point(122, 440);
            btnDeselectAll.Size = new System.Drawing.Size(100, 30);
            btnDeselectAll.Click += BtnDeselectAll_Click;
            this.Controls.Add(btnDeselectAll);

            // OK Button
            btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new System.Drawing.Point(166, 480);
            btnOK.Size = new System.Drawing.Size(75, 30);
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            // Cancel Button
            btnCancel = new Button();
            btnCancel.Text = "გაუქმება";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(247, 480);
            btnCancel.Size = new System.Drawing.Size(75, 30);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;

            this.ResumeLayout(false);
        }

        private void PopulateCheckedListBox()
        {
            checkedListBoxColumns.Items.Clear();

            // Id და GroupId-ს არ ჩავრთავთ, რადგან ისინი ყოველთვის უნდა იყოს (თუმცა ფარული)
            var visibleColumns = columnVisibility.Keys
                .Where(key => key != "Id" && key != "GroupId")
                .OrderBy(key =>
                {
                    // რიგის განსაზღვრა (StudentsEditForm და MainForm-ის სვეტები)
                    var order = new[] {
                        "Select", "StudentCode", "StudentID", "FirstName", "LastName", "GroupName",
                        "Age", "ParentName", "PhoneNumber", "Id_Numb", "Address", "RegistrationDate",
                        "DateOfPayment", "TuitionFee", "TotalPaid", "AmountDue", "NextPaymentDate",
                        "Discount", "PaymentStatus", "Status", "Balance", "UpdatedAt"
                    };
                    int index = Array.IndexOf(order, key);
                    return index >= 0 ? index : 999;
                });

            int index = 0;
            foreach (var columnName in visibleColumns)
            {
                string displayName = columnDisplayNames.ContainsKey(columnName)
                    ? columnDisplayNames[columnName]
                    : columnName;
                checkedListBoxColumns.Items.Add(displayName, columnVisibility[columnName]);
                indexToColumnName[index] = columnName;
                index++;
            }
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBoxColumns.Items.Count; i++)
            {
                checkedListBoxColumns.SetItemChecked(i, true);
            }
        }

        private void BtnDeselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBoxColumns.Items.Count; i++)
            {
                // Select-ს არ ვაუქმებთ, რადგან ის ყოველთვის უნდა იყოს ჩანს
                if (indexToColumnName.ContainsKey(i))
                {
                    var columnName = indexToColumnName[i];
                    if (columnName != "Select")
                    {
                        checkedListBoxColumns.SetItemChecked(i, false);
                    }
                }
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            ResultColumnVisibility = new Dictionary<string, bool>();

            // Id და GroupId ყოველთვის false (ფარული)
            ResultColumnVisibility["Id"] = false;
            ResultColumnVisibility["GroupId"] = false;

            for (int i = 0; i < checkedListBoxColumns.Items.Count; i++)
            {
                if (indexToColumnName.ContainsKey(i))
                {
                    var columnName = indexToColumnName[i];
                    ResultColumnVisibility[columnName] = checkedListBoxColumns.GetItemChecked(i);
                }
            }
        }
    }
}
