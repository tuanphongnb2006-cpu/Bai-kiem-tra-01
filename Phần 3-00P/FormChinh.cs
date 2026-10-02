#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class Product
{
    public string ProductId { get; set; } = "";
    public string ProductName { get; set; } = "";
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string ImagePath { get; set; } = "";
}

public class FormChinh : Form
{
    TextBox txtProductId = new TextBox();
    TextBox txtProductName = new TextBox();
    TextBox txtUnitPrice = new TextBox();
    TextBox txtQuantity = new TextBox();
    ComboBox cboCategory = new ComboBox();
    PictureBox picAvatar = new PictureBox();
    Button btnChooseImage = new Button();
    Button btnAdd = new Button();
    Button btnUpdate = new Button();
    Button btnDelete = new Button();
    Button btnExport = new Button();
    TextBox txtSearch = new TextBox();
    DataGridView dgvProducts = new DataGridView();
    ErrorProvider errorProvider = new ErrorProvider();
    BindingSource bindingSource = new BindingSource();
    ToolStripStatusLabel lblTotal = new ToolStripStatusLabel();

    List<Product> allProducts = new List<Product>();
    BindingList<Product> showList = new BindingList<Product>();
    string currentImagePath = "";
    int nextId = 1;
    bool loading = false;

    public FormChinh()
    {
        Text = "TechMart Product Manager";
        Size = new Size(1100, 650);
        MinimumSize = new Size(900, 550);
        StartPosition = FormStartPosition.CenterScreen;

        errorProvider.ContainerControl = this;
        errorProvider.BlinkStyle = ErrorBlinkStyle.AlwaysBlink;

        BuildLayout();
        BuildStatus();
        BuildMenu();
        LoadCategories();
        LoadGrid();
    }

    void BuildLayout()
    {
        TableLayoutPanel main = new TableLayoutPanel();
        main.Dock = DockStyle.Fill;
        main.ColumnCount = 2;
        main.RowCount = 1;
        main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
        main.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        main.Controls.Add(BuildLeft(), 0, 0);
        main.Controls.Add(BuildRight(), 1, 0);
        Controls.Add(main);
    }

    TableLayoutPanel BuildLeft()
    {
        TableLayoutPanel left = new TableLayoutPanel();
        left.Dock = DockStyle.Fill;
        left.Padding = new Padding(10);
        left.ColumnCount = 2;
        left.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        left.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < 5; i++)
            left.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        left.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        AddRow(left, "Mã SP:", txtProductId, 0);
        AddRow(left, "Tên SP:", txtProductName, 1);
        AddRow(left, "Đơn giá:", txtUnitPrice, 2);
        AddRow(left, "Số lượng:", txtQuantity, 3);
        AddRow(left, "Danh mục:", cboCategory, 4);

        btnChooseImage.Text = "Chọn ảnh";
        btnChooseImage.AutoSize = true;
        btnChooseImage.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        btnChooseImage.Click += (s, e) => ChooseImage();
        left.Controls.Add(btnChooseImage, 0, 5);

        picAvatar.Dock = DockStyle.Fill;
        picAvatar.SizeMode = PictureBoxSizeMode.Zoom;
        picAvatar.BorderStyle = BorderStyle.FixedSingle;
        picAvatar.MinimumSize = new Size(100, 100);
        left.Controls.Add(picAvatar, 1, 5);

        btnAdd.Text = "Thêm mới";
        btnUpdate.Text = "Cập nhật";
        btnDelete.Text = "Xóa";
        btnExport.Text = "Xuất CSV";
        btnAdd.AutoSize = true;
        btnUpdate.AutoSize = true;
        btnDelete.AutoSize = true;
        btnExport.AutoSize = true;
        btnAdd.Click += (s, e) => AddProduct();
        btnUpdate.Click += (s, e) => UpdateProduct();
        btnDelete.Click += (s, e) => DeleteProduct();
        btnExport.Click += (s, e) => ExportCsv();

        FlowLayoutPanel flow = new FlowLayoutPanel();
        flow.Dock = DockStyle.Fill;
        flow.AutoSize = true;
        flow.WrapContents = true;
        flow.Controls.Add(btnAdd);
        flow.Controls.Add(btnUpdate);
        flow.Controls.Add(btnDelete);
        flow.Controls.Add(btnExport);
        left.Controls.Add(flow, 0, 6);
        left.SetColumnSpan(flow, 2);

