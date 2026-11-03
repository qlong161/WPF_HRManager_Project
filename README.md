# 🏢 HR MANAGEMENT SYSTEM (WPF & SQLite)

<p align="center">
  <b>Hệ thống Quản lý Nhân sự hiện đại được xây dựng bằng WPF (C#) và cơ sở dữ liệu SQLite.</b>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Platform-Windows-blue?style=flat-square" alt="Platform">
  <img src=".github/badges/dotnet.svg" alt=".NET">
  <img src="https://img.shields.io/badge/Database-SQLite-003B57?style=flat-square&logo=sqlite&logoColor=white" alt="SQLite">
  <img src="https://img.shields.io/badge/License-MIT-green?style=flat-square" alt="License">
</p>

---

## 👨‍💻 Tác giả
* **Họ và tên:** Nguyễn Hữu Quang Long
* **Mã sinh viên:** 23010309
* **Họ và tên:** Trần Hoàng Mạnh
* **Mã sinh viên:** 23010309
* **Họ và tên:** Phạm Khắc Hùng
* **Mã sinh viên:** 23010309

---

## 🚀 Giới thiệu dự án
**HR Management System** là ứng dụng desktop quản lý nhân sự toàn diện, được thiết kế với giao diện hiện đại (Modern UI) dựa trên WPF và .NET. Hệ thống giúp tối ưu hóa các quy trình hành chính nhân sự trong doanh nghiệp như quản lý hồ sơ nhân viên, chấm công, quản lý ca làm việc, nghỉ phép và xuất báo cáo tự động.

---

## ✨ Tính năng chính
* **📊 Tổng quan (Dashboard):** Hiển thị thống kê nhanh số liệu nhân sự (Tổng nhân viên, đang làm việc, nghỉ phép, ca làm trong ngày) và danh sách hoạt động gần đây theo thời gian thực.
* **👥 Quản lý nhân sự:** Thêm, sửa, xóa, tìm kiếm và lọc hồ sơ nhân viên theo phòng ban với giao diện bảng dữ liệu (`DataGrid`) trực quan.
* **⏱️ Quản lý Chấm công (Vào/Ra):** Theo dõi và ghi nhận thời gian làm việc của nhân viên.
* **📅 Quản lý Ca làm việc & Nhân lực:** Phân công ca làm, quản lý lịch trình và cơ cấu nhân sự linh hoạt.
* **🏖️ Quản lý Nghỉ phép:** Xử lý các yêu cầu nghỉ phép, phê duyệt và theo dõi lịch sử nghỉ của nhân viên.
* **📈 Báo cáo & Thống kê:** Hỗ trợ xuất dữ liệu chuyên nghiệp ra file **Excel** (thông qua `ClosedXML`) hoặc **PDF** (thông qua `QuestPDF`).

---

## 🛠️ Công nghệ sử dụng
* **Ngôn ngữ:** C# (`.NET Core / .NET Framework`)
* **Giao diện:** WPF (Windows Presentation Foundation) với Custom Styles/Templates
* **Cơ sở dữ liệu:** SQLite (`System.Data.SQLite`)
---

## 📦 Cấu trúc cơ sở dữ liệu (SQLite)
Hệ thống vận hành với các bảng chính:
* `Workers`: Lưu trữ thông tin chi tiết nhân viên.
* `ManPower`: Quản lý thông tin nhân lực và chức vụ.
* `NghiPhep`: Quản lý lịch sử và trạng thái nghỉ phép.
* `WaveShifts`: Quản lý ca làm việc.
* `InOuts`: Dữ liệu chấm công vào/ra.
* `Roles` & `Departments`: Danh mục chức vụ và phòng ban hệ thống.

---

## ⚙️ Hướng dẫn cài đặt & Chạy ứng dụng
1. Clone repository này về máy:
   ```bash
   git clone [https://github.com/longq161/WPF_HRManager_Project.git](https://github.com/longq161/WPF_HRManager_Project.git)
