using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Xml;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Presentation.Services;

namespace BCCStudents.Presentation
{
    public partial class ConnectionSettingsForm : Form
    {
        private readonly IConfigurationService _configService;

        public ConnectionSettingsForm(IConfigurationService configService)
        {
            InitializeComponent();
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));

            //txtConnectionString.Text = _configService.GetConnectionString("MySQLConnection");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _configService.SaveConnectionString("MySQLConnection", txtConnectionString.Text);
                MessageBox.Show("კავშირის სტრიქონი წარმატებით განახლდა!", "ინფორმაცია", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("შეცდომა: " + ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            /*SaveConnectionString(txtConnectionString.Text);
            MessageBox.Show("კავშირის სტრინგი წარმატებით განახლდა!", "ინფორმაცია", MessageBoxButtons.OK, MessageBoxIcon.Information);*/
        }

        /*private void SaveConnectionString(string newConnectionString)
        {
            string configPath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configPath);

            XmlNode node = xmlDoc.SelectSingleNode("//connectionStrings/add[@name='DbConnection']");
            if (node != null)
            {
                node.Attributes["connectionString"].Value = newConnectionString;
            }

            xmlDoc.Save(configPath);
            ConfigurationManager.RefreshSection("connectionStrings");
        }*/
    }
}

