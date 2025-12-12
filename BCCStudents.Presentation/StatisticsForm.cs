using BCCStudents.Infrastructure.Data;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Application.Services;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BCCStudents.Application.Interfaces;

namespace BCCStudents.Presentation
{
    public partial class StatisticsForm : Form
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IGroupRepository _groupRepository;

        public StatisticsForm(IStatisticsService statisticsService, IGroupRepository groupRepository)
        {
            InitializeComponent();
            _statisticsService = statisticsService;
            _groupRepository = groupRepository;
            FormTitleHelper.SetTitle(this, "სტატისტიკა");
        }

        private void StatisticsForm_Load(object sender, EventArgs e)
        {
            InitializeFilters();
            InitializeCharts();
            LoadStatistics();
        }

        private void InitializeFilters()
        {
            // ჯგუფების ComboBox-ის შევსება
            cmbGroup.Items.Add("ყველა ჯგუფი");
            var groups = _groupRepository.GetAllGroups();
            foreach (var group in groups)
            {
                cmbGroup.Items.Add(group.Name);
            }
            cmbGroup.SelectedIndex = 0;

            // თარიღების დაყენება
            dtpStartDate.Value = DateTime.Now.AddMonths(-6);
            dtpEndDate.Value = DateTime.Now;
        }

        private void InitializeCharts()
        {
            // ყველა Chart-ის საერთო კონფიგურაცია
            var charts = new[] 
            { 
                chartStudentCount, chartPayments, chartPaymentStatus, chartGender, 
                chartAge, chartNames, chartRegistration, chartGroupDensity, 
                chartAveragePayment, chartDiscounts, chartSubGroupDistribution, 
                chartPaymentTrend, chartGroupEfficiency 
            };

            foreach (var chart in charts)
            {
                chart.ChartAreas.Clear();
                chart.Series.Clear();
                chart.Legends.Clear();
                
                var chartArea = new ChartArea("MainArea");
                chartArea.AxisX.Title = "კატეგორია";
                chartArea.AxisY.Title = "რაოდენობა";
                chart.ChartAreas.Add(chartArea);
                
                var legend = new Legend("MainLegend");
                legend.Docking = Docking.Bottom;
                chart.Legends.Add(legend);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            int? groupId = null;
            if (cmbGroup.SelectedIndex > 0)
            {
                var selectedGroupName = cmbGroup.SelectedItem.ToString();
                var group = _groupRepository.GetAllGroups().FirstOrDefault(g => g.Name == selectedGroupName);
                if (group != null)
                    groupId = group.Id;
            }

            DateTime? startDate = dtpStartDate.Value.Date;
            DateTime? endDate = dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1);

            // Tab 1: სტუდენტების რაოდენობა
            LoadStudentCountStatistics(groupId, startDate, endDate);

            // Tab 2: გადახდების სტატისტიკა
            LoadPaymentStatistics(groupId, startDate, endDate);

            // Tab 3: გადახდილი/გადასახდელი მოსწავლეები
            LoadPaymentStatusStatistics(groupId);

            // Tab 4: გენდერული სტატისტიკა
            LoadGenderStatistics(groupId);

            // Tab 5: ასაკის სტატისტიკა
            LoadAgeStatistics(groupId);

            // Tab 6: სახელების სტატისტიკა
            LoadNameStatistics(groupId);

            // Tab 7: რეგისტრაციის სტატისტიკა
            LoadRegistrationStatistics(startDate, endDate);

            // Tab 8: ჯგუფების სიმჭიდროვე
            LoadGroupDensityStatistics();

            // Tab 9: საშუალო გადახდა
            LoadAveragePaymentStatistics(groupId);

            // Tab 10: ფასდაკლებების სტატისტიკა
            LoadDiscountStatistics(groupId);

            // Tab 11: მოსწავლეების განაწილება ქვეჯგუფებში
            LoadSubGroupDistributionStatistics(groupId);

            // Tab 12: გადახდების ტენდენცია
            LoadPaymentTrendStatistics(startDate, endDate);

            // Tab 13: ჯგუფების ეფექტურობა
            LoadGroupEfficiencyStatistics();
        }

