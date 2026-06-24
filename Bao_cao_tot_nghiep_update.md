BỘ GIÁO DỤC VÀ ĐÀO TẠO
TRƯỜNG ĐẠI HỌC THỦY LỢI

**NGUYỄN ĐỨC CHIẾN**

**XÂY DỰNG TRÒ CHƠI NHẬP VAI HÀNH ĐỘNG 2D PHONG CÁCH ROGUELIKE DÙNG UNITY ENGINE VÀ NGÔN NGỮ C#**

**ĐỒ ÁN TỐT NGHIỆP**

HÀ NỘI, NĂM 2026

---

**LỜI CAM ĐOAN**
Em xin cam đoan đây là Đồ án tốt nghiệp của bản thân em. Các kết quả trong Đồ án tốt nghiệp này là trung thực, và không sao chép từ bất kỳ một nguồn nào và dưới bất kỳ hình thức nào. Việc tham khảo các nguồn tài liệu (nếu có) đã được thực hiện trích dẫn và ghi nguồn tài liệu tham khảo đúng quy định. Em hoàn toàn chịu trách nhiệm về nội dung của lời cam đoan trên.

Hà Nội, ngày 29 tháng 6 năm 2026
Sinh viên thực hiện: Nguyễn Đức Chiến

---

**LỜI CÁM ƠN**
Trước hết, em xin gửi lời tri ân sâu sắc đến TS. Cù Việt Dũng, người đã tận tình hướng dẫn, hỗ trợ em trong suốt quá trình nghiên cứu và thực hiện đề tài này. Bên cạnh đó, em cũng xin bày tỏ lòng biết ơn chân thành đến quý Thầy Cô Khoa Công nghệ thông tin, Trường Đại học Thủy lợi. Đồng thời, em cũng muốn gửi lời cảm ơn đến gia đình và bạn bè – những người đã luôn bên cạnh động viên và hỗ trợ em trong suốt quá trình học tập và hoàn thiện dự án.

Hà Nội, ngày 29 tháng 6 năm 2026
Sinh viên thực hiện: Nguyễn Đức Chiến

---

### MỤC LỤC
**CHƯƠNG 1: TỔNG QUAN ĐỀ TÀI**
1.1 Lý do chọn đề tài
1.2 Giới thiệu đề tài và mục tiêu nghiên cứu
1.3 Cơ sở khoa học và tính thực tiễn của đề tài
1.4 Mô tả ý tưởng và vòng lặp trải nghiệm cốt lõi (Core Loop)
1.5 Phạm vi và đối tượng người chơi hướng đến

**CHƯƠNG 2: CƠ SỞ LÝ THUYẾT VÀ THIẾT KẾ HỆ THỐNG**
2.1 Công nghệ nền tảng sử dụng
2.2 Kiến trúc và Các mẫu thiết kế phần mềm ứng dụng (Design Patterns)
2.3 Thuật toán tìm đường A* (A-Star Pathfinding)
2.4 Phân tích và Đặc tả yêu cầu hệ thống

**CHƯƠNG 3: CÀI ĐẶT CHI TIẾT, GIẢI PHÁP KỸ THUẬT VÀ THỰC NGHIỆM**
3.1 Môi trường phát triển và Cấu trúc dự án
3.2 Cài đặt chi tiết mã nguồn các lớp cốt lõi
3.3 Hiện thực hóa các giải pháp nâng cao và tối ưu hiệu năng
3.4 Kiểm thử hệ thống và Đánh giá hiệu năng
3.5 Kết luận và Hướng phát triển

---

## CHƯƠNG 1: TỔNG QUAN ĐỀ TÀI

