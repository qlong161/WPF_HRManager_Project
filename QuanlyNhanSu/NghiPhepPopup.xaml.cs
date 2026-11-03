using System;
using System.Data.SQLite;
using System.Windows;

namespace QuanlyNhanSu
{
    public partial class NghiPhepPopup : Window
    {
        public NghiPhepPopup()
        {
            InitializeComponent();
            LoadWorkersToComboBox();
            txtBreakStart.Text = DateTime.Now.ToString("dd/MM/yyyy 08:00");
            txtBreakEnd.Text = DateTime.Now.ToString("dd/MM/yyyy 17:00");
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
                if (cbWorker.Items.Count > 0) cbWorker.SelectedIndex = 0;
            }
            catch { }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbWorker.SelectedItem == null || string.IsNullOrWhiteSpace(txtBreakStart.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin nhân viên và thời gian nghỉ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string wn = cbWorker.SelectedItem.ToString().Split(' ')[0];
            string startStr = txtBreakStart.Text.Trim();
            string endStr = txtBreakEnd.Text.Trim();
            double duration = double.TryParse(txtDuration.Text, out double d) ? d : 8.0;
            string note = txtNote.Text.Trim();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string qry = @"
                        INSERT INTO Breaks (WN, BreakStart, BreakEnd, Duration, Allowed, TimeLeft, Note) 
                        VALUES (@wn, @start, @end, @duration, @allowed, @timeLeft, @note)";

                    using (var cmd = new SQLiteCommand(qry, conn))
                    {
                        cmd.Parameters.AddWithValue("@wn", wn);
                        cmd.Parameters.AddWithValue("@start", startStr);
                        cmd.Parameters.AddWithValue("@end", endStr);
                        cmd.Parameters.AddWithValue("@duration", duration);
                        cmd.Parameters.AddWithValue("@allowed", 8.0); 
                        cmd.Parameters.AddWithValue("@timeLeft", 0);
                        cmd.Parameters.AddWithValue("@note", note);
                        cmd.ExecuteNonQuery();
                    }
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu đơn nghỉ: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