        #region Tab 1: სტუდენტების რაოდენობა

        private void LoadStudentCountStatistics(int? groupId, DateTime? startDate, DateTime? endDate)
        {
            var data = _statisticsService.GetStudentCountByGroup(groupId, startDate, endDate);
            dgvStudentCount.DataSource = data;

            // Chart
            chartStudentCount.Series.Clear();
            var seriesActive = new Series("აქტიური") { ChartType = SeriesChartType.Column };
            var seriesInactive = new Series("არააქტიური") { ChartType = SeriesChartType.Column };

            foreach (DataRow row in data.Rows)
            {
                var groupName = row["ჯგუფი"].ToString();
                seriesActive.Points.AddXY(groupName, Convert.ToInt32(row["აქტიური"]));
                seriesInactive.Points.AddXY(groupName, Convert.ToInt32(row["არააქტიური"]));
            }

            chartStudentCount.Series.Add(seriesActive);
            chartStudentCount.Series.Add(seriesInactive);
            chartStudentCount.ChartAreas[0].AxisX.Interval = 1;
        }

        #endregion

        #region Tab 2: გადახდების სტატისტიკა

        private void LoadPaymentStatistics(int? groupId, DateTime? startDate, DateTime? endDate)
        {
            var data = _statisticsService.GetPaymentStatisticsByGroup(groupId, startDate, endDate);
            dgvPayments.DataSource = data;

            // საერთო თანხები
            var totals = _statisticsService.GetTotalPaymentStatistics(groupId, startDate, endDate);
            lblTotalPaid.Text = $"გადახდილი: {totals.TotalPaid:F2} ₾";
            lblTotalPending.Text = $"გადასახდელი: {totals.TotalPending:F2} ₾";

            // Chart - დროის მიხედვით
            var timeData = _statisticsService.GetPaymentStatisticsByTime(startDate, endDate);
            chartPayments.Series.Clear();
            var seriesPaid = new Series("გადახდილი") { ChartType = SeriesChartType.Line };
            var seriesPending = new Series("გადასახდელი") { ChartType = SeriesChartType.Line };

            foreach (DataRow row in timeData.Rows)
            {
                var date = row["თარიღი"].ToString();
                seriesPaid.Points.AddXY(date, Convert.ToDecimal(row["გადახდილი"]));
                seriesPending.Points.AddXY(date, Convert.ToDecimal(row["გადასახდელი"]));
            }

            chartPayments.Series.Add(seriesPaid);
            chartPayments.Series.Add(seriesPending);
            chartPayments.ChartAreas[0].AxisX.Interval = 1;
        }

        #endregion

        #region Tab 3: გადახდილი/გადასახდელი მოსწავლეები

        private void LoadPaymentStatusStatistics(int? groupId)
        {
            var data = _statisticsService.GetStudentsPaymentStatus(groupId);
            dgvPaymentStatus.DataSource = data;

            var counts = _statisticsService.GetStudentsPaymentStatusCount(groupId);

            // Chart - Pie
            chartPaymentStatus.Series.Clear();
            var series = new Series("სტატუსი") { ChartType = SeriesChartType.Pie };
            series.Points.AddXY("გადახდილი", counts.PaidCount);
            series.Points.AddXY("გადასახდელი", counts.PendingCount);
            chartPaymentStatus.Series.Add(series);
        }

        #endregion

        #region Tab 4: გენდერული სტატისტიკა

        private void LoadGenderStatistics(int? groupId)
        {
            var data = _statisticsService.GetGenderStatistics(groupId);
            dgvGender.DataSource = data;

            // Chart - Pie
            chartGender.Series.Clear();
            var series = new Series("გენდერი") { ChartType = SeriesChartType.Pie };

            foreach (DataRow row in data.Rows)
            {
                series.Points.AddXY(
                    row["გენდერი"].ToString(), 
                    Convert.ToInt32(row["რაოდენობა"])
                );
            }

            chartGender.Series.Add(series);
        }