**1.1 Lý do chọn đề tài**
Ngành công nghiệp trò chơi điện tử hiện đại không chỉ mang tính chất giải trí đơn thuần mà còn đòi hỏi người chơi có tư duy chiến thuật, quản lý tài nguyên và phản xạ linh hoạt. Dòng game nhập vai hành động (Action RPG) kết hợp yếu tố Roguelike đang trở thành xu hướng nổi bật với giá trị chơi lại (replayability) cực cao. Việc nghiên cứu và phát triển game bằng Unity Engine kết hợp ngôn ngữ C# là cơ hội để sinh viên tiếp cận quy trình kỹ thuật phần mềm thực tế, tối ưu hóa hệ thống AI tìm đường, quản lý bộ nhớ và ứng dụng các cấu trúc dữ liệu linh hoạt.

**1.2 Giới thiệu đề tài và mục tiêu nghiên cứu**
Đề tài tập trung xây dựng một trò chơi hành động nhập vai 2D (Top-down) trên nền tảng PC (Windows). 
Các phân hệ chức năng trọng tâm bao gồm:
- **Cơ chế chiến đấu nhịp độ cao:** Tích hợp hệ thống lướt (Dash) tiêu hao năng lượng (Energy), tấn công định hướng và né tránh.
- **Hệ thống Trí tuệ nhân tạo (Enemy AI):** Ứng dụng thuật toán tìm đường A* (A-Star) để quái vật có khả năng di chuyển né vật cản kết hợp cùng Máy trạng thái hữu hạn (FSM).
- **Hệ thống Cửa hàng và Kỹ năng thông qua NPC:** Cây kỹ năng (Skill Tree) và cửa hàng (Shop) được vận hành thông qua các NPC tương tác (ShopKeeper, SkillKeeper).
- **Hệ thống quản lý dữ liệu tĩnh (Scriptable Object):** Tối ưu hóa dữ liệu vật phẩm, chỉ số quái vật và cấu hình màn chơi.

**1.3 Cơ sở khoa học và tính thực tiễn của đề tài**
Đề tài ứng dụng các nguyên lý khoa học máy tính:
- **Thuật toán tìm đường A*:** Tính toán quỹ đạo di chuyển tối ưu nhất cho các thực thể AI.
- **Design Patterns:** Áp dụng Singleton Pattern (cho GameManager, RunManager) và Observer Pattern (Xử lý sự kiện giao diện).
- **Object-Oriented Data Management:** Dùng `ScriptableObject` để tách biệt cấu hình tĩnh (ItemSO, EnemyData) khỏi logic thực thi.

**1.4 Vòng lặp trải nghiệm cốt lõi (Core Gameplay Loop)**
1. **Sảnh chính (Main Hub):** Tương tác với NPC để mở Cây kỹ năng (Skill Tree), nâng cấp sức mạnh, mua sắm vật phẩm (ShopKeeper) và chọn màn chơi (LevelSelectKeeper).
2. **Khám phá và chiến đấu:** Di chuyển qua các khu vực, sử dụng kỹ năng Dash và vũ khí. Đối đầu với kẻ địch thông minh sử dụng thuật toán A*.
3. **Phát triển nhân vật:** Thu thập EXP để lên cấp, nhặt vật phẩm rơi rạc (Loot) và quản lý túi đồ (InventoryManager).
4. **Permadeath:** Nếu nhân vật chết, hành trình kết thúc. Các vật phẩm tạm thời bị mất, người chơi quay lại sảnh chính để dùng kinh nghiệm nâng cấp vĩnh viễn qua Skill Tree.

---

## CHƯƠNG 2: CƠ SỞ LÝ THUYẾT VÀ THIẾT KẾ HỆ THỐNG

**2.1 Công nghệ nền tảng sử dụng**
- **Unity 6 LTS:** Nền tảng phát triển với hệ sinh thái 2D mạnh mẽ (2D Physics, Rigidbody2D, Tilemap, Animation Events).
- **C# & IDE:** Sử dụng ngôn ngữ C# với mô hình lập trình hướng đối tượng. Công cụ IDE là Visual Studio/Rider.

