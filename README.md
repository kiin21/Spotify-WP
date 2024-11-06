# Thành viên nhóm

- 22120120: Đặng Phúc Hưng

- 22120157: Nguyễn Nam Khánh

- 22120163: Phạm Đào Anh Khoa

  

# Các tính năng chính làm được milestone 1

- [Thiết kế giao diện bằng Figma](https://www.figma.com/design/KQ9QM9HPORYoEXSJpGvn65/LTWin?node-id=0-1&t=a4tj18mjb5halaHx-1) - 1 giờ
- [Vẽ diagram, thiết kế database bằng mongodb](https://app.diagrams.net/#G1t48u7IyroU-gqegTtjc5Kz67LG8n8B9M#%7B%22pageId%22%3A%22WQx1MMiO1pLtJ0Js-RWY%22%7D) - 1 giờ

- Tạo playlist - 1 giờ 

- Tạo hàng chờ - 1 giờ 

- Tùy chỉnh tốc độ phát nhạc - 1 giờ
- Tìm kiếm theo tên bài hát - 1 giờ 


- [Link demo youtube](https://www.youtube.com/watch?v=mV57ojbW5mU)
  

# Mô tả chi tiết quá trình làm
## Tạo cấu trúc project
Tạo cấu trúc project theo mô hình MVVM, với các folder chính
- Models:
	- Chứa các class model đại diện cho dữ liệu
	- Định nghĩa cấu trúc dữ liệu cơ bản của ứng dụng
- ViewModels
	- Chứa các class ViewModel, đóng vai trò trung gian giữa View và Model
	- Xử lý logic hiển thị và tương tác với người dùng
	- Chứa các Command để xử lý các action từ UI
- Views
	- Chứa các file .xaml định nghĩa giao diện người dùng
	- Chỉ chứa code liên quan đến UI, code xử lý bắt sự kiện
Ngoài ra còn có các folder hỗ trợ cho việc xử lý logic
- Contracts/DAO
	- Chứa các interface định nghĩa contract cho DAO (Data Access Object)
	- Định nghĩa các phương thức truy xuất dữ liệu
- Converter
	- Chứa các class converter để chuyển đổi dữ liệu
- DAOs
	- Chứa các class implement interface từ Contracts/DAO
	- Xử lý truy xuất dữ liệu trực tiếp từ database
- Helpers
	- Chứa các utility class và helper method
	- Các function dùng chung trong ứng dụng
- Services
	- Chứa các service class cung cấp chức năng cho ứng dụng
Có các file .xaml và .xaml.cs
- App.xaml & App.xaml.cs
	- Entry point của ứng dụng
	- Khởi tạo resources và dependency injection
	- Cấu hình routing và navigation
-  ShellWindow.xaml & ShellWindow.xaml.cs
	- Cửa sổ chính của ứng dụng
	- Định nghĩa layout chung (menu, navigation, status bar...)
	- Container cho các view khác
Các file khác
- .env
	- Chứa các biến môi trường 




## Quá trình làm của các thành viên

Đầu tiên sẽ tạo cấu trúc thư mục giống như ở phần trên và dựng layout cơ bản như hình:
![s21130211062024](https://a.okmd.dev/md/672b7970efe21.png)

Đến đây sẽ phân công công việc
Hưng: Làm phần trình phát nhạc (thanh dưới cùng) và phần queue
Khánh: Tạo playlist ở phần main bên trái, và xử lý sự kiện khi click 
Khoa: Làm phần search bài hát và giao diện chính hiển thị bài hát
### 22120120 - Đặng Phúc Hưng
- **Trình phát nhạc**: play, pause, next, previous, shuffle, loop, speed, volume, time
![s20595311062024](https://a.okmd.dev/md/672b765baeea5.png)

- **Tạo hàng chờ** và thêm các bài hát vào hàng chờ
![s21004011062024](https://a.okmd.dev/md/672b768a38f7d.png)

### 22120157 - Nguyễn Nam Khánh
- **Tạo playlist, xóa playlist**
![s21045411062024](https://a.okmd.dev/md/672b77898b661.png)
![s21104911062024](https://a.okmd.dev/md/672b78ebc77d4.png)
- **Hiển thị data của bài hát** khi click (có được bằng việc join giữa 2 bảng trong database)
![s21034911062024](https://a.okmd.dev/md/672b774890f42.png)


### 22120163 - Phạm Đào Anh Khoa
- **Đăng nhập, đăng ký**
![s21070011062024](https://a.okmd.dev/md/672b780771140.png)

![s21073611062024](https://a.okmd.dev/md/672b782ac7778.png)
- **Danh sách bài hát**
![s21101811062024](https://a.okmd.dev/md/672b78cce5dba.png)
- **Tìm kiếm bài hát**
![s21091311062024](https://a.okmd.dev/md/672b788c084fd.png)
- **Hiển thị lời bài hát**
![s21081911062024](https://a.okmd.dev/md/672b78559cae7.png)

### Giao diện khi hoàn thành
![s21135511062024](https://a.okmd.dev/md/672b79a673ee6.png)
