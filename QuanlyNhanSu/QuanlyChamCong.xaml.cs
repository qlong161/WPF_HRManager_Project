using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using QuanlyNhanSu.Classes; 

namespace QuanlyNhanSu
{
    public partial class QuanlyChamCong : UserControl
    {
        public QuanlyChamCong()
        {
            InitializeComponent();
            dpFilterDate.SelectedDate = DateTime.Today; 
            LoadInOutData();
        }

        private void LoadInOutData()
        {
            List<InOut> inoutList = new List<InOut>();
            string keyword = txtSearch.Text.Trim();
            string selectedDate = dpFilterDate.SelectedDate?.ToString("yyyy-MM-dd") ?? "";

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string qry = @"
                        SELECT i.ID, i.WN, w.Name, i.Date, i.In_Work, i.Out_Work, i.Line, i.RowColor
                        FROM InOuts i
                        JOIN Workers w ON i.WN = w.WN
                        WHERE (w.Name LIKE @key OR i.WN LIKE @key)";

                    if (!string.IsNullOrEmpty(selectedDate))
                    {
                        qry += " AND i.Date = @date";
                    }

                    using (var cmd = new SQLiteCommand(qry, conn))
                    {
                        cmd.Parameters.AddWithValue("@key", "%" + keyword + "%");
                        cmd.Parameters.AddWithValue("@date", selectedDate);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                inoutList.Add(new InOut
                                {
                                    WN = reader["WN"].ToString(),
                                    Name = reader["Name"].ToString(),
                                    Date = DateTime.Parse(reader["Date"].ToString()),
                                    In_Work = reader["In_Work"].ToString(),
                                    Out_Work = reader["Out_Work"].ToString(),
                                    Line = reader["Line"].ToString(),
                                    RowColor = string.IsNullOrEmpty(reader["RowColor"].ToString()) ? "White" : reader["RowColor"].ToString()
                                });
                            }
                        }
                    }
                }
                dgInOuts.ItemsSource = inoutList;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải dữ liệu chấm công: " + ex.Message); }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadInOutData();
        private void dpFilterDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => LoadInOutData();

        private void btnAddInOut_Click(object sender, RoutedEventArgs e)
        {
            ChamCongPopup popup = new ChamCongPopup();
            if (popup.ShowDialog() == true)
            {
                LoadInOutData();
            }
        }

        private void btnDeleteInOut_Click(object sender, RoutedEventArgs e)
        {
            if (dgInOuts.SelectedItem is InOut selectedInOut)
            {
                var confirm = MessageBox.Show($"Xóa dữ liệu chấm công của {selectedInOut.Name} ngày {selectedInOut.Date:dd/MM/yyyy}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var conn = DatabaseHelper.GetConnection())
                        {
                            string qry = "DELETE FROM InOuts WHERE WN = @wn AND Date = @date";
                            using (var cmd = new SQLiteCommand(qry, conn))
                            {
                                cmd.Parameters.AddWithValue("@wn", selectedInOut.WN);
                                cmd.Parameters.AddWithValue("@date", selectedInOut.Date.ToString("yyyy-MM-dd"));
                                cmd.ExecuteNonQuery();
                            }
                        }
                        LoadInOutData();
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi khi xóa: " + ex.Message); }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn dòng dữ liệu cần xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void dgInOuts_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgInOuts.SelectedItem is InOut selectedInOut)
            {
                ChamCongPopup popup = new ChamCongPopup(selectedInOut);
                if (popup.ShowDialog() == true)
                {
                    LoadInOutData();
                }
            }
        }
    }
}
