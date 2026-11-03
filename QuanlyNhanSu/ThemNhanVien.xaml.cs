using System;
using System.Data.SQLite;
using System.Windows;
using QuanlyNhanSu.Classes;

namespace QuanlyNhanSu
{
    public partial class ThemNhanVien : Window
    {
        private Worker _currentWorker;
        private bool _isEditMode = false;
        public ThemNhanVien(Worker worker)
        {
            InitializeComponent();
            LoadDepartments();

            _currentWorker = worker;
            if (_currentWorker != null)
            {
                _isEditMode = true;
                txtFormTitle.Text = "CẬP NHẬT HỒ SƠ NHÂN VIÊN";
                txtWN.IsReadOnly = true;
                txtWN.Background = System.Windows.Media.Brushes.LightGray;

                txtWN.Text = _currentWorker.WN;
                txtName.Text = _currentWorker.Name;
                cbDepartment.SelectedItem = _currentWorker.Department;

                if (DateTime.TryParseExact(_currentWorker.StartDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                    dpStartDate.SelectedDate = parsedDate;
            }
            else
            {
                dpStartDate.SelectedDate = DateTime.Now;
            }
        }
        private void LoadDepartments()
        {
            cbDepartment.Items.Clear();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    using (var cmd = new SQLiteCommand("SELECT DepartmentName FROM Departments", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbDepartment.Items.Add(reader["DepartmentName"].ToString());
                        }
                    }
                }
            }
            catch { }
            if (cbDepartment.Items.Count > 0) cbDepartment.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtWN.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Mã nhân viên và Họ tên không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string startDateStr = dpStartDate.SelectedDate?.ToString("dd/MM/yyyy") ?? DateTime.Now.ToString("dd/MM/yyyy");
            string totalYears = CalculateSeniority(dpStartDate.SelectedDate ?? DateTime.Now);
            string selectedDept = cbDepartment.SelectedItem?.ToString() ?? cbDepartment.Text;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string qry;
                    if (_isEditMode)
                    {
                        qry = "UPDATE Workers SET Name=@name, Department=@dept, StartDate=@start, TotalYears=@years WHERE WN=@wn";
                    }
                    else
                    {
                        var checkCmd = new SQLiteCommand("SELECT COUNT(1) FROM Workers WHERE WN=@wn", conn);
                        checkCmd.Parameters.AddWithValue("@wn", txtWN.Text.Trim());
                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Mã nhân viên này đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        qry = "INSERT INTO Workers (WN, Name, Department, StartDate, TotalYears) VALUES (@wn, @name, @dept, @start, @years)";
                    }

                    using (var cmd = new SQLiteCommand(qry, conn))
                    {
                        cmd.Parameters.AddWithValue("@wn", txtWN.Text.Trim());
                        cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("@dept", selectedDept);
                        cmd.Parameters.AddWithValue("@start", startDateStr);
                        cmd.Parameters.AddWithValue("@years", totalYears);
                        cmd.ExecuteNonQuery();
                    }
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private string CalculateSeniority(DateTime startDate)
        {
            DateTime now = DateTime.Now;
            int years = now.Year - startDate.Year;
            int months = now.Month - startDate.Month;

            if (now.Day < startDate.Day) months--;
            if (months < 0) { years--; months += 12; }

            if (years < 0) return "Chưa làm việc";
            if (years == 0 && months == 0) return "Dưới 1 tháng";
            if (years == 0) return $"{months} tháng";
            return $"{years} năm {months} tháng";
        }
    }
}
