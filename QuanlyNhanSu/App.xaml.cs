using System;
using System.Windows;

namespace QuanlyNhanSu
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                DatabaseHelper.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo cơ sở dữ liệu: " + ex.Message,
                                "Lỗi Nghiêm Trọng",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Current.Shutdown();
            }
        }
    }
}
