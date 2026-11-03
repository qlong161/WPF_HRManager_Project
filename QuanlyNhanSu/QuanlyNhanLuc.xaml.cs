using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using QuanlyNhanSu.Classes;

namespace QuanlyNhanSu
{
    public partial class QuanlyNhanLuc : UserControl
    {
        public QuanlyNhanLuc()
        {
            InitializeComponent();
            LoadDepartmentsFilter();
            LoadManPowerData();
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

        private void LoadManPowerData()
        {
            List<ManPower> manPowerList = new List<ManPower>();
            string keyword = txtSearch.Text.Trim();
            string deptFilter = cbFilterDept.SelectedItem?.ToString();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string qry = @"
                        SELECT w.WN, w.Name, w.Department, 
                               m.MainRole, m.RoleCapacity, 
                               m.BackUpRole, m.BackUpCapacity, 
                               m.BackUpRole2, m.NextCapacity 
                        FROM Workers w
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
                                manPowerList.Add(new ManPower
                                {
                                    WN = reader["WN"].ToString(),
                                    Name = reader["Name"].ToString(),
                                    Department = reader["Department"].ToString(),
                                    MainRole = reader["MainRole"] != DBNull.Value ? reader["MainRole"].ToString() : "Chưa phân công",
                                    RoleCapacity = reader["RoleCapacity"] != DBNull.Value ? Convert.ToInt32(reader["RoleCapacity"]) : 0,

                                    BackupRole = reader["BackUpRole"] != DBNull.Value ? reader["BackUpRole"].ToString() : "Không",
                                    BackUpCapacity = reader["BackUpCapacity"] != DBNull.Value ? Convert.ToInt32(reader["BackUpCapacity"]) : 0,

                                    BackupRole2 = reader["BackUpRole2"] != DBNull.Value ? reader["BackUpRole2"].ToString() : "Không",
                                    NextCapacity = reader["NextCapacity"] != DBNull.Value ? Convert.ToInt32(reader["NextCapacity"]) : 0
                                });
                            }
                        }
                    }
                }
                dgManPower.ItemsSource = manPowerList;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message); }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadManPowerData();
        private void cbFilterDept_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadManPowerData();

        private void btnUpdateSkill_Click(object sender, RoutedEventArgs e)
        {
            if (dgManPower.SelectedItem is ManPower selectedEmp)
            {
                CapNhatNhanLuc popup = new CapNhatNhanLuc(selectedEmp);
                if (popup.ShowDialog() == true) LoadManPowerData(); 
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một nhân viên trên bảng để cập nhật phân công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void dgManPower_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnUpdateSkill_Click(null, null);
        }
    }
}
