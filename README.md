⚔️ The Last Gleam - 2D Action RPG
* The Last Gleam là một trò chơi nhập vai hành động (RPG) phong cách Pixel Art, được xây dựng trên Unity Engine. Người chơi vào vai Ember, nữ chiến binh ánh sáng cuối cùng, trên hành trình giải cứu vùng đất Elaria khỏi sự thao túng của bóng tối và người đồng đội cũ Morveth.
  
🚀 Tính năng nổi bật (Key Features)
* Hệ thống chiến đấu linh hoạt (Fluid Combat System): Chuỗi combo tấn công mượt mà kết hợp với các kỹ năng di chuyển nâng cao như lướt (dash), nhảy tường (wall jump) và trượt tường (wall slide).
* Hệ thống AI đa dạng: Kẻ địch có hành vi tuần tra, phát hiện và tấn công dựa trên State Machine. Boss cuối (Morveth) được thiết kế với nhiều giai đoạn (phases) và bộ kỹ năng phức tạp.
* Hệ thống tiến hóa nhân vật (RPG Elements): * Skill Tree: Mở khóa và nâng cấp kỹ năng bằng linh hồn (Souls) thu thập được.
* Crafting & Inventory: Thu thập nguyên liệu từ môi trường để chế tạo trang bị, giúp gia tăng chỉ số sinh tồn và sát thương.
* Môi trường tương tác: Bản đồ phân cấp từ Rừng Sâu, Hang Tối đến Lâu Đài, tích hợp hệ thống lưu tiến trình (Checkpoint).

🛠 Kiến trúc kỹ thuật (Technical Highlights)
* Dự án chú trọng vào việc tổ chức mã nguồn khoa học, dễ bảo trì và mở rộng:
* Finite State Machine (FSM): Sử dụng FSM để quản lý trạng thái phức tạp của nhân vật và AI, giúp tách biệt logic xử lý và tránh lỗi kẹt khung hình (Animation Glitches).
* Scriptable Objects: Áp dụng để quản lý dữ liệu vật phẩm, hội thoại và chỉ số quái vật, giúp dễ dàng cân bằng game mà không cần can thiệp sâu vào mã nguồn.
* Singleton Pattern: Quản lý tập trung các hệ thống quan trọng như GameManager, AudioManager và PlayerManager.
* Save/Load System: Triển khai cơ chế lưu trữ dữ liệu dưới dạng JSON, đảm bảo tính toàn vẹn của chỉ số nhân vật và tiến trình game khi chơi lại.
* Version Control: Quản lý dự án qua Unity DevOps để tối ưu hóa quy trình làm việc nhóm và theo dõi lịch sử thay đổi.

📂 Cấu trúc mã nguồn (Project Structure)
Plaintext

Scripts/
├── Player/          # Logic điều khiển, FSM nhân vật chính (Ember)
├── Enemy/           # AI quái vật (Skeleton, Mushroom) và Boss (DeathBringer)
├── Items/           # Hệ thống vật phẩm, Inventory và Crafting
├── UI/              # Quản lý giao diện, HUD, Skill Tree
├── Managers/        # Các lớp Singleton điều phối toàn bộ game
└── Save & Load/     # Logic xử lý lưu trữ dữ liệu người dùng

💻 Yêu cầu hệ thống (System Requirements)
* OS: Windows 10 (64-bit).
* CPU: Intel Pentium IV 3.0 GHz / AMD Athlon64 1.8 GHz.
* RAM: 2 GB.
* DirectX: Phiên bản June 2010.

👥 Đội ngũ phát triển (Contributors)
* Nguyễn Trần Bảo Thương: Trưởng nhóm, Thiết kế Core Gameplay, Backend & Âm thanh.
* Hồ Công Minh: Thiết kế bản đồ (Map), Quản lý Asset, Kiểm thử & Báo cáo.
* Lý Hoàng Phúc: Thiết kế UI/UX, Logic Frontend & Hệ thống Inventory.
