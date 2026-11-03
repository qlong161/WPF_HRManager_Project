using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using QuanlyNhanSu.Classes;

namespace QuanlyNhanSu
{
    public partial class MainWindow : Window
    {
        private UIElement _dashboardContent;

        public MainWindow()
        {
            InitializeComponent();
            if (MainContentArea.Children.Count > 0)
            {
                _dashboardContent = MainContentArea.Children[0];
            }
            SetActiveButton(btnDashboard);
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            if (_dashboardContent == null) return;
            string todayStr = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    // 1. Tổng số nhân viên từ bảng Workers
                    int totalWorkers = 0;
                    using (var cmd = new SQLiteCommand("SELECT COUNT(1) FROM Workers", conn))
                    {
                        totalWorkers = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // 2. Nhân viên đang làm việc
                    int activeWorkers = totalWorkers;

                    // 3. Nhân viên đang nghỉ phép trong ngày hôm nay 
                    int onLeaveCount = 0;
                    try
                    {
                        string qryLeave = "SELECT COUNT(1) FROM NghiPhep WHERE @today BETWEEN FromDate AND ToDate AND Status = 'Approved'";
                        using (var cmd = new SQLiteCommand(qryLeave, conn))
                        {
                            cmd.Parameters.AddWithValue("@today", todayStr);
                            onLeaveCount = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }
                    catch { }

                    // 4. Ca làm việc được phân công trong ngày hôm nay
                    int todayShifts = 0;
                    try
                    {
                        string qryShift = "SELECT COUNT(1) FROM WaveShifts WHERE Date = @today";
                        using (var cmd = new SQLiteCommand(qryShift, conn))
                        {
                            cmd.Parameters.AddWithValue("@today", todayStr);
                            todayShifts = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }
                    catch { }
                    UpdateTextBlock("txtStatTotalWorkers", totalWorkers.ToString());
                    UpdateTextBlock("txtStatActiveWorkers", activeWorkers.ToString());
                    UpdateTextBlock("txtStatOnLeave", onLeaveCount.ToString("D2"));
                    UpdateTextBlock("txtStatTodayShifts", todayShifts.ToString("D2"));
                    LoadRecentActivities(conn);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load thống kê Dashboard: " + ex.Message);
            }
        }

        private void LoadRecentActivities(SQLiteConnection conn)
        {
            var activities = new List<ActivityItem>();

            try
            {
                string qryWorker = "SELECT Name, StartDate FROM Workers ORDER BY WN DESC LIMIT 3";
                using (var cmd = new SQLiteCommand(qryWorker, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        activities.Add(new ActivityItem
                        {
                            Icon = "♙",
                            BgColor = "#DBEAFE",
                            FgColor = "#2563EB",
                            Title = "Hồ sơ nhân sự mới",
                            Description = $"Nhân viên: {reader["Name"]}",
                            Time = "Gần đây"
                        });
                    }
                }
            }
            catch { }

            if (activities.Count == 0)
            {
                activities.Add(new ActivityItem
                {
                    Icon = "ℹ",
                    BgColor = "#F1F5F9",
                    FgColor = "#64748B",
                    Title = "Hệ thống hoạt động bình thường",
                    Description = "Chưa có phát sinh sự kiện mới trong ngày",
                    Time = DateTime.Now.ToString("HH:mm")
                });
            }

            var itemsControl = LogicalTreeHelper.FindLogicalNode(_dashboardContent, "icRecentActivities") as ItemsControl;
            if (itemsControl != null)
            {
                itemsControl.ItemsSource = activities;
            }
        }

        private void UpdateTextBlock(string name, string value)
        {
            if (_dashboardContent == null) return;
            var tb = LogicalTreeHelper.FindLogicalNode(_dashboardContent, name) as TextBlock;
            if (tb != null)
            {
                tb.Text = value;
            }
        }

        private void SetActiveButton(Button activeButton)
        {
            Button[] allButtons = { btnDashboard, btnEmpList, btnBreaks, btnManPower, btnInOut, btnWave, btnSettings };

            foreach (var btn in allButtons)
            {
                if (btn != null)
                {
                    btn.Style = (Style)FindResource("SidebarButton");
                }
            }

            if (activeButton != null)
            {
                activeButton.Style = (Style)FindResource("ActiveSidebarButton");
            }
        }
        private void ShowSection(string title, string description)
        {
            txtPageTitle.Text = title;

            Border content = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(15),
                Padding = new Thickness(35)
            };

            StackPanel panel = new StackPanel();

            TextBlock heading = new TextBlock
            {
                Text = title,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(30, 41, 59))
            };

            TextBlock subtitle = new TextBlock
            {
                Text = description,
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)),
                Margin = new Thickness(0, 10, 0, 25)
            };

            TextBlock placeholder = new TextBlock
            {
                Text = "Khu vực này sẽ hiển thị chức năng và dữ liệu tương ứng.",
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(100, 116, 139))
            };

            panel.Children.Add(heading);
            panel.Children.Add(subtitle);
            panel.Children.Add(placeholder);
            content.Child = panel;

            MainContentArea.Children.Clear();
            MainContentArea.Children.Add(content);
        }
        // 1. Tổng quan (Dashboard)
        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnDashboard);
            MainContentArea.Children.Clear();

            if (_dashboardContent != null)
            {
                LoadDashboardData();
                MainContentArea.Children.Add(_dashboardContent);
            }
            txtPageTitle.Text = "Tổng quan";
        }

        // 2. Quản lý nhân viên
        private void btnEmpList_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnEmpList);
            MainContentArea.Children.Clear();
            MainContentArea.Children.Add(new QuanlyNhanVien());
            txtPageTitle.Text = "Quản lý nhân viên";
        }

        // 3. Quản lý nghỉ phép
        private void btnBreaks_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnBreaks);
            MainContentArea.Children.Clear();
            MainContentArea.Children.Add(new QuanlyNghiPhep());
            txtPageTitle.Text = "Quản lý nghỉ phép";
        }

        // 4. Quản lý nhân lực
        private void btnManPower_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnManPower);
            MainContentArea.Children.Clear();
            MainContentArea.Children.Add(new QuanlyNhanLuc());
            txtPageTitle.Text = "Quản lý nhân lực";
        }

        
        // 6. Vào / Ra (Chấm công)
        private void btnInOut_Click(object sender, RoutedEventArgs e)
        {
            txtPageTitle.Text = "Chấm công vào / ra";
            MainContentArea.Children.Clear();
            QuanlyChamCong ucChamCong = new QuanlyChamCong();
            MainContentArea.Children.Add(ucChamCong);
            SetActiveButton(btnInOut);
        }

        // 7. Ca làm việc
        private void btnWave_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnWave);
            MainContentArea.Children.Clear();
            MainContentArea.Children.Add(new QuanlyCaLam());
            txtPageTitle.Text = "Quản lý ca làm việc";
        }

        // 8. Cài đặt hệ thống (Quản lý danh mục)
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnSettings);

            QuanlyDanhMuc popup = new QuanlyDanhMuc();
            popup.Owner = this;
            popup.ShowDialog();
        }
    }
}
