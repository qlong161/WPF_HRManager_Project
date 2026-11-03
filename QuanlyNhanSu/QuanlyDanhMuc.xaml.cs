using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Windows;

namespace QuanlyNhanSu
{
    public partial class QuanlyDanhMuc : Window
    {
        public ObservableCollection<string> DeptList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> RolesList { get; set; } = new ObservableCollection<string>();

        public QuanlyDanhMuc()
        {
            InitializeComponent();
            lstDepartments.ItemsSource = DeptList;
            lstRoles.ItemsSource = RolesList;

            EnsureDepartmentTableExists();
            LoadMasterData();
        }
        private void EnsureDepartmentTableExists()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string qry = "CREATE TABLE IF NOT EXISTS Departments (DepartmentName TEXT PRIMARY KEY);";
                    using (var cmd = new SQLiteCommand(qry, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi khởi tạo bảng Phòng ban: " + ex.Message); }
        }
        private void LoadMasterData()
        {
            DeptList.Clear();
            RolesList.Clear();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    using (var cmdDept = new SQLiteCommand("SELECT DepartmentName FROM Departments", conn))
                    using (var reader = cmdDept.ExecuteReader())
                    {
                        while (reader.Read())
                            DeptList.Add(reader["DepartmentName"].ToString());
                    }
                    using (var cmdRole = new SQLiteCommand("SELECT RoleName FROM Roles", conn))
                    using (var reader = cmdRole.ExecuteReader())
                    {
                        while (reader.Read())
                            RolesList.Add(reader["RoleName"].ToString());
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message); }
        }
        private void btnAddDept_Click(object sender, RoutedEventArgs e)
        {
            string newDept = txtDeptName.Text.Trim();
            if (string.IsNullOrEmpty(newDept)) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string qry = "INSERT INTO Departments (DepartmentName) VALUES (@name)";
                    using (var cmd = new SQLiteCommand(qry, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", newDept);
                        cmd.ExecuteNonQuery();
                    }
                }
                DeptList.Add(newDept);
                txtDeptName.Text = "";
            }
            catch (SQLiteException) { MessageBox.Show("Phòng ban này đã tồn tại!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        private void btnDeleteDept_Click(object sender, RoutedEventArgs e)
        {
            if (lstDepartments.SelectedItem is string selectedDept)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa phòng ban '{selectedDept}' không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var conn = DatabaseHelper.GetConnection())
                        {
                            string qry = "DELETE FROM Departments WHERE DepartmentName = @name";
                            using (var cmd = new SQLiteCommand(qry, conn))
                            {
                                cmd.Parameters.AddWithValue("@name", selectedDept);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        DeptList.Remove(selectedDept);
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi xóa: " + ex.Message); }
                }
            }
        }

        private void btnAddRole_Click(object sender, RoutedEventArgs e)
        {
            string newRole = txtRoleName.Text.Trim();
            if (string.IsNullOrEmpty(newRole)) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string qry = "INSERT INTO Roles (RoleName) VALUES (@name)";
                    using (var cmd = new SQLiteCommand(qry, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", newRole);
                        cmd.ExecuteNonQuery();
                    }
                }
                RolesList.Add(newRole);
                txtRoleName.Text = "";
            }
            catch (SQLiteException) { MessageBox.Show("Chức vụ này đã tồn tại!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        private void btnDeleteRole_Click(object sender, RoutedEventArgs e)
        {
            if (lstRoles.SelectedItem is string selectedRole)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa chức vụ '{selectedRole}' không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var conn = DatabaseHelper.GetConnection())
                        {
                            string qry = "DELETE FROM Roles WHERE RoleName = @name";
                            using (var cmd = new SQLiteCommand(qry, conn))
                            {
                                cmd.Parameters.AddWithValue("@name", selectedRole);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        RolesList.Remove(selectedRole);
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi xóa: " + ex.Message); }
                }
            }
        }
    }
}
