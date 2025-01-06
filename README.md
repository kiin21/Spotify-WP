# Spotify-WP Project

## Mục lục

- [Spotify-WP Project](#spotify-wp-project)
  - [Mục lục](#mục-lục)
  - [Thành viên nhóm](#thành-viên-nhóm)
  - [Milestone 1](#milestone-1)
    - [Các tính năng chính hoàn thành milestone 1](#các-tính-năng-chính-hoàn-thành-milestone-1)
    - [Cấu trúc project](#cấu-trúc-project)
    - [Mô tả các chức năng milestone 1](#mô-tả-các-chức-năng-milestone-1)
      - [1. **22120120 - Đặng Phúc Hưng**](#1-22120120---đặng-phúc-hưng)
      - [2. **22120157 - Nguyễn Nam Khánh**](#2-22120157---nguyễn-nam-khánh)
      - [3. **22120163 - Phạm Đào Anh Khoa**](#3-22120163---phạm-đào-anh-khoa)
    - [Advanced Topics Milestone 1](#advanced-topics-milestone-1)
    - [Phương pháp đảm bảo chất lượng milestone 1](#phương-pháp-đảm-bảo-chất-lượng-milestone-1)
    - [Giao diện khi hoàn thành milestone 1](#giao-diện-khi-hoàn-thành-milestone-1)
    - [Demo milestone 1](#demo-milestone-1)
  - [Milestone 2](#milestone-2)
    - [Các tính năng chính đã hoàn thành milestone 2](#các-tính-năng-chính-đã-hoàn-thành-milestone-2)
    - [Mô tả các chức năng milestone 2](#mô-tả-các-chức-năng-milestone-2)
      - [1. **22120120 - Đặng Phúc Hưng**](#1-22120120---đặng-phúc-hưng-1)
      - [2. **22120157 - Nguyễn Nam Khánh**](#2-22120157---nguyễn-nam-khánh-1)
      - [3. **22120163 - Phạm Đào Anh Khoa**](#3-22120163---phạm-đào-anh-khoa-1)
    - [Advanced Topics Milestone 2](#advanced-topics-milestone-2)
    - [Phương pháp đảm bảo chất lượng milestone 2](#phương-pháp-đảm-bảo-chất-lượng-milestone-2)
    - [Giao diện khi hoàn thành milestone 2](#giao-diện-khi-hoàn-thành-milestone-2)
    - [Demo milestone 2](#demo-milestone-2)
    - [Document milestone 2](#document-milestone-2)
  - [Milestone 3](#milestone-3)
    - [Các tính năng chính đã hoàn thành milestone 3](#các-tính-năng-chính-đã-hoàn-thành-milestone-3)
    - [Mô tả các chức năng milestone 3](#mô-tả-các-chức-năng-milestone-3)
      - [1. **22120120 - Đặng Phúc Hưng**](#1-22120120---đặng-phúc-hưng-2)
      - [2. **22120157 - Nguyễn Nam Khánh**](#2-22120157---nguyễn-nam-khánh-2)
      - [3. **22120163 - Phạm Đào Anh Khoa**](#3-22120163---phạm-đào-anh-khoa-2)
    - [Advanced Topics Milestone 3](#advanced-topics-milestone-3)
    - [Phương pháp đảm bảo chất lượng milestone 3](#phương-pháp-đảm-bảo-chất-lượng-milestone-3)
    - [Giao diện khi hoàn thành milestone 3](#giao-diện-khi-hoàn-thành-milestone-3)
    - [Document milestone 3](#document-milestone-3)
  - [Link project và các demo](#link-project-và-các-demo)

## Thành viên nhóm

- **22120120**: Đặng Phúc Hưng
- **22120157**: Nguyễn Nam Khánh
- **22120163**: Phạm Đào Anh Khoa

---

## Milestone 1

### Các tính năng chính hoàn thành milestone 1

- **Thiết kế giao diện bằng Figma**: [Link](https://www.figma.com/design/KQ9QM9HPORYoEXSJpGvn65/LTWin?node-id=0-1&t=a4tj18mjb5halaHx-1) - 1 giờ
- **Thiết kế database và diagram**: [Link](https://app.diagrams.net/#G1t48u7IyroU-gqegTtjc5Kz67LG8n8B9M#%7B%22pageId%22%3A%22WQx1MMiO1pLtJ0Js-RWY%22%7D) - 1 giờ
- **Tạo playlist** - 1 giờ
- **Tạo hàng chờ (Queue)** - 1 giờ
- **Tùy chỉnh tốc độ phát nhạc** - 1 giờ
- **Tìm kiếm bài hát theo tên** - 1 giờ

**Links**:

- [Demo trên YouTube](https://www.youtube.com/watch?v=mV57ojbW5mU)
- [Repository trên GitHub](https://github.com/kiin21/Spotify-WP)

---

### Cấu trúc project

Project sử dụng mô hình **MVVM**, với các thành phần chính:

- **Models**

  - Định nghĩa cấu trúc dữ liệu cơ bản của ứng dụng.

- **ViewModels**

  - Đóng vai trò trung gian giữa View và Model.
  - Xử lý logic hiển thị và giao tiếp với người dùng.

- **Views**

  - Chứa các file `.xaml` định nghĩa giao diện người dùng.
  - Đảm nhiệm xử lý UI và bắt sự kiện cơ bản.

- **Các thư mục hỗ trợ**

  - **Contracts/DAO**: Interface định nghĩa các phương thức truy xuất dữ liệu.
  - **DAOs**: Hiện thực các interface từ Contracts/DAO.
  - **Helpers**: Chứa utility classes và methods dùng chung.
  - **Converters**: Chuyển đổi dữ liệu giữa các dạng khác nhau.
  - **Services**: Cung cấp các chức năng chính của ứng dụng.

- **Entry Point**

  - **App.xaml & App.xaml.cs**: Khởi tạo resources, dependency injection, cấu hình routing và navigation.
  - **ShellWindow.xaml & ShellWindow.xaml.cs**: Layout chính (menu, navigation, status bar).

---

### Mô tả các chức năng milestone 1

Đầu tiên sẽ tạo cấu trúc thư mục giống như ở phần trên và dựng layout cơ bản như hình: 
[![s21130211062024](https://camo.githubusercontent.com/9ed265685946fc464709446815e30c1d1372c666da805d165328c3a858331adb/68747470733a2f2f612e6f6b6d642e6465762f6d642f363732623739373065666532312e706e67)](https://camo.githubusercontent.com/9ed265685946fc464709446815e30c1d1372c666da805d165328c3a858331adb/68747470733a2f2f612e6f6b6d642e6465762f6d642f363732623739373065666532312e706e67)
Đến đây sẽ phân công công việc
**Hưng:** Làm phần trình phát nhạc (thanh dưới cùng) và phần queue 
**Khánh:** Tạo playlist ở phần main bên trái, và xử lý sự kiện khi click 
**Khoa:** Làm phần search bài hát và giao diện chính hiển thị bài hát

#### 1. **22120120 - Đặng Phúc Hưng**

- **Trình phát nhạc**:

  - Chức năng: Play, pause, next, previous, shuffle, loop, chỉnh tốc độ, volume, thời gian.
  - ![Giao diện trình phát nhạc](https://a.okmd.dev/md/672b765baeea5.png)

- **Hàng chờ (Queue)**:

  - Quản lý và thêm bài hát vào hàng chờ.
  - ![Giao diện hàng chờ](https://a.okmd.dev/md/672b768a38f7d.png)

---

#### 2. **22120157 - Nguyễn Nam Khánh**

- **Playlist**:

  - Tạo và xóa playlist.
  - ![Giao diện tạo playlist](https://a.okmd.dev/md/672b77898b661.png)
  - ![Xóa playlist](https://a.okmd.dev/md/672b78ebc77d4.png)

- **Xử lý sự kiện click bài hát**:

  - Hiển thị thông tin chi tiết bài hát từ database.
  - ![Hiển thị dữ liệu bài hát](https://a.okmd.dev/md/672b774890f42.png)

---

#### 3. **22120163 - Phạm Đào Anh Khoa**

- **Chức năng đăng nhập/đăng ký**:

  - ![Đăng nhập](https://a.okmd.dev/md/672b780771140.png)
  - ![Đăng ký](https://a.okmd.dev/md/672b782ac7778.png)

- **Danh sách và tìm kiếm bài hát**:

  - Hiển thị danh sách bài hát, tìm kiếm theo tên bài hát.
  - ![Danh sách bài hát](https://a.okmd.dev/md/672b78cce5dba.png)
  - ![Tìm kiếm bài hát](https://a.okmd.dev/md/672b788c084fd.png)

- **Hiển thị lời bài hát**:

  - ![Hiển thị lời bài hát](https://a.okmd.dev/md/672b78559cae7.png)

---

### Advanced Topics Milestone 1

1. **Sử dụng ORM để map dữ liệu**: Map các trường trong object với các thuộc tính trong database MongoDB.![ORM 1](https://a.okmd.dev/md/67584d1505b28.png)![ORM 2](https://a.okmd.dev/md/67584d2f148b3.png)
2. **Đăng ký và lắng nghe sự kiện**:

   - Định nghĩa sự kiện:![Định nghĩa sự kiện](https://a.okmd.dev/md/67584d3e4a7e4.png)![Đăng ký sự kiện](https://a.okmd.dev/md/67584d46c1e7d.png)
   - Trigger sự kiện:![Trigger sự kiện](https://a.okmd.dev/md/67584d51aee5f.png)
   - Ví dụ: Đăng ký sự kiện khi bài hát ở PlaybackControl thay đổi.
     ![PlaybackControl](https://a.okmd.dev/md/67584d5d7bef2.png)

3. **Định nghĩa style và sử dụng converter để binding**:

   - Binding `IsChecked` với màu nền của toggle button.
     ![Style và converter 1](https://a.okmd.dev/md/67584d69ace37.png)
     ![Style và converter 2](https://a.okmd.dev/md/67584d787de17.png)

4. **Dependency Injection (DI) và Singleton Pattern**:

   - Đăng ký dịch vụ duy nhất cho các Services.
     ![DI 1](https://a.okmd.dev/md/67584d84ac8f5.png)
     ![DI 2](https://a.okmd.dev/md/67584d8cd503e.png)

5. **Sử dụng DotNetEnv để lưu thông tin nhạy cảm**:![DotNetEnv 1](https://a.okmd.dev/md/67584d954fa66.png)![DotNetEnv 2](https://a.okmd.dev/md/67584d9de97bd.png)
6. **Gửi và nhận dữ liệu giữa các trang**:

   - Sử dụng hàm `OnNavigatedTo` để nhận dữ liệu.
     ![OnNavigatedTo 1](https://a.okmd.dev/md/67584da86ee7d.png)
     ![OnNavigatedTo 2](https://a.okmd.dev/md/67584db3988bc.png)

7. **Sử dụng NAudio**: Điều chỉnh tốc độ phát nhạc.![NAudio](https://a.okmd.dev/md/67584dbcdfe17.png)
8. **Hiển thị mô tả chức năng bằng ToolTipService**:![ToolTipService](https://a.okmd.dev/md/67584dde1da12.png)
9. **Sử dụng Command thay vì Click trong giao diện**:

   - Tách biệt logic và giao diện.
     ![Command](https://a.okmd.dev/md/67584dec2c7f1.png)

---

### Phương pháp đảm bảo chất lượng milestone 1

- **Kiểm thử thủ công**

### Giao diện khi hoàn thành milestone 1

![Giao diện hoàn chỉnh](https://a.okmd.dev/md/672b79a673ee6.png)

### Demo milestone 1

- [Demo trên YouTube](https://www.youtube.com/watch?v=mV57ojbW5mU)

---

## Milestone 2

### Các tính năng chính đã hoàn thành milestone 2

- **Hiển thị lời bài hát trong lúc phát theo thời gian thực** - 2 giờ - Khoa
- **Lịch sử phát của người dùng** - 2 giờ - Khoa
- **Xem trang ca sĩ** - 1 giờ - Khánh
- **Thông báo khi nghệ sĩ được theo dõi có bài hát mới** - 1 giờ - Khánh
- **Chia sẻ playlist với các user khác** - 2 giờ - Khánh
- **Hiển thị quảng cáo** - 2 giờ - Hưng
- **Đăng ký, thanh toán tài khoản premium để loại bỏ quảng cáo** - 2 giờ - Hưng
- **TỔNG SỐ GIỜ LÀM VIỆC: 12h**
- **Links**:
- [Demo mileston 2 trên YouTube](https://www.youtube.com/watch?v=mV57ojbW5mU)
- [Repository trên GitHub](https://github.com/kiin21/Spotify-WP)

---

### Mô tả các chức năng milestone 2

#### 1. **22120120 - Đặng Phúc Hưng**

- **Hiển thị quảng cáo**:

  - Hiển thị quảng cáo sau khi nghe 3 bài hát, với điều kiện là mỗi bài hát phải nghe được hơn 1 nửa bài. Quảng cáo không thể tua, bắt buộc người dùng phải nghe hết.
    ![s15372612112024](https://a.okmd.dev/md/67594f47cd5c2.png)

    ![s15262912112024](https://a.okmd.dev/md/67594cb77b986.png)

- **Đăng ký, thanh toán tài khoản premium để loại bỏ quảng cáo**:

  - Nếu muốn loại bỏ quảng cáo thì tiến hành mua gói premium. Tiến hành chọn gói cần mua
    ![s14092712112024](https://a.okmd.dev/md/67593aa9c618a.png)
    ![s15301212112024](https://a.okmd.dev/md/67594d961c50c.png)
  - Sau khi chọn Proceed thì điền thông tin thanh toán vào form bên dưới
    ![s14102912112024](https://a.okmd.dev/md/67593ae67fd41.png)
    ![s15343212112024](https://a.okmd.dev/md/67594e9a31e39.png)
  - Sau khi ấn Pay thì sẽ thanh toán thành công và user sẽ trở thành premium, có thể nghe nhạc không quảng cáo
    (thêm ảnh)
  - Ở đây chỉ làm xác nhận thanh toán thành công luôn, còn việc liên kết qua các cổng thanh toán thì chưa thực hiện được, nếu còn thời gian sẽ tìm hiểu thêm.

---

#### 2. **22120157 - Nguyễn Nam Khánh**

- **Trang ca sĩ**

  - Khi click vào tên nghệ sĩ ở bài hát thuộc các playlist thì sẽ link đến trang nghệ sĩ đó

  ![s22332612102024](https://a.okmd.dev/md/67585f49639e7.png)
  ![s22230312102024](https://a.okmd.dev/md/67585cda97f14.png)

  - Trang chi tiết nghệ sĩ sẽ chứa danh sách các bài hát của nghệ sĩ đó và khi click vào bài hát thì sẽ link đến chi tiết bài hát đó
  - Ở trên còn có nút Follow, khi click vào thì tức là user hiện tại đã follow nghệ sĩ đó (áp dụng cho chức năng thông báo ở sau)

  ![s22313812102024](https://a.okmd.dev/md/67585ede06a10.png)

- **Thông báo khi nghệ sĩ theo dõi ra bài hát mới**

  - Làm nút thông báo ban đầu nếu nghệ sĩ user follow chưa ra bài hát mới thì hiển thị không có gì mới
    ![s22362312102024](https://a.okmd.dev/md/67585ffaa0502.png)
  - Nếu nghệ sĩ ra bài hát mới thì sẽ phải cập nhật vào thông báo (ở đây thêm trực tiếp vào song_ids của collection Artist trong database vì khảo sát các ứng dụng thì họ không làm hỗ trợ thêm qua form trên UI)
    ![s23314912102024](https://a.okmd.dev/md/67586cf93d594.png)
  - Click vào logo để load lại thì sẽ thấy icon thông báo có dấu Red Dot biểu thị có thông báo mới. Và khi click vào thông báo thì sẽ hiển thị chi tiết
    ![s23350512102024](https://a.okmd.dev/md/67586dbcdd8ae.png)
    ![s23370812102024](https://a.okmd.dev/md/67586e36ceeab.png)

- **Chia sẻ danh sách playlist**

  - Khi click vào dấu "..." ở playlist thì sẽ popup lên MenuFlyout chứa các tính năng, click vào "Share playlist" thì sẽ hiện lên 1 dialog chứa thông tin các user (hiện tại hiển thị tất cả user, sau này có thể update lên chức năng kết bạn thì mới hiển thị user để chia sẻ).
    ![s23434512102024](https://a.okmd.dev/md/67586fc3d60d8.png) ![s23411412102024](https://a.okmd.dev/md/67586f2d3727e.png)
  - Sau khi click "Share" thì các account vừa được share sẽ nhận được playlist đó
    ![s23510512102024](https://a.okmd.dev/md/6758717c5726e.png)
  - Ví dụ ở đây user "test1" đã nhận được playlist user "admin" share
    ![s23522412102024](https://a.okmd.dev/md/675871cbd18e6.png)

- **Một số chức năng thêm**
  - Khi bấm vào nút "More options" ở từng bài hát trong playlist thì sẽ có lựa chọn xóa khỏi playlist hoặc thêm vào playlist yêu thích. Ví dụ khi bấm xóa "On My Way" khỏi playlist được mô tả như các hình dưới đây. Thêm vào playlist yêu thích cũng tương tự
    ![s10161812112024](https://a.okmd.dev/md/67590403c8e5d.png)
    ![s10165212112024](https://a.okmd.dev/md/67590424cb520.png)
    ![s10171612112024](https://a.okmd.dev/md/6759043d74afc.png)

---

#### 3. **22120163 - Phạm Đào Anh Khoa**

- **Hiển thị lời bài hát trong lúc phát theo thời gian thực**

  - Ở thanh công cụ phát nhạc, khi click vào biểu tượng có hình thanh nhạc ở dưới thì sẽ hiển thị lời bài hát
    ![s09293712112024](https://a.okmd.dev/md/6758f913122da.png)
    ![s09315912112024](https://a.okmd.dev/md/6758f9a04d010.png)
  - Khi click vào nút play, thì lời bài hát sẽ synced theo thời gian thực
    ![s09341812112024](https://a.okmd.dev/md/6758fa2c0cc82.png)

- **Lưu lịch sử phát của user**

  - Click vào avatar của user sẽ hiện lên các chức năng. Click vào "Recently played" sẽ hiển thị các bài hát mà user đã nghe trước đó
    ![s09421012112024](https://a.okmd.dev/md/6758fc034c949.png)
    ![s09590412112024](https://a.okmd.dev/md/6758fffa04fa7.png)
  - Khi click vào nghe một bài khác thì nó sẽ được lưu vào đây
    ![s10001112112024](https://a.okmd.dev/md/6759003c62113.png)
    ![s10005412112024](https://a.okmd.dev/md/675900670d171.png)

- **Một số chức năng thêm**
  - Thay đổi giữa việc hiển thị thông tin bài hát và hiển thị hàng chờ ở phần Right Side
    ![s10290712112024](https://a.okmd.dev/md/67590704940bf.png)
    ![s10292612112024](https://a.okmd.dev/md/675907179c7f1.png)
  - Thêm một bài hát vào queue ở trang home
    ![image](https://github.com/user-attachments/assets/4f997a68-ec6c-44c8-98f7-939f5332805a)

---

### Advanced Topics Milestone 2

1. **Lưu vào biến có chức năng như cache**:
   Làm thông báo nghệ sĩ ra bài hát mới bằng cách ban đầu lưu các bài hát của Artist mà User hiện tại đang follow vào 1 biến cache, sau đó mỗi lần Artist thêm bài hát mới thì sẽ so sánh những bài hát có trong database với những bài hát lưu vào cache trước đó, nếu khác cache thì lấy phần khác ra - đó là bài hát mới, đồng thời dùng ProperyChanged và ObservableCollection để lắng nghe sự thay đổi. Ở đây làm chưa hiệu quả lắm, đáng lẽ biến cache đó phải được lưu ở localStorage, sẽ sửa lại ở milestone sau

   ![s10443012112024](https://a.okmd.dev/md/67590a9f34454.png)

   ![s10450312112024](https://a.okmd.dev/md/67590ac0b948c.png)

   ![s10462412112024](https://a.okmd.dev/md/67590b1142c52.png)

   ![s10471712112024](https://a.okmd.dev/md/67590b46d53c3.png)

2. **Sử dụng ConcurrentDictionary**
   Để quản lý cache hiệu quả, an toàn trong môi trường đa luồng (nếu có làm đa luồng thì không cần dùng lệnh lock thủ công để đảm bảo tính toàn viện khi thực hiện các thao tác CRUD). ConcurrentDictionary hỗ trợ các phương thức TryGetValue, TryAdd,... mà k quan tâm việc xử lí luồng nếu sau này project của chúng ta có update lên để chạy nhiều luồng khác nhau. Ví dụ về việc xử lý đa luồng có thể sẽ gặp phải ở trường hợp: 1 luồng kiểm tra và cập nhật thông báo bài hát mới từ server (Task A), 1 luồng khác đọc cache để hiển thị danh sách bài hát cho người dùng (Task B). Nếu xử lý không đúng cách có thể dẫn tới race condition (dữ liệu đọc ghi không đồng bộ). Nhưng vấn đề này sẽ được xử lý sau nếu mở rộng thêm project
   ![s10483012112024](https://a.okmd.dev/md/67590b8fb4ffa.png)
3. **Truyền data động vào MenuFlyout**
   ![s10580212112024](https://a.okmd.dev/md/67590dcb32060.png)

   ![s10582712112024](https://a.okmd.dev/md/67590de45c27c.png)

4. Sử dụng Aggregation pipeline - một kiểu truy vấn SQL của non relational database

   ![image](https://github.com/user-attachments/assets/deb54bd2-8702-40fc-b3bb-b031592defe4)

   Đoạn mã trên thực hiện lọc record trong collection **\_playHistory** có user_id = userID, chuyển trường song_id của record sang string để tiện so sánh. "JOIN" với collection "Songs" với điều kiện localField=foreignField và đặt kết quả và field có tên là songDetails, $unwind để phân rã mảng (biến songDetails từ Array thành nested document), sau đó sắp xếp kết quả theo trường played_at và đổi lại kiểu của song_id thành string. Đây là một cú pháp xử lý theo tư duy **functional programming,** giúp chia nhỏ quá trình xử lý thành các stages.

---

### Phương pháp đảm bảo chất lượng milestone 2

Kiểm thử thủ công

[Tài liệu kiểm thử](https://docs.google.com/document/d/1Hlu2Kc0C9AjmgavFr9NYYOiqLHUydJSfvBMsp2q0b-Q/edit?usp=sharing)

### Giao diện khi hoàn thành milestone 2

![s11050312112024](https://a.okmd.dev/md/67590f711b3f5.png)

### Demo milestone 2

- [Demo trên YouTube](https://youtu.be/l39P_xqKiDE)
- [Demo du phong](https://drive.google.com/file/d/19MjbKHEmWo7_i0g_mRdVU5avVvAmoBUO/view?usp=sharing)

### Document milestone 2

- file index.html trong thư mục doxygen


## Milestone 3
### Các tính năng chính đã hoàn thành milestone 3
- **Thống kê dựa theo lịch sử phát nhạc** - 2 giờ - Khoa
- **Cho phép comment bài hát** - 2 giờ - Khánh
- **Ngoài ra còn bổ sung thêm một số việc để hoàn thiện milestone cũng như project**
	- **Sửa tính năng thanh toán** - 1 giờ - Khánh 
	- **Bổ sung dữ liệu vào cơ sở dữ liệu** - 1 giờ - Khoa
	- **Tài liệu kiểm thử** - 1 giờ - Khoa
	- **Báo cáo (readme)** - 1 giờ - Khánh
- **TỔNG SỐ GIỜ LÀM VIỆC: 8h**
### Mô tả các chức năng milestone 3
#### 1. **22120157 - Nguyễn Nam Khánh**
- **Thêm comment vào bài hát**

   - Khi click vào chi tiết 1 bài hát, kéo xuống phía dưới sẽ thấy những comment cho bài hát đó
	  ![s09534101042025](https://a.okmd.dev/md/6778a2b7c90cf.png)
 - Ở dưới các comment sẽ có ô input để viết comment
	![s09563101042025](https://a.okmd.dev/md/6778a361e874c.png)
	
 - Khi gõ nội dung comment và ấn post thì sẽ hiển thị trực tiếp comment vừa gửi lên
		![s09582001042025](https://a.okmd.dev/md/6778a3ce307f1.png)
- **Một số chức năng thêm**

  - Khi search bài hát và bấm vào button "+" thì sẽ có lựa chọn thêm vào các playlist. Khi click vào thì bài hát sẽ được thêm vào playlist tương ứng
	  ![s10120712112024](https://a.okmd.dev/md/67590308e025f.png)
	  ![s10125412112024](https://a.okmd.dev/md/675903379bc3d.png)
#### 2. **22120163 - Phạm Đào Anh Khoa**
- **Thống kê dựa theo lịch sử phát nhạc**
	- Thống kê số bài hát đã nghe theo ngày
 		![image](https://github.com/user-attachments/assets/d8cd5680-618c-4c86-8e1d-d32c53582e6a)

	- Thống kê số bài hát đã nghe theo buổi: sáng, chiều, tối
		![image](https://github.com/user-attachments/assets/0d85ae82-d513-4e75-abc8-e5a657953701)

	- Thống kê theo thể loại nhac và số phút nghe
		![image](https://github.com/user-attachments/assets/3cd888e6-a47c-4058-b727-7f40c6ee7c44)

- **Một số chức năng thêm**
  - Khi click vào nút Play ở playlist thì sẽ thêm các bài hát trong playlist đó vào hàng chờ phát nhạc và tiến hành phát bài nhạc đầu tiên
	  ![s10225812112024](https://a.okmd.dev/md/675905939ac5b.png)
	  ![s10241112112024](https://a.okmd.dev/md/675905dd05fbe.png)
	
### Advanced Topics Milestone 3
### Phương pháp đảm bảo chất lượng milestone 3
Kiểm thử thủ công
### Giao diện khi hoàn thành milestone 3
![s13281701042025](https://a.okmd.dev/md/6778d5049f3a1.png)

![s13291801042025](https://a.okmd.dev/md/6778d5411f777.png)

![s13290301042025](https://a.okmd.dev/md/6778d5322a2ba.png)

![s13302601042025](https://a.okmd.dev/md/6778d5846f985.png)

![s13304401042025](https://a.okmd.dev/md/6778d59682f52.png)

![s13340901042025](https://a.okmd.dev/md/6778d6645aadb.png)

![s13353601042025](https://a.okmd.dev/md/6778d6bb29bb5.png)
### Document milestone 3
file index.html trong thư mục doxygen
mô tả csdl: [https://drive.google.com/file/d/1t48u7IyroU-gqegTtjc5Kz67LG8n8B9M/view](link)
## Link project và các demo

- [Link demo milstone 1 youtube](https://www.youtube.com/watch?v=mV57ojbW5mU)
- [Link demo milestone 2 youtube](https://youtu.be/l39P_xqKiDE)
- [Link github](https://github.com/kiin21/Spotify-WP)
- [Link Project Proposal](https://docs.google.com/document/d/1GUuwiBjEMCA0-htyOsCtCUVEv3k0X6tHqeOWFXf3WpI/edit?tab=t.0)