**2.2 Kiến trúc và Các mẫu thiết kế phần mềm ứng dụng**
- **Singleton Pattern:** Đảm bảo duy nhất một thực thể quản lý toàn cục. Các lớp như `GameManager`, `RunManager`, `LevelManager`, và `AudioManager` được khởi tạo bằng Singleton kết hợp `DontDestroyOnLoad` để giữ dữ liệu liên tục qua các phân cảnh (Scene). Việc áp dụng cho `AudioManager` giúp xử lý âm thanh đồng bộ và không bị gián đoạn khi chuyển màn.
- **Máy trạng thái hữu hạn (FSM) bằng Enum:** Để tối ưu chi phí hiệu năng thay vì dùng State Pattern nguyên bản, AI quái vật sử dụng `Enum EnemyState` (Idle, Chasing, Retreating, Attacking, Knockback) kết hợp logic điều hướng rẽ nhánh trong C#.
- **Observer Pattern (Event/Delegate):** Xử lý giao diện và trạng thái sống/chết (Ví dụ: Sự kiện `SceneManager.sceneLoaded` hoặc cập nhật thanh máu UI khi nhận sát thương).

**2.3 Cơ sở lý thuyết Thuật toán tìm đường A* (A-Star Pathfinding)**
Trong quá trình phát triển trí tuệ nhân tạo (AI) cho quái vật (ví dụ: quái vật bắn xa), phương pháp dò đường hướng mục tiêu bằng tia vật lý (Raycast) thường gặp hạn chế lớn khi AI dễ bị kẹt vào các vật cản địa hình (góc tường, cột đá). Để khắc phục vấn đề này, dự án lựa chọn tích hợp giải thuật tìm đường A* (A-Star Pathfinding).
- **Nguyên lý hoạt động:** Thuật toán A* là sự kết hợp tối ưu giữa thuật toán Dijkstra (đảm bảo tìm đường ngắn nhất) và thuật toán tham lam Greedy Best-First-Search (định hướng nhanh tới đích). Không gian di chuyển được biểu diễn dưới dạng một ma trận lưới tọa độ 2D.
- **Hàm đánh giá chi phí:** Tại mỗi bước di chuyển, A* đánh giá các điểm nút (nodes) xung quanh dựa trên hàm chi phí tổng: F(n) = G(n) + H(n). Trong đó, G(n) là chi phí thực tế từ điểm xuất phát đến nút hiện tại, và H(n) là hàm Heuristic (chi phí ước tính từ nút hiện tại đến đích).
- **Ưu điểm:** Bằng cách sử dụng khoảng cách Chebyshev cho môi trường di chuyển 8 hướng, thuật toán A* đảm bảo luôn tìm được con đường tiếp cận người chơi tối ưu nhất mà không đi xuyên qua vật cản, đồng thời tối ưu hóa số lượng nút phải duyệt so với các thuật toán tìm kiếm mù.

**2.4 Phân tích và Đặc tả yêu cầu hệ thống**
- **Điều khiển (FR_01):** Di chuyển 8 hướng, xoay lật nhân vật (Flip) theo con trỏ chuột. Cơ chế Lướt (Dash) bằng cách nhấn phím, tiêu thụ Energy.
- **Tương tác NPC và UI (FR_02):** NPC có vùng Trigger (LevelSelectKeeper, ShopKeeper). Nhấn nút để mở bảng giao diện tương ứng và dừng thời gian (Time.timeScale = 0).
- **Hệ thống túi đồ & Kỹ năng (FR_03):** Quản lý vật phẩm bằng `ItemSO`. Cây kỹ năng sử dụng điểm kinh nghiệm (Exp) để tăng vĩnh viễn máu hoặc sát thương.

**2.5 Thiết kế hệ thống bằng ngôn ngữ mô hình hóa UML**
*(Phần này sử dụng các sơ đồ Use Case, Activity Diagram, Sequence Diagram, Class Diagram từ bản nháp gốc của bạn, vui lòng chèn các hình ảnh biểu đồ vào đây).*

