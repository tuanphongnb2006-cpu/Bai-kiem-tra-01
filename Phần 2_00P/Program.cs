using System;
using System.Collections.Generic;
using System.Linq;

public abstract class PhuongTien
{
    private string _maPT = "PT000";
    private string _tenHang = "";
    private int _namSanXuat;
    private decimal _giaGoc;

    public string MaPT
    {
        get { return _maPT; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Mã phương tiện không được để trống!");
            _maPT = value;
        }
    }

    public string TenHang
    {
        get { return _tenHang; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Tên hãng không được để trống!");
            _tenHang = value;
        }
    }

    public int NamSanXuat
    {
        get { return _namSanXuat; }
        set
        {
            if (value < 1900 || value > DateTime.Now.Year)
                throw new ArgumentException("Năm sản xuất không hợp lệ!");
            _namSanXuat = value;
        }
    }

    public decimal GiaGoc
    {
        get { return _giaGoc; }
        set
        {
            if (value <= 0)
                throw new ArgumentException("Giá gốc phải lớn hơn 0!");
            _giaGoc = value;
        }
    }

    public PhuongTien(string maPT, string tenHang, int namSanXuat, decimal giaGoc)
    {
        MaPT = maPT;
        TenHang = tenHang;
        NamSanXuat = namSanXuat;
        GiaGoc = giaGoc;
    }

    public abstract decimal TinhGiaLanBanh();

    public virtual string GetInfo()
    {
        return "Mã: " + MaPT + " | Hãng: " + TenHang + " | Năm SX: " + NamSanXuat + " | Giá gốc: " + GiaGoc.ToString("N0") + " VNĐ";
    }
}

public class OTo : PhuongTien
{
    private int _soChoNgoi;
    private double _dungTichDongCo;

    public int SoChoNgoi
    {
        get { return _soChoNgoi; }
        set
        {
            if (value <= 0)
                throw new ArgumentException("Số chỗ ngồi phải lớn hơn 0!");
            _soChoNgoi = value;
        }
    }

    public double DungTichDongCo
    {
        get { return _dungTichDongCo; }
        set
        {
            if (value <= 0)
                throw new ArgumentException("Dung tích động cơ phải lớn hơn 0!");
            _dungTichDongCo = value;
        }
    }

    public OTo(string maPT, string tenHang, int namSanXuat, decimal giaGoc, int soChoNgoi, double dungTichDongCo)
        : base(maPT, tenHang, namSanXuat, giaGoc)
    {
        SoChoNgoi = soChoNgoi;
        DungTichDongCo = dungTichDongCo;
    }

    public override decimal TinhGiaLanBanh()
    {
        if (SoChoNgoi <= 9)
            return GiaGoc + GiaGoc * 0.12m + GiaGoc * 0.30m;
        return GiaGoc + GiaGoc * 0.10m;
    }

    public override string GetInfo()
    {
        return base.GetInfo() + " | Số chỗ: " + SoChoNgoi + " | Dung tích động cơ: " + DungTichDongCo + " L";
    }
}

public class XeMay : PhuongTien
{
    private int _dungTichXylanh;

    public int DungTichXylanh
    {
        get { return _dungTichXylanh; }
        set
        {
            if (value <= 0)
                throw new ArgumentException("Dung tích xylanh phải lớn hơn 0!");
            _dungTichXylanh = value;
        }
    }

    public XeMay(string maPT, string tenHang, int namSanXuat, decimal giaGoc, int dungTichXylanh)
        : base(maPT, tenHang, namSanXuat, giaGoc)
    {
        DungTichXylanh = dungTichXylanh;
    }

    public override decimal TinhGiaLanBanh()
    {
        if (DungTichXylanh < 175)
            return GiaGoc + GiaGoc * 0.02m;
        return GiaGoc + GiaGoc * 0.05m;
    }

    public override string GetInfo()
    {
        return base.GetInfo() + " | Dung tích xylanh: " + DungTichXylanh + " cc";
    }
}

public class QuanLyPhuongTien
{
    private List<PhuongTien> _ds = new List<PhuongTien>();

    public void AddPhuongTien(PhuongTien pt)
    {
        if (pt == null)
            throw new ArgumentNullException("pt");
        _ds.Add(pt);
    }

    public void DisplayAll()
    {
        if (_ds.Count == 0)
        {
            Console.WriteLine("Danh sách trống.");
            return;
        }
        foreach (PhuongTien pt in _ds)
        {
            Console.WriteLine(pt.GetInfo());
            Console.WriteLine("   => Giá lăn bánh: " + pt.TinhGiaLanBanh().ToString("N0") + " VNĐ");
        }
    }

    public PhuongTien FindMaxGiaLanBanh()
    {
        if (_ds.Count == 0)
            return null;
        PhuongTien max = _ds[0];
        foreach (PhuongTien pt in _ds)
        {
            if (pt.TinhGiaLanBanh() > max.TinhGiaLanBanh())
                max = pt;
        }
        return max;
    }

    public List<PhuongTien> SearchByName(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<PhuongTien>();
        return _ds.Where(pt => pt.TenHang.ToLower().Contains(keyword.Trim().ToLower())).ToList();
    }
}

public class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("===== TC01: Validation năm sản xuất =====");
        try
        {
            OTo loi = new OTo("PT001", "Toyota", 1850, 1000000000m, 5, 2.0);
            Console.WriteLine("Tạo được đối tượng (SAI)");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("Ném ngoại lệ: " + ex.Message);
        }

        OTo oto = new OTo("PT002", "Toyota Camry", 2022, 1000000000m, 5, 2.5);
        XeMay xe = new XeMay("PT003", "Honda Winner", 2023, 50000000m, 150);

        Console.WriteLine();
        Console.WriteLine("===== TC02: Giá lăn bánh ô tô 5 chỗ =====");
        Console.WriteLine(oto.TinhGiaLanBanh().ToString("N0") + " VNĐ");

        Console.WriteLine();
        Console.WriteLine("===== TC03: Giá lăn bánh xe máy 150cc =====");
        Console.WriteLine(xe.TinhGiaLanBanh().ToString("N0") + " VNĐ");

        Console.WriteLine();
        Console.WriteLine("===== TC04: Đa hình List<PhuongTien> =====");
        List<PhuongTien> ds = new List<PhuongTien>();
        ds.Add(oto);
        ds.Add(xe);
        foreach (PhuongTien pt in ds)
        {
            Console.WriteLine(pt.TenHang + ": " + pt.TinhGiaLanBanh().ToString("N0") + " VNĐ");
        }

        QuanLyPhuongTien ql = new QuanLyPhuongTien();
        ql.AddPhuongTien(oto);
        ql.AddPhuongTien(xe);

        Console.WriteLine();
        Console.WriteLine("===== Danh sách toàn bộ phương tiện =====");
        ql.DisplayAll();

        Console.WriteLine();
        Console.WriteLine("===== TC05: Phương tiện có giá lăn bánh cao nhất =====");
        PhuongTien max = ql.FindMaxGiaLanBanh();
        if (max != null)
        {
            Console.WriteLine(max.GetInfo());
            Console.WriteLine("Giá lăn bánh: " + max.TinhGiaLanBanh().ToString("N0") + " VNĐ");
        }

        Console.WriteLine();
        Console.WriteLine("===== Tìm kiếm theo tên hãng: \"honda\" =====");
        List<PhuongTien> kq = ql.SearchByName("honda");
        foreach (PhuongTien pt in kq)
        {
            Console.WriteLine(pt.GetInfo());
        }

        Console.ReadKey();
    }
}
