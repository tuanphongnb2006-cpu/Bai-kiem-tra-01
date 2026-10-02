Câu 1:Value Types và Reference Types(Stack vs Heap)
Tiêu chí               	|Value Type                                    	      |Value Type
Ví dụ                 	|-int, double, decimal, bool, char, struct, enum      |-class, string, array, interface, delegate, object
Cái được lưu            |-Chính giá trị dữ liệu                               |-Địa chỉ(tham chiếu) trỏ tới đối tượng
Vùng nhớ                |-Thường nằm trên Stack(biến cục bộ). Nếu là field    |-Biến tham chiếu nằm trên Stack, còn đối tượng thật nằm 
                          của một class thì nằm trong đối tượng đó trên Heap  |trên Heap
Khi gán b-a             |-Sao chép giá trị, hai biến độc lập                  |-Sao chép địa chỉ, hai biến cùng trỏ một đối tượng
Giá trị null            |-Không nhận null(trừ khi dùng int?)                  |-Có thể là null
Giải phóng              |-Tự mất khi ra khỏi phạm vi                          |-Do Garbage Collector(GC) dọn khi không còn tham chiếu

Câu 2:Init-only Properties(init)
-Thuộc tính set thông thường có thể gán lại giá trị ở bất kỳ lúc nào, bất kỳ đâu có quyền truy cập.
-Thuộc tính init chỉ cho phép gán trong lúc khởi tạo đối tượng: trong constructor, trong object initializer new X{ ... } hoặc trong biểu thức with. Sau khi khởi tạo xong, nó trở thành chỉ đọc, gán lại sẽ báo lỗi biên dịch.

public class SanPham
{
    public string Ma { get; init; }
    public string Ten { get; set; }
}

var sp = new SanPham { Ma = "SP01", Ten = "Laptop" };
sp.Ten = "PC";     
sp.Ma = "SP02";

Trường hợp sử dụng thực tế:
-Các lớp DTO / Model truyền dữ liệu giữa các tầng, dữ liệu không được sửa sau khi tạo.
-Đối tượng cấu hình(configuration), dữ liệu đọc từ API hoặc database.
-Khóa chính, mã định danh không được thay đổi.
-Đối tượng bất biến(immutable) nên an toàn khi dùng đa luồng; đồng thời vẫn viết được bằng object initializer gọn gàng, không cần constructor nhiều tham số.

Câu 3:Phương thức virtual và override trong đa hình
-virtual đặt ở lớp cha. Nó cung cấp một cài đặt mặc định và cho phép lớp con viết lại(ghi đè).
-override đặt ở lớp con. Nó viết lại phương thức virtual(hoặc abstract) của lớp cha, bắt buộc cùng tên, kiểu trả về và tham số.
-Khi gọi qua biến kiểu lớp cha, C# quyết định phương thức nào chạy dựa vào kiểu thực của đối tượng lúc chạy(liên kết muộn, runtime binding), không dựa vào kiểu khai báo của biến. Đó chính là đa hình.

class PhuongTien { public virtual string GetInfo() { return "Phương tiện"; } }
class OTo : PhuongTien { public override string GetInfo() { return "Ô tô"; } }

PhuongTien p = new OTo();
Console.WriteLine(p.GetInfo());

Câu 4:Vì sao thành phần static không truy xuất qua đối tượng tạo bằng new?
-Thành viên static thuộc về chính lớp (kiểu dữ liệu), chỉ có một bản duy nhất dùng chung và được tạo ra khi lớp được nạp, không cần tạo đối tượng.
-Mỗi đối tượng tạo bằng new chỉ chứa các thành viên instance (của riêng nó), không chứa thành viên static.
-Vì vậy C# bắt buộc truy cập qua tên lớp (TenLop.ThanhVien). Gọi qua đối tượng (obj.ThanhVien) báo lỗi CS0176. Quy định này tránh nhầm lẫn rằng dữ liệu static là của riêng từng đối tượng.


class Dem { public static int Tong = 0; }
Dem d = new Dem();
d.Tong = 5;       
Dem.Tong = 5;
