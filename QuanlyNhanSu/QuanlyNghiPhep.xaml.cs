using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using QuanlyNhanSu.Classes;

namespace QuanlyNhanSu
{
    public partial class QuanlyNghiPhep : UserControl
    {
        public QuanlyNghiPhep()
        {
            InitializeComponent();
            LoadBreakData();
        }

        private void LoadBreakData()
        {
            List<Break> breakList = new List<Break>();
            string keyword = txtSearch.Text.Trim();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string qry = @"
                        SELECT b.Break_ID, b.WN, w.Name, b.BreakStart, b.BreakEnd, b.Duration, b.Allowed, b.TimeLeft, b.Note
                        FROM Breaks b
                        JOIN Workers w ON b.WN = w.WN
                        WHERE (w.Name LIKE @key OR b.WN LIKE @key OR b.Note LIKE @key)";

                    using (var cmd = new SQLiteCommand(qry, conn))
                    {
                        cmd.Parameters.AddWithValue("@key", "%" + keyword + "%");

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                breakList.Add(new Break
                                {
                                    Break_ID = Convert.ToInt32(reader["Break_ID"]),
                                    WN = reader["WN"].ToString(),
                                    Name = reader["Name"].ToString(),
                                    BreakStart = reader["BreakStart"].ToString(),
                                    BreakEnd = reader["BreakEnd"].ToString(),
                                    Duration = reader["Duration"] != DBNull.Value ? Convert.ToDouble(reader["Duration"]) : 0,
                                    Allowed = reader["Allowed"] != DBNull.Value ? Convert.ToDouble(reader["Allowed"]) : 8.0,
                                    TimeLeft = reader["TimeLeft"] != DBNull.Value ? Convert.ToDouble(reader["TimeLeft"]) : 0,
                                    Note = reader["Note"].ToString()
                                });
                            }
                        }
                    }
                }
                dgBreaks.ItemsSource = breakList;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải dữ liệu nghỉ phép: " + ex.Message); }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadBreakData();

        private void btnAddBreak_Click(object sender, RoutedEventArgs e)
        {
            NghiPhepPopup popup = new NghiPhepPopup();
            if (popup.ShowDialog() == true)
            {
                LoadBreakData();
            }
        }

        private void btnDeleteBreak_Click(object sender, RoutedEventArgs e)
        {
            if (dgBreaks.SelectedItem is Break selectedBreak)
            {
                var confirm = MessageBox.Show($"Xóa đơn nghỉ phép của nhân viên {selectedBreak.Name}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var conn = DatabaseHelper.GetConnection())
                        {
                            string qry = "DELETE FROM Breaks WHERE Break_ID = @id";
                            using (var cmd = new SQLiteCommand(qry, conn))
                            {
                                cmd.Parameters.AddWithValue("@id", selectedBreak.Break_ID);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        LoadBreakData();
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi khi xóa: " + ex.Message); }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn bản ghi cần xóa trên bảng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