---

## CHƯƠNG 3: CÀI ĐẶT CHI TIẾT, GIẢI PHÁP KỸ THUẬT VÀ THỰC NGHIỆM

**3.1 Môi trường phát triển và Cấu trúc dự án**
- Phân tách rõ ràng các thư mục: `Scripts/Enemy` (chứa FSM và A*), `Scripts/Player Scripts` (Dash, Combat, Health), `Scripts/Inventory & Shop` (ItemSO, ShopKeeper), `Scripts/ScriptableObjects`.

**3.2 Cài đặt chi tiết mã nguồn các lớp cốt lõi**

**3.2.1 Bộ điều khiển nhân vật chính và Cơ chế Lướt (Dash)**
Trong các dòng game Action RPG nhịp độ cao, khả năng né tránh đòn tấn công và căn khoảng cách là yếu tố sống còn. Dự án đã thiết kế bộ điều khiển nhân vật với hai luồng logic tách biệt nhằm đảm bảo tính toàn vẹn của mã nguồn:
- **Xử lý di chuyển cơ bản (`PlayerMovement.cs`):** Thu nhận tín hiệu đầu vào từ bàn phím để tính toán vector hướng di chuyển. Vận tốc sau đó được áp dụng trực tiếp vào `Rigidbody2D` thông qua hàm vòng đời `FixedUpdate()` nhằm đảm bảo sự đồng bộ chặt chẽ với bộ xử lý vật lý của Unity. Đồng thời, hệ thống tính toán góc xoay dựa trên vị trí con trỏ chuột thực tế trên màn hình (`Camera.main.ScreenToWorldPoint`) để lật (Flip) hình ảnh nhân vật (Sprite), giúp nhân vật luôn quay mặt về hướng ngắm bắn vũ khí.
- **Cơ chế Lướt và Quản lý Năng lượng (`PlayerDash.cs` & `EnergyManager.cs`):** Thay vì chỉ di chuyển đơn thuần, người chơi có thể nhấn phím lướt (Dash) để tăng tốc độ đột ngột trong một khoảng thời gian ngắn (Dash Duration). Khi kích hoạt, lớp `PlayerDash` sẽ vô hiệu hóa tạm thời điều khiển cơ bản, áp dụng một lực đẩy lớn vào nhân vật theo hướng di chuyển hiện tại. Đồng thời, hệ thống gọi lớp `EnergyManager` để trừ đi một lượng năng lượng (Energy) nhất định. Chỉ số năng lượng này được liên kết trực tiếp với thanh trạng thái giao diện (`EnergyUI`) thông qua Observer Pattern và sẽ tự động phục hồi (Regen) theo thời gian thực. Cơ chế này buộc người chơi phải quản lý tài nguyên chiến thuật, loại bỏ tình trạng lạm dụng lướt né đòn liên tục.

*3.2.2 Tích hợp giải thuật A* và làm mượt quỹ đạo di chuyển*
Để hiện thực hóa lý thuyết A* vào Unity, hệ thống thiết lập một lưới tọa độ động (Dynamic Grid) bao quanh quái vật thông qua lớp `AStarPathfinder.cs`.
- **Khởi tạo ma trận lưới:** Hàm `BuildGrid()` sử dụng `Physics2D.OverlapCircle()` quét bán kính mở rộng để kiểm tra lớp vật cản (`obstacleLayer`). Các vị trí an toàn sẽ được đánh dấu thuộc tính Walkable trên mảng ma trận hai chiều `walkableGrid`.
- **Kỹ thuật làm mượt quỹ đạo (Path Smoothing):** Thay vì di chuyển góc cạnh cứng nhắc theo các ô vuông, hàm `SmoothPath()` sử dụng tia quét thể tích `Physics2D.CircleCast()` để cắt bỏ các điểm trung gian dư thừa trên đường đi. Nếu một đường chéo không bị che khuất bởi tường, các điểm gấp khúc trung gian sẽ bị loại bỏ, giúp quỹ đạo của quái vật mượt mà và tự nhiên hơn.
- **Tích hợp vào Bộ điều khiển:** Các lớp AI như `Archer_Movement.cs` không trực tiếp tính toán logic dò đường mà chỉ gọi hàm `GetDirectionToTarget(player.position)` theo chu kỳ nhất định. Hàm này trả về một Vector2 chuẩn hóa để gán vào `Rigidbody2D.linearVelocity`, giúp phân tách hoàn toàn bài toán tìm đường khỏi logic máy trạng thái (FSM) của quái vật.