        #endregion

        #region Tab 5: ასაკის სტატისტიკა

        private void LoadAgeStatistics(int? groupId)
        {
            var data = _statisticsService.GetAgeStatistics(groupId);
            dgvAge.DataSource = data;

            // Chart - Column
            chartAge.Series.Clear();
            var series = new Series("ასაკი") { ChartType = SeriesChartType.Column };

            foreach (DataRow row in data.Rows)
            {
                series.Points.AddXY(
                    row["ასაკის დიაპაზონი"].ToString(),
                    Convert.ToInt32(row["რაოდენობა"])
                );
            }

            chartAge.Series.Add(series);
            chartAge.ChartAreas[0].AxisX.Interval = 1;
        }

        #endregion

        #region Tab 6: სახელების სტატისტიკა

        private void LoadNameStatistics(int? groupId)
        {
            var data = _statisticsService.GetNameStatistics(groupId, 10);
            dgvNames.DataSource = data;

            // Chart - Column
            chartNames.Series.Clear();
            var series = new Series("სახელი") { ChartType = SeriesChartType.Column };

            foreach (DataRow row in data.Rows)
            {
                series.Points.AddXY(
                    row["სახელი"].ToString(),
                    Convert.ToInt32(row["რაოდენობა"])
                );
            }

            chartNames.Series.Add(series);
            chartNames.ChartAreas[0].AxisX.Interval = 1;
        }

        #endregion

        #region Tab 7: რეგისტრაციის სტატისტიკა

        private void LoadRegistrationStatistics(DateTime? startDate, DateTime? endDate)
        {
            var data = _statisticsService.GetRegistrationStatistics(startDate, endDate);
            dgvRegistration.DataSource = data;

            // Chart - Line
            chartRegistration.Series.Clear();
            var series = new Series("რეგისტრაცია") { ChartType = SeriesChartType.Line };

            foreach (DataRow row in data.Rows)
            {
                series.Points.AddXY(
                    row["თვე"].ToString(),
                    Convert.ToInt32(row["რაოდენობა"])
                );
            }

            chartRegistration.Series.Add(series);
            chartRegistration.ChartAreas[0].AxisX.Interval = 1;
        }

        #endregion

        #region Tab 8: ჯგუფების სიმჭიდროვე

        private void LoadGroupDensityStatistics()
        {
            var data = _statisticsService.GetGroupDensityStatistics();
            dgvGroupDensity.DataSource = data;

            // Chart - Column
            chartGroupDensity.Series.Clear();
            var seriesCount = new Series("მოსწავლეების რაოდენობა") { ChartType = SeriesChartType.Column };
            var seriesAverage = new Series("საშუალო") { ChartType = SeriesChartType.Line };

            double average = 0;
            if (data.Rows.Count > 0)
            {
                average = Convert.ToDouble(data.Rows[0]["საშუალო"]);
            }

            foreach (DataRow row in data.Rows)
            {
                var groupName = row["ჯგუფი"].ToString();
                seriesCount.Points.AddXY(groupName, Convert.ToInt32(row["მოსწავლეების რაოდენობა"]));
                seriesAverage.Points.AddXY(groupName, average);
            }

            chartGroupDensity.Series.Add(seriesCount);
            chartGroupDensity.Series.Add(seriesAverage);
            chartGroupDensity.ChartAreas[0].AxisX.Interval = 1;
        }

        #endregion

        #region Tab 9: საშუალო გადახდა

