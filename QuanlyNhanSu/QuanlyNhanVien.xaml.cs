using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using QuanlyNhanSu.Classes;

namespace QuanlyNhanSu
{
    public partial class QuanlyNhanVien : UserControl
    {
        public QuanlyNhanVien()
        {
            InitializeComponent();
            LoadDepartmentsFilter();
            LoadWorkerData();
        }

        private void LoadDepartmentsFilter()
        {
            cbFilterDept.Items.Clear();
            cbFilterDept.Items.Add("Tất cả phòng ban");
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    using (var cmd = new SQLiteCommand("SELECT DepartmentName FROM Departments", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) cbFilterDept.Items.Add(reader["DepartmentName"].ToString());
                    }
                }
            }
            catch { }
            cbFilterDept.SelectedIndex = 0;
        }
        private void LoadWorkerData()
        {
            List<ManPower> workers = new List<ManPower>();
            string keyword = txtSearch.Text.Trim();
            string deptFilter = cbFilterDept.SelectedItem?.ToString();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string qry = @"
                        SELECT w.WN, w.Name, w.Department, w.StartDate, w.TotalYears, w.Status,
                               COALESCE(s.Shift_Duration, 0) AS ShiftDuration,
                               COALESCE(m.MainRole, 'Chưa phân công') AS MainRole,
                               COALESCE(m.RoleType, '') AS RoleType,
                               COALESCE(m.RoleCapacity, 0) AS RoleCapacity
                        FROM Workers w
                        LEFT JOIN (
                            SELECT WN, Shift_Duration, MAX(Date) 
                            FROM WaveShifts 
                            GROUP BY WN
                        ) s ON w.WN = s.WN
                        LEFT JOIN ManPower m ON w.WN = m.WN
                        WHERE (w.Name LIKE @key OR w.WN LIKE @key)";

                    if (deptFilter != "Tất cả phòng ban" && !string.IsNullOrEmpty(deptFilter))
                    {
                        qry += " AND w.Department = @dept";
                    }

                    using (var cmd = new SQLiteCommand(qry, conn))
                    {
                        cmd.Parameters.AddWithValue("@key", "%" + keyword + "%");
                        cmd.Parameters.AddWithValue("@dept", deptFilter);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int duration = Convert.ToInt32(reader["ShiftDuration"]);
                                string shiftText = duration > 0 ? $"Ca {duration} Giờ" : "Chưa phân ca";

                                workers.Add(new ManPower
                                {
                                    WN = reader["WN"].ToString(),
                                    Name = reader["Name"].ToString(),
                                    Department = reader["Department"] != DBNull.Value ? reader["Department"].ToString() : "Chưa phân bổ",
                                    ShiftDisplay = shiftText,
                                    MainRole = reader["MainRole"].ToString(), 
                                    RoleType = reader["RoleType"].ToString(),
                                    RoleCapacity = Convert.ToInt32(reader["RoleCapacity"]),
                                    StartDate = reader["StartDate"] != DBNull.Value ? reader["StartDate"].ToString() : "",
                                    TotalYears = reader["TotalYears"] != DBNull.Value ? reader["TotalYears"].ToString() : "Mới",
                                    Status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : "Active"
                                });
                            }
                        }
                    }
                }
                dgWorkers.ItemsSource = workers;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải dữ liệu nhân viên: " + ex.Message); }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadWorkerData();
        private void cbFilterDept_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadWorkerData();

        private void btnAddWorker_Click(object sender, RoutedEventArgs e)
        {
            ThemNhanVien popup = new ThemNhanVien(null);
            if (popup.ShowDialog() == true) LoadWorkerData();
        }

        private void btnEditWorker_Click(object sender, RoutedEventArgs e)
        {
            if (dgWorkers.SelectedItem is ManPower selectedWorker)
            {
                ThemNhanVien popup = new ThemNhanVien(selectedWorker);
                if (popup.ShowDialog() == true) LoadWorkerData();
            }
            else MessageBox.Show("Vui lòng chọn một nhân viên để sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void dgWorkers_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnEditWorker_Click(null, null);
        }

        private void btnDeleteWorker_Click(object sender, RoutedEventArgs e)
        {
            if (dgWorkers.SelectedItem is ManPower selectedWorker)
            {
                var confirm = MessageBox.Show($"Xóa nhân viên {selectedWorker.Name}? Việc này sẽ xóa toàn bộ dữ liệu phân công, chấm công của người này!", "Cảnh báo nguy hiểm", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (confirm == MessageBoxResult.Yes)
                {
                    using (var conn = DatabaseHelper.GetConnection())
                    {
                        var cmd = new SQLiteCommand("DELETE FROM Workers WHERE WN = @wn", conn);
                        cmd.Parameters.AddWithValue("@wn", selectedWorker.WN);
                        cmd.ExecuteNonQuery();
                    }
                    LoadWorkerData();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa trên bảng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