**3.2.3 Hệ thống NPC Tương tác và Quản lý Giao diện (UI)**
Để tăng tính nhập vai (Immersion) cho người chơi tại Sảnh chính (Main Hub), việc gọi các bảng giao diện tính năng (như Cửa hàng, Cây kỹ năng, Chọn màn chơi) không được đặt cố định trên màn hình mà được ủy quyền (delegate) cho các NPC chuyên biệt thông qua nhóm kịch bản Keeper (bao gồm `ShopKeeper.cs`, `LevelSelectKeeper.cs`, `SkillKeeper.cs`).
- **Vùng kích hoạt tương tác (Trigger Area):** Mỗi NPC được trang bị một vùng `Collider2D` thiết lập ở chế độ `IsTrigger`. Khi nhân vật chính đi vào vùng này (thông qua hàm sự kiện `OnTriggerEnter2D`), kịch bản sẽ đánh dấu cờ hiệu nội bộ và kích hoạt biến boolean `playerInRange` trên `Animator`. Quá trình này giúp hiển thị hoạt ảnh nút bấm (Visual Prompt) nổi lên trên đầu NPC, gợi ý cho người chơi phím cần nhấn để tương tác.
- **Xử lý Logic Giao diện:** Khi người chơi nhấn phím tương tác trong vùng cho phép, lớp Keeper sẽ kích hoạt các lớp quản lý giao diện trung tâm tương ứng (ví dụ: `ShopManager.Instance.OpenShop()`). Tại thời điểm giao diện mở ra, hệ thống tự động đóng băng trạng thái runtime của toàn bộ môi trường xung quanh (`Time.timeScale = 0f`) để người chơi an tâm thao tác mua sắm. Đồng thời, bộ điều khiển di chuyển của nhân vật bị vô hiệu hóa tạm thời, ngăn chặn triệt để hiện tượng lỗi xung đột phím nhận diện (UI Hopping) khi người chơi thao tác bằng chuột trên các thanh trượt (Sliders) hay nút bấm hệ thống.

**3.3 Hiện thực hóa các giải pháp nâng cao và tối ưu hiệu năng**

**3.3.1 Quản lý Dữ liệu tĩnh bằng kiến trúc ScriptableObject**
Trong các dự án game nhập vai, số lượng vật phẩm (Vũ khí, máu) và các loại quái vật thường rất lớn, đi kèm với hàng loạt thông số phức tạp (Sát thương, tốc độ, giá tiền). Nếu khai báo các thông số này trực tiếp trong lớp `MonoBehaviour` gắn trên GameObject, mỗi khi một quái vật hay vật phẩm được khởi tạo trên Scene, Unity sẽ tạo ra một bản sao (clone) mới của toàn bộ khối dữ liệu đó, gây tiêu tốn bộ nhớ RAM nghiêm trọng.
Để giải quyết bài toán này, dự án ứng dụng kiến trúc `ScriptableObject` để tách biệt hoàn toàn dữ liệu (Data) ra khỏi logic thực thi. 
- Các lớp như `ItemSO.cs` (Thông số vật phẩm), `EnemyData.cs` (Chỉ số quái vật) và `LevelConfig.cs` (Cấu hình hầm ngục) được định nghĩa kế thừa từ `ScriptableObject`.
- Mỗi vật phẩm hoặc quái vật sẽ được tạo thành một file dữ liệu độc lập (`.asset`) nằm cố định trong thư mục dự án. 
- Khi một thực thể (ví dụ: `Archer_Combat`) được sinh ra trên Scene, nó chỉ nắm giữ duy nhất một tham chiếu (Reference) trỏ đến file `.asset` để đọc chỉ số. Dù trên bản đồ có xuất hiện hàng trăm quái vật cùng loại, hệ thống cũng chỉ cấp phát và đọc dữ liệu từ một vùng nhớ tĩnh duy nhất. Phương pháp này giúp tối ưu hóa dung lượng RAM và tạo điều kiện thuận lợi cho việc cân bằng thông số (Game Balancing) trực tiếp trên Editor mà không cần sửa code C#.