        private void LoadAveragePaymentStatistics(int? groupId)
        {
            var data = _statisticsService.GetAveragePaymentStatistics(groupId);
            dgvAveragePayment.DataSource = data;

            // Chart - Column
            chartAveragePayment.Series.Clear();
            var seriesPaid = new Series("საშუალო გადახდილი") { ChartType = SeriesChartType.Column };
            var seriesPending = new Series("საშუალო გადასახდელი") { ChartType = SeriesChartType.Column };

            foreach (DataRow row in data.Rows)
            {
                var groupName = row["ჯგუფი"].ToString();
                seriesPaid.Points.AddXY(groupName, Convert.ToDecimal(row["საშუალო გადახდილი"]));
                seriesPending.Points.AddXY(groupName, Convert.ToDecimal(row["საშუალო გადასახდელი"]));
            }

            chartAveragePayment.Series.Add(seriesPaid);
            chartAveragePayment.Series.Add(seriesPending);
            chartAveragePayment.ChartAreas[0].AxisX.Interval = 1;
        }

        #endregion

        #region Tab 10: ფასდაკლებების სტატისტიკა

        private void LoadDiscountStatistics(int? groupId)
        {
            var data = _statisticsService.GetDiscountStatistics(groupId);
            dgvDiscounts.DataSource = data;

            // Chart - Column
            chartDiscounts.Series.Clear();
            var series = new Series("ფასდაკლება %") { ChartType = SeriesChartType.Column };

            foreach (DataRow row in data.Rows)
            {
                series.Points.AddXY(
                    row["ჯგუფი"].ToString(),
                    Convert.ToDouble(row["საშუალო ფასდაკლება %"])
                );
            }

            chartDiscounts.Series.Add(series);
            chartDiscounts.ChartAreas[0].AxisX.Interval = 1;
        }

        #endregion

        #region Tab 11: მოსწავლეების განაწილება ქვეჯგუფებში

        private void LoadSubGroupDistributionStatistics(int? groupId)
        {
            var data = _statisticsService.GetSubGroupDistributionStatistics(groupId);
            dgvSubGroupDistribution.DataSource = data;

            // Chart - Pie
            chartSubGroupDistribution.Series.Clear();
            var series = new Series("ქვეჯგუფი") { ChartType = SeriesChartType.Pie };

            foreach (DataRow row in data.Rows)
            {
                series.Points.AddXY(
                    row["ქვეჯგუფი"].ToString(),
                    Convert.ToInt32(row["მოსწავლეების რაოდენობა"])
                );
            }

            chartSubGroupDistribution.Series.Add(series);
        }

        #endregion

        #region Tab 12: გადახდების ტენდენცია

        private void LoadPaymentTrendStatistics(DateTime? startDate, DateTime? endDate)
        {
            var data = _statisticsService.GetPaymentTrendStatistics(startDate, endDate);
            dgvPaymentTrend.DataSource = data;

            // Chart - Line
            chartPaymentTrend.Series.Clear();
            var series = new Series("გადახდილი") { ChartType = SeriesChartType.Line };

            foreach (DataRow row in data.Rows)
            {
                series.Points.AddXY(
                    row["თარიღი"].ToString(),
                    Convert.ToDecimal(row["გადახდილი"])
                );
            }

            chartPaymentTrend.Series.Add(series);
            chartPaymentTrend.ChartAreas[0].AxisX.Interval = 1;
        }

        #endregion

        #region Tab 13: ჯგუფების ეფექტურობა

        private void LoadGroupEfficiencyStatistics()
        {
            var data = _statisticsService.GetGroupEfficiencyStatistics();
            dgvGroupEfficiency.DataSource = data;

            // Chart - Column
            chartGroupEfficiency.Series.Clear();
            var series = new Series("ეფექტურობა %") { ChartType = SeriesChartType.Column };

            foreach (DataRow row in data.Rows)
            {
                series.Points.AddXY(
                    row["ჯგუფი"].ToString(),
                    Convert.ToDouble(row["ეფექტურობა %"])
                );
            }

            chartGroupEfficiency.Series.Add(series);
            chartGroupEfficiency.ChartAreas[0].AxisX.Interval = 1;
        }

        #endregion
    }
}