        return left;
    }

    void AddRow(TableLayoutPanel panel, string caption, Control ctrl, int row)
    {
        Label lbl = new Label();
        lbl.Text = caption;
        lbl.AutoSize = true;
        lbl.Anchor = AnchorStyles.Left;
        ctrl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        ctrl.Margin = new Padding(3, 6, 3, 6);
        panel.Controls.Add(lbl, 0, row);
        panel.Controls.Add(ctrl, 1, row);
    }

    TableLayoutPanel BuildRight()
    {
        TableLayoutPanel right = new TableLayoutPanel();
        right.Dock = DockStyle.Fill;
        right.Padding = new Padding(10);
        right.ColumnCount = 2;
        right.RowCount = 2;
        right.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        right.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        Label lblSearch = new Label();
        lblSearch.Text = "Tìm kiếm:";
        lblSearch.AutoSize = true;
        lblSearch.Anchor = AnchorStyles.Left;
        txtSearch.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        txtSearch.Margin = new Padding(3, 6, 3, 6);
        txtSearch.TextChanged += (s, e) => LoadGrid();

        SetupGrid();

        right.Controls.Add(lblSearch, 0, 0);
        right.Controls.Add(txtSearch, 1, 0);
        right.Controls.Add(dgvProducts, 0, 1);
        right.SetColumnSpan(dgvProducts, 2);

        return right;
    }

    void SetupGrid()
    {
        dgvProducts.Dock = DockStyle.Fill;
        dgvProducts.AutoGenerateColumns = false;
        dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvProducts.MultiSelect = false;
        dgvProducts.ReadOnly = true;
        dgvProducts.AllowUserToAddRows = false;
        dgvProducts.AllowUserToDeleteRows = false;
        dgvProducts.RowHeadersVisible = false;
        dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        AddColumn("ProductId", "Mã SP");
        AddColumn("ProductName", "Tên SP");
        AddColumn("CategoryName", "Danh Mục");
        DataGridViewTextBoxColumn colPrice = AddColumn("UnitPrice", "Đơn Giá (VNĐ)");
        colPrice.DefaultCellStyle.Format = "N0";
        colPrice.DefaultCellStyle.FormatProvider = new CultureInfo("en-US");
        colPrice.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        AddColumn("Quantity", "Số Lượng");

        bindingSource.DataSource = showList;
        dgvProducts.DataSource = bindingSource;
        dgvProducts.SelectionChanged += (s, e) => ShowSelected();
    }

    DataGridViewTextBoxColumn AddColumn(string property, string header)
    {
        DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
        col.Name = "col" + property;
        col.DataPropertyName = property;
        col.HeaderText = header;
        dgvProducts.Columns.Add(col);
        return col;
    }

    void BuildStatus()
    {
        StatusStrip status = new StatusStrip();
        status.Items.Add(lblTotal);
        status.Dock = DockStyle.Bottom;
        Controls.Add(status);
    }

    void BuildMenu()
    {
        MenuStrip menu = new MenuStrip();
        ToolStripMenuItem mFile = new ToolStripMenuItem("File");

        ToolStripMenuItem mExport = new ToolStripMenuItem("Export CSV");
        mExport.ShortcutKeys = Keys.Control | Keys.E;
        mExport.Click += (s, e) => ExportCsv();

        ToolStripMenuItem mExit = new ToolStripMenuItem("Exit");
        mExit.ShortcutKeys = Keys.Control | Keys.X;
        mExit.Click += (s, e) => Close();

        mFile.DropDownItems.Add(mExport);
        mFile.DropDownItems.Add(mExit);
        menu.Items.Add(mFile);
        menu.Dock = DockStyle.Top;
        MainMenuStrip = menu;
        Controls.Add(menu);
    }

    void LoadCategories()
    {
        List<Category> cats = new List<Category>();
        cats.Add(new Category { Id = 1, Name = "Điện thoại" });
        cats.Add(new Category { Id = 2, Name = "Laptop" });
        cats.Add(new Category { Id = 3, Name = "Phụ kiện" });

        cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        cboCategory.DisplayMember = "Name";
        cboCategory.ValueMember = "Id";
        cboCategory.DataSource = cats;
        cboCategory.SelectedIndex = 0;
    }

    void LoadGrid()
    {
        loading = true;
        string key = txtSearch.Text.Trim().ToLower();
        showList.Clear();
        foreach (Product p in allProducts)
        {
            if (p.ProductName.ToLower().Contains(key))
                showList.Add(p);
        }
        dgvProducts.ClearSelection();
        loading = false;
        lblTotal.Text = "Tổng số sản phẩm: " + allProducts.Count;
    }

    Product GetSelected()
    {
        if (dgvProducts.SelectedRows.Count == 0)
            return null;
        return dgvProducts.SelectedRows[0].DataBoundItem as Product;
    }

    void ShowSelected()
    {
        if (loading)
            return;
        Product p = GetSelected();
        if (p == null)
            return;

        errorProvider.Clear();
        txtProductId.Text = p.ProductId;
        txtProductName.Text = p.ProductName;
        txtUnitPrice.Text = p.UnitPrice.ToString("0.##", CultureInfo.InvariantCulture);
        txtQuantity.Text = p.Quantity.ToString();
        cboCategory.SelectedValue = p.CategoryId;
        currentImagePath = p.ImagePath;
        ShowImage(currentImagePath);
    }

    void ShowImage(string path)
    {
        if (picAvatar.Image != null)
        {
            picAvatar.Image.Dispose();
            picAvatar.Image = null;
        }
        if (path != "" && File.Exists(path))
        {
            using (Image img = Image.FromFile(path))
            {
                picAvatar.Image = new Bitmap(img);
            }
        }
    }

    void ChooseImage()
    {
        using (OpenFileDialog dlg = new OpenFileDialog())
        {
            dlg.Title = "Chọn ảnh sản phẩm";
            dlg.Filter = "Tệp ảnh (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ShowImage(dlg.FileName);
                    currentImagePath = dlg.FileName;
                }
                catch (Exception)
                {
                    MessageBox.Show("Không đọc được file ảnh này!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    bool CheckInput()
    {
        bool ok = true;
        errorProvider.Clear();

        if (txtProductName.Text.Trim() == "")
        {
            errorProvider.SetError(txtProductName, "Tên sản phẩm không được để trống");
            ok = false;
        }

        decimal price;
        if (!decimal.TryParse(txtUnitPrice.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out price) || price <= 0)
        {
            errorProvider.SetError(txtUnitPrice, "Đơn giá phải là số lớn hơn 0");
            ok = false;
        }

        int qty;
        if (!int.TryParse(txtQuantity.Text.Trim(), out qty) || qty < 0)
        {
            errorProvider.SetError(txtQuantity, "Số lượng phải là số nguyên lớn hơn hoặc bằng 0");
            ok = false;
        }

        return ok;
    }

    void ClearInputs()
    {
        txtProductId.Text = "";
        txtProductName.Text = "";
        txtUnitPrice.Text = "";
        txtQuantity.Text = "";
        cboCategory.SelectedIndex = 0;
        currentImagePath = "";
        ShowImage("");
        errorProvider.Clear();
        dgvProducts.ClearSelection();
    }

    void AddProduct()
    {
        if (!CheckInput())
            return;

        string id = txtProductId.Text.Trim();
        if (id == "")
        {
            do
            {
                id = "SP" + nextId.ToString("D3");
                nextId++;
            } while (allProducts.Any(x => x.ProductId == id));
        }
        else if (allProducts.Any(x => x.ProductId == id))
        {
            errorProvider.SetError(txtProductId, "Mã sản phẩm đã tồn tại");
            return;
        }

        Category cat = (Category)cboCategory.SelectedItem;
        Product p = new Product();
        p.ProductId = id;
        p.ProductName = txtProductName.Text.Trim();
        p.CategoryId = cat.Id;
        p.CategoryName = cat.Name;
        p.UnitPrice = decimal.Parse(txtUnitPrice.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture);
        p.Quantity = int.Parse(txtQuantity.Text.Trim());
        p.ImagePath = currentImagePath;

        allProducts.Add(p);
        LoadGrid();
        ClearInputs();
    }

    void UpdateProduct()
    {
        Product p = GetSelected();
        if (p == null)
        {
            MessageBox.Show("Hãy chọn một sản phẩm trên bảng để cập nhật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (!CheckInput())
            return;

        Category cat = (Category)cboCategory.SelectedItem;
        p.ProductName = txtProductName.Text.Trim();
        p.CategoryId = cat.Id;
        p.CategoryName = cat.Name;
        p.UnitPrice = decimal.Parse(txtUnitPrice.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture);
        p.Quantity = int.Parse(txtQuantity.Text.Trim());
        p.ImagePath = currentImagePath;

        LoadGrid();
        ClearInputs();
    }

    void DeleteProduct()
    {
        Product p = GetSelected();
        if (p == null)
        {
            MessageBox.Show("Hãy chọn một sản phẩm trên bảng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        DialogResult r = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm \"" + p.ProductName + "\" không?",
            "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (r == DialogResult.Yes)
        {
            allProducts.Remove(p);
            LoadGrid();
            ClearInputs();
        }
    }

    string Csv(string s)
    {
        if (s.Contains(",") || s.Contains("\"") || s.Contains("\n"))
            return "\"" + s.Replace("\"", "\"\"") + "\"";
        return s;
    }

    void ExportCsv()
    {
        using (SaveFileDialog dlg = new SaveFileDialog())
        {
            dlg.Title = "Xuất danh sách sản phẩm";
            dlg.Filter = "Tệp CSV (*.csv)|*.csv";
            dlg.FileName = "sanpham.csv";
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                using (StreamWriter sw = new StreamWriter(dlg.FileName, false, new UTF8Encoding(true)))
                {
                    sw.WriteLine("Mã SP,Tên SP,Danh Mục,Đơn Giá,Số Lượng");
                    foreach (Product p in allProducts)
                    {
                        sw.WriteLine(Csv(p.ProductId) + "," + Csv(p.ProductName) + "," + Csv(p.CategoryName) + ","
                            + p.UnitPrice.ToString("0.##", CultureInfo.InvariantCulture) + "," + p.Quantity);
                    }
                }
                MessageBox.Show("Xuất file thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể ghi file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
