using System;
using System.Data.SQLite;
using System.Windows;

namespace QuanlyNhanSu
{
    public partial class PhanCaWindow : Window
    {
        public PhanCaWindow()
        {
            InitializeComponent();
            LoadWorkersToComboBox();
            dpDate.SelectedDate = DateTime.Today;
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
            if (cbWorker.SelectedItem == null || dpDate.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên và ngày làm việc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string selectedItem = cbWorker.SelectedItem.ToString();
            string wn = selectedItem.Split(' ')[0];

            string dateStr = dpDate.SelectedDate.Value.ToString("yyyy-MM-dd");
            int shiftDuration = int.TryParse(txtShiftDuration.Text, out int s) ? s : 8;
            int breakDuration = int.TryParse(txtBreakDuration.Text, out int b) ? b : 60;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string qry = "INSERT INTO WaveShifts (WN, Date, Shift_Duration, Break_Duration) VALUES (@wn, @date, @shiftDur, @breakDur)";
                    using (var cmd = new SQLiteCommand(qry, conn))
                    {
                        cmd.Parameters.AddWithValue("@wn", wn);
                        cmd.Parameters.AddWithValue("@date", dateStr);
                        cmd.Parameters.AddWithValue("@shiftDur", shiftDuration);
                        cmd.Parameters.AddWithValue("@breakDur", breakDuration);
                        cmd.ExecuteNonQuery();
                    }
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi phân ca (Có thể nhân viên này đã có ca trong ngày): " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
