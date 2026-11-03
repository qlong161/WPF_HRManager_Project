using System.Data;
using System.Data.SQLite;
using System.IO;

namespace QuanlyNhanSu
{
    public static class DatabaseHelper
    {
        private const string ConnectionString = "Data Source=EmployeeManager.db;Version=3;";

        public static void InitializeDatabase()
        {
            if (!File.Exists("EmployeeManager.db"))
            {
                SQLiteConnection.CreateFile("EmployeeManager.db");
            }

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string initQuery = @"
                    -- 1. Bảng Tài khoản hệ thống (Users)
                    CREATE TABLE IF NOT EXISTS Users (
                        Username TEXT PRIMARY KEY,
                        Password TEXT NOT NULL,
                        FullName TEXT,
                        Role TEXT
                    );

                    -- 2. Bảng Danh mục Chức vụ (Roles - Master Data)
                    CREATE TABLE IF NOT EXISTS Roles (
                        RoleName TEXT PRIMARY KEY
                    );

                    -- 3. Bảng Nhân sự Gốc (Workers) - Giữ toàn bộ thông tin hành chính
                    CREATE TABLE IF NOT EXISTS Workers (
                        WN TEXT PRIMARY KEY,
                        Name TEXT NOT NULL,
                        Department TEXT,
                        Shift TEXT,
                        Level INTEGER,
                        StartDate TEXT,
                        TotalYears TEXT,
                        Status TEXT DEFAULT 'Active'
                    );

                    -- 4. Bảng Lực lượng lao động / Kỹ năng (ManPower)
                    CREATE TABLE IF NOT EXISTS ManPower (
                        WN TEXT PRIMARY KEY,
                        RoleType TEXT,
                        MainRole TEXT,
                        BackUpRole TEXT,
                        BackUpRole2 TEXT,
                        NextRole TEXT,
                        RoleCapacity INTEGER DEFAULT 0,
                        BackUpCapacity INTEGER DEFAULT 0,
                        NextCapacity INTEGER DEFAULT 0,
                        FOREIGN KEY(WN) REFERENCES Workers(WN) ON DELETE CASCADE
                    );

                    -- 5. Bảng Ca làm việc (WaveShifts)
                    CREATE TABLE IF NOT EXISTS WaveShifts (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        WN TEXT,
                        Date TEXT,
                        Shift_Duration INTEGER,
                        Break_Duration INTEGER,
                        FOREIGN KEY(WN) REFERENCES Workers(WN) ON DELETE CASCADE
                    );

                    -- 6. Bảng Quản lý nghỉ phép (Breaks)
                    CREATE TABLE IF NOT EXISTS Breaks (
                        Break_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        WN TEXT,
                        BreakStart_Full TEXT,
                        BreakEnd_Full TEXT,
                        BreakStart TEXT,
                        BreakEnd TEXT,
                        Duration REAL,
                        Allowed REAL,
                        TimeLeft REAL,
                        LateLessFive INTEGER,
                        LateMoreFive INTEGER,
                        AllLates INTEGER,
                        Note TEXT,
                        DeletedBy TEXT,
                        RowColor TEXT,
                        FOREIGN KEY(WN) REFERENCES Workers(WN) ON DELETE CASCADE
                    );

                    -- 7. Bảng Vào/Ra Chấm công (InOuts)
                    CREATE TABLE IF NOT EXISTS InOuts (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT,
                        WN TEXT,
                        In_Work TEXT,
                        Out_Work TEXT,
                        Line TEXT,
                        In_Line TEXT,
                        Out_Line TEXT,
                        RowColor TEXT,
                        FOREIGN KEY(WN) REFERENCES Workers(WN) ON DELETE CASCADE
                    );
                ";

                using (var cmdPragma = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
                {
                    cmdPragma.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(initQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static SQLiteConnection GetConnection()
        {
            var conn = new SQLiteConnection(ConnectionString);
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("PRAGMA foreign_keys = ON;", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            return conn;
        }
    }
}
