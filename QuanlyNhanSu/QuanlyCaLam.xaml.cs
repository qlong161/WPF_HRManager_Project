using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using QuanlyNhanSu.Classes;

namespace QuanlyNhanSu
{
    public partial class QuanlyCaLam : UserControl
    {
        public QuanlyCaLam()
        {
            InitializeComponent();
            dpFilterDate.SelectedDate = DateTime.Today; 
            LoadShiftData();
        }

        private void LoadShiftData()
        {
            List<WaveShift> shiftList = new List<WaveShift>();
            string keyword = txtSearch.Text.Trim();
            string selectedDate = dpFilterDate.SelectedDate?.ToString("yyyy-MM-dd") ?? "";

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string qry = @"
                        SELECT s.ID, s.WN, w.Name, w.Department, s.Date, s.Shift_Duration, s.Break_Duration
                        FROM WaveShifts s
                        JOIN Workers w ON s.WN = w.WN
                        WHERE (w.Name LIKE @key OR s.WN LIKE @key)";

                    if (!string.IsNullOrEmpty(selectedDate))
                    {
                        qry += " AND s.Date = @date";
                    }

                    using (var cmd = new SQLiteCommand(qry, conn))
                    {
                        cmd.Parameters.AddWithValue("@key", "%" + keyword + "%");
                        cmd.Parameters.AddWithValue("@date", selectedDate);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                shiftList.Add(new WaveShift
                                {
                                    WN = reader["WN"].ToString(),
                                    Name = reader["Name"].ToString(),
                                    Department = reader["Department"].ToString(),
                                    Date = DateTime.Parse(reader["Date"].ToString()),
                                    Shift_Duration = reader["Shift_Duration"] != DBNull.Value ? Convert.ToInt32(reader["Shift_Duration"]) : 8,
                                    Break_Duration = reader["Break_Duration"] != DBNull.Value ? Convert.ToInt32(reader["Break_Duration"]) : 60
                                });
                            }
                        }
                    }
                }
                dgShifts.ItemsSource = shiftList;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải dữ liệu ca làm: " + ex.Message); }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadShiftData();
        private void dpFilterDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => LoadShiftData();

        private void btnAddShift_Click(object sender, RoutedEventArgs e)
        {
            PhanCaWindow popup = new PhanCaWindow();
            if (popup.ShowDialog() == true)
            {
                LoadShiftData();
            }
        }

        private void btnDeleteShift_Click(object sender, RoutedEventArgs e)
        {
            if (dgShifts.SelectedItem is WaveShift selectedShift)
            {
                var confirm = MessageBox.Show($"Xóa ca làm việc của nhân viên {selectedShift.Name} ngày {selectedShift.Date:dd/MM/yyyy}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var conn = DatabaseHelper.GetConnection())
                        {
                            string qry = "DELETE FROM WaveShifts WHERE WN = @wn AND Date = @date";
                            using (var cmd = new SQLiteCommand(qry, conn))
                            {
                                cmd.Parameters.AddWithValue("@wn", selectedShift.WN);
                                cmd.Parameters.AddWithValue("@date", selectedShift.Date.ToString("yyyy-MM-dd"));
                                cmd.ExecuteNonQuery();
                            }
                        }
                        LoadShiftData();
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi khi xóa: " + ex.Message); }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn ca làm việc cần xóa trên bảng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
