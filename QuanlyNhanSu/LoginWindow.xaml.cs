using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Input;
using QuanlyNhanSu.Classes; 

namespace QuanlyNhanSu
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            txtUsername.Focus();
            EnsureUserTableExists(); 
        }

        private void EnsureUserTableExists()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string qry = @"CREATE TABLE IF NOT EXISTS Users (
                                    Username TEXT PRIMARY KEY, 
                                    Password TEXT, 
                                    FullName TEXT, 
                                    Role TEXT)";
                    using (var cmd = new SQLiteCommand(qry, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            ProcessLogin();
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ProcessLogin();
            }
        }

        private void ProcessLogin()
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ mã nhân viên và mật khẩu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var conn = DatabaseHelper.GetConnection())
            {
                string qry = "SELECT * FROM Users WHERE Username = @username AND Password = @password";
                using (var cmd = new SQLiteCommand(qry, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            User currentUser = new User
                            {
                                Username = reader["Username"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Role = reader["Role"].ToString()
                            };

                            MessageBox.Show($"Xin chào bạn! Bạn đã đăng nhập thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                            MainWindow mainWindow = new MainWindow();
                            mainWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            txtUsername.Text = "";
                            txtPassword.Password = "";
                            txtUsername.Focus();
                        }
                    }
                }
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng điền tài khoản và mật khẩu trước khi bấm đăng ký!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string qryUser = "INSERT INTO Users (Username, Password, FullName, Role) VALUES (@username, @password, @fullname, @role)";
                    using (var cmdUser = new SQLiteCommand(qryUser, conn))
                    {
                        cmdUser.Parameters.AddWithValue("@username", username);
                        cmdUser.Parameters.AddWithValue("@password", password);
                        cmdUser.Parameters.AddWithValue("@fullname", "New User");
                        cmdUser.Parameters.AddWithValue("@role", "Staff");
                        cmdUser.ExecuteNonQuery();
                    }

                    MessageBox.Show("Tạo tài khoản thành công! Bạn có thể đăng nhập ngay.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (SQLiteException)
            {
                MessageBox.Show("Tài khoản này đã tồn tại trong hệ thống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