**3.3.2 Triệt tiêu chu kỳ thu gom rác (Garbage Collection) bằng Object Pooling**
Trong các trận chiến nhịp độ cao, đặc biệt khi đối đầu với các quái vật đánh xa (như `Frostbite Archer`), số lượng cung tên sinh ra và bay khỏi màn hình diễn ra liên tục. Nếu sử dụng các hàm gốc của Unity là `Instantiate()` để tạo mới và `Destroy()` để xóa bỏ đạn, hệ thống sẽ phải cấp phát và giải phóng bộ nhớ động liên tục. Quá trình này kích hoạt bộ thu gom rác (Garbage Collector - GC) của C# hoạt động, gây ra hiện tượng khựng khung hình (Lag Spike) khi GC đóng băng các luồng xử lý để dọn dẹp RAM.
Để tối ưu hóa, dự án áp dụng kỹ thuật **Object Pooling (Hồ chứa đối tượng)**. 
- Hệ thống duy trì một danh sách (List hoặc Queue) chứa sẵn một lượng đạn nhất định được khởi tạo ngay khi nạp Scene nhưng ở trạng thái ẩn (`SetActive(false)`).
- Khi quái vật cần tấn công, hệ thống không tạo mới mà sẽ duyệt tìm một mũi tên đang rảnh rỗi trong Pool, lấy nó ra, thiết lập vị trí, hướng bay và bật hiển thị (`SetActive(true)`).
- Khi mũi tên trúng mục tiêu, thay vì bị phá hủy (`Destroy()`), nó sẽ tự động reset các thuộc tính vật lý và chuyển về trạng thái ẩn, sẵn sàng cho lần tái sử dụng tiếp theo. Giải pháp này đưa tần suất cấp phát bộ nhớ động về mức 0, giữ cho tốc độ khung hình luôn ổn định ở mức cao.

**3.4 Kiểm thử hệ thống và Đánh giá hiệu năng**
- **Đánh giá thuật toán A*:** Quái vật di chuyển thông minh hơn, né tránh chướng ngại vật mượt mà, không xảy ra hiện tượng kẹt vào góc tường so với thuật toán Raycast cũ.
- **FPS và RAM:** Hoạt động ổn định ở mức 60 FPS trên PC. Lượng RAM tiêu thụ dao động cực thấp nhờ `ScriptableObject` và Object Pooling.

**3.5 Kết luận và Hướng phát triển**
**Kết luận:** Đồ án đã thành công tích hợp các cơ chế RPG phức tạp bao gồm thuật toán A*, hệ thống Dash, Shop và Skill Tree. Cấu trúc mã nguồn đạt chuẩn hướng đối tượng và hiệu năng được tối ưu.
**Hướng phát triển:** Mở rộng bản đồ tự động sinh (Procedural Generation) chuyên sâu hơn; Thêm đa dạng các lớp nhân vật và vũ khí; Tối ưu hóa A* bằng cách giảm chu kỳ gọi thuật toán (Pathfinding Caching) để tối ưu CPU.
