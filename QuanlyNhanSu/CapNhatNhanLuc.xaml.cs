using System;
using System.Data.SQLite;
using System.Windows;
using QuanlyNhanSu.Classes;

namespace QuanlyNhanSu
{
    public partial class CapNhatNhanLuc : Window
    {
        private ManPower _employee;

        public CapNhatNhanLuc(ManPower employee)
        {
            InitializeComponent();
            _employee = employee;

            LoadRolesFromMasterData();
            LoadEmployeeInfo();
        }
        private void LoadRolesFromMasterData()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    using (var cmd = new SQLiteCommand("SELECT RoleName FROM Roles", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        cbMainRole.Items.Add("Chưa phân công");
                        cbBackUp1.Items.Add("Không");
                        cbBackUp2.Items.Add("Không");

                        while (reader.Read())
                        {
                            string role = reader["RoleName"].ToString();
                            cbMainRole.Items.Add(role);
                            cbBackUp1.Items.Add(role);
                            cbBackUp2.Items.Add(role);
                        }
                    }
                }
            }
            catch { }
        }
        private void LoadEmployeeInfo()
        {
            if (_employee == null) return;

            // Hồ sơ gốc
            txtWN.Text = _employee.WN;
            txtName.Text = _employee.Name;
            txtDepartment.Text = _employee.Department;

            // Dữ liệu phân công
            cbMainRole.SelectedItem = _employee.MainRole;
            txtMainCap.Text = _employee.RoleCapacity.ToString();

            cbBackUp1.SelectedItem = _employee.BackupRole;
            txtBackUpCap1.Text = _employee.BackUpCapacity.ToString();

            cbBackUp2.SelectedItem = _employee.BackupRole2;
            txtBackUpCap2.Text = _employee.NextCapacity.ToString();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            int mainCap = int.TryParse(txtMainCap.Text, out int m) ? m : 0;
            int b1Cap = int.TryParse(txtBackUpCap1.Text, out int b1) ? b1 : 0;
            int b2Cap = int.TryParse(txtBackUpCap2.Text, out int b2) ? b2 : 0;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                   
                    string qry = @"
                        INSERT OR REPLACE INTO ManPower 
                        (WN, RoleType, MainRole, RoleCapacity, BackUpRole, BackUpCapacity, BackUpRole2, NextCapacity) 
                        VALUES 
                        (@wn, @roleType, @mainRole, @mainCap, @backUp1, @backUp1Cap, @backUp2, @backUp2Cap)";

                    using (var cmd = new SQLiteCommand(qry, conn))
                    {
                        cmd.Parameters.AddWithValue("@wn", _employee.WN);
                        cmd.Parameters.AddWithValue("@roleType", ""); 

                        cmd.Parameters.AddWithValue("@mainRole", cbMainRole.Text);
                        cmd.Parameters.AddWithValue("@mainCap", mainCap);

                        cmd.Parameters.AddWithValue("@backUp1", cbBackUp1.Text);
                        cmd.Parameters.AddWithValue("@backUp1Cap", b1Cap);

                        cmd.Parameters.AddWithValue("@backUp2", cbBackUp2.Text);
                        cmd.Parameters.AddWithValue("@backUp2Cap", b2Cap);

                        cmd.ExecuteNonQuery();
                    }
                }

                this.DialogResult = true; 
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu phân công: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
