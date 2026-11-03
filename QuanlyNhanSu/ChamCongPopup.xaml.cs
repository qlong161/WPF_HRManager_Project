using System;
using System.Data.SQLite;
using System.Windows;
using QuanlyNhanSu.Classes;

namespace QuanlyNhanSu
{
    public partial class ChamCongPopup : Window
    {
        private InOut _editingRecord = null;

        public ChamCongPopup(InOut selectedRecord = null)
        {
            InitializeComponent();
            LoadWorkersToComboBox();

            _editingRecord = selectedRecord;

            if (_editingRecord != null)
            {
                dpDate.SelectedDate = _editingRecord.Date;
                txtChuThich.Text = _editingRecord.Line; 

                foreach (var item in cbWorker.Items)
                {
                    if (item.ToString().StartsWith(_editingRecord.WN + " -"))
                    {
                        cbWorker.SelectedItem = item;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(_editingRecord.In_Work) && string.IsNullOrEmpty(_editingRecord.Out_Work))
                {
                    radRa.IsChecked = true;
                    txtGio.Text = DateTime.Now.ToString("HH:mm");
                }
                else if (!string.IsNullOrEmpty(_editingRecord.Out_Work))
                {
                    radRa.IsChecked = true;
                    txtGio.Text = _editingRecord.Out_Work;
                }
                else
                {
                    radVao.IsChecked = true;
                    txtGio.Text = _editingRecord.In_Work;
                }
            }
            else
            {
                dpDate.SelectedDate = DateTime.Today;
                txtGio.Text = DateTime.Now.ToString("HH:mm");
            }
        }

        private void LoadWorkersToComboBox()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    using (var cmd = new SQLiteCommand("SELECT WN, Name FROM Workers", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbWorker.Items.Add($"{reader["WN"]} - {reader["Name"]}");
                        }
                    }
                }
                if (cbWorker.Items.Count > 0 && _editingRecord == null) cbWorker.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách NV: " + ex.Message);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbWorker.SelectedItem == null || dpDate.SelectedDate == null || string.IsNullOrWhiteSpace(txtGio.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin bắt buộc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string wn = cbWorker.SelectedItem.ToString().Split(' ')[0];
            string dateStr = dpDate.SelectedDate.Value.ToString("yyyy-MM-dd");
            string timeStr = txtGio.Text.Trim();
            string lineStr = txtChuThich.Text.Trim(); 

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    int count = 0;
                    using (var cmdCheck = new SQLiteCommand("SELECT COUNT(*) FROM InOuts WHERE WN = @wn AND Date = @date", conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@wn", wn);
                        cmdCheck.Parameters.AddWithValue("@date", dateStr);
                        count = Convert.ToInt32(cmdCheck.ExecuteScalar());
                    }

                    if (radVao.IsChecked == true)
                    {
                        if (count > 0)
                        {
                            string qry = "UPDATE InOuts SET In_Work = @time, Line = @line WHERE WN = @wn AND Date = @date";
                            using (var cmd = new SQLiteCommand(qry, conn))
                            {
                                cmd.Parameters.AddWithValue("@wn", wn);
                                cmd.Parameters.AddWithValue("@date", dateStr);
                                cmd.Parameters.AddWithValue("@time", timeStr);
                                cmd.Parameters.AddWithValue("@line", lineStr);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            string qry = "INSERT INTO InOuts (WN, Date, In_Work, Line) VALUES (@wn, @date, @time, @line)";
                            using (var cmd = new SQLiteCommand(qry, conn))
                            {
                                cmd.Parameters.AddWithValue("@wn", wn);
                                cmd.Parameters.AddWithValue("@date", dateStr);
                                cmd.Parameters.AddWithValue("@time", timeStr);
                                cmd.Parameters.AddWithValue("@line", lineStr);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        if (count > 0)
                        {
                            string qry = "UPDATE InOuts SET Out_Work = @time, Line = @line WHERE WN = @wn AND Date = @date";
                            using (var cmd = new SQLiteCommand(qry, conn))
                            {
                                cmd.Parameters.AddWithValue("@wn", wn);
                                cmd.Parameters.AddWithValue("@date", dateStr);
                                cmd.Parameters.AddWithValue("@time", timeStr);
                                cmd.Parameters.AddWithValue("@line", lineStr);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Chưa có giờ Check-in cho ngày này để Check-out!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu chấm công: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
