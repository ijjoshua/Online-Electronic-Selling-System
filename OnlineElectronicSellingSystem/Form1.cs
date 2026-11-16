using OnlineElectronicSellingSystem.Controllers;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem;

public partial class Form1 : Form
{
    private readonly Database database;
    private readonly OessRepository repository;
    private readonly AuthController auth;
    private readonly ProductController products;
    private readonly SalesController sales;
    private readonly ReportsController reports;
    private readonly AdvancedRepository advancedRepository;
    private readonly AdvancedController advanced;
    private User? currentUser;
    private DataGridView productGrid = null!, cartGrid = null!, orderGrid = null!;
    private TextBox searchBox = null!;
    private ComboBox paymentSelector = null!;

    public Form1()
    {
        InitializeComponent();
        database = new Database();
        repository = new OessRepository(database);
        auth = new AuthController(repository);
        products = new ProductController(repository);
        sales = new SalesController(repository);
        reports = new ReportsController(repository);
        advancedRepository = new AdvancedRepository(database);
        advanced = new AdvancedController(advancedRepository);
        ShowLogin();
    }

    private void ShowLogin()
    {
        Controls.Clear(); Text = "OESS - Sign in"; ClientSize = new Size(460, 360); StartPosition = FormStartPosition.CenterScreen;
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(35), ColumnCount = 2, RowCount = 6 };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115)); panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.Controls.Add(new Label { Text = "Online Electronic Selling System", Font = new Font(Font, FontStyle.Bold), AutoSize = true }, 0, 0); panel.SetColumnSpan(panel.GetControlFromPosition(0, 0)!, 2);
        var name = new TextBox { PlaceholderText = "Required for new customers", Dock = DockStyle.Fill };
        var email = new TextBox { Dock = DockStyle.Fill }; var password = new TextBox { UseSystemPasswordChar = true, Dock = DockStyle.Fill };
        panel.Controls.Add(new Label { Text = "Full name", AutoSize = true }, 0, 1); panel.Controls.Add(name, 1, 1);
        panel.Controls.Add(new Label { Text = "Email", AutoSize = true }, 0, 2); panel.Controls.Add(email, 1, 2);
        panel.Controls.Add(new Label { Text = "Password", AutoSize = true }, 0, 3); panel.Controls.Add(password, 1, 3);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
        var login = new Button { Text = "Sign in", AutoSize = true }; login.Click += (_, _) => Login(email.Text, password.Text);
        var register = new Button { Text = "Register customer", AutoSize = true }; register.Click += (_, _) => RegisterCustomer(name.Text, email.Text, password.Text);
        buttons.Controls.Add(login); buttons.Controls.Add(register); panel.Controls.Add(buttons, 1, 4);
        panel.Controls.Add(new Label { Text = "Development admin: admin@oess.local / admin", ForeColor = Color.DimGray, AutoSize = true }, 0, 5); panel.SetColumnSpan(panel.GetControlFromPosition(0, 5)!, 2);
        Controls.Add(panel); AcceptButton = login;
    }

    private void Login(string email, string password)
    {
        try
        {
            currentUser = auth.Login(email, password);
            if (currentUser is null) { MessageBox.Show("Invalid credentials or inactive account.", "Sign in", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            ShowDashboard();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Database connection error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void RegisterCustomer(string fullName, string email, string password)
    {
        try
        {
            auth.RegisterCustomer(fullName, email, password);
            MessageBox.Show("Customer account created. You can now sign in.", "Registration", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Registration failed", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private void ShowDashboard()
    {
        Text = $"OESS - {currentUser!.FullName} ({currentUser.Role})"; WindowState = FormWindowState.Maximized; Controls.Clear();
        var tabs = new TabControl { Dock = DockStyle.Fill }; tabs.TabPages.Add(BuildCatalogueTab()); tabs.TabPages.Add(BuildCartTab()); tabs.TabPages.Add(BuildOrdersTab());
        if (currentUser.Role is UserRole.Admin or UserRole.Seller or UserRole.SalesOfficer) { tabs.TabPages.Add(BuildManagementTab()); tabs.TabPages.Add(BuildReportsTab()); }
        if (currentUser.Role is UserRole.Admin or UserRole.Seller or UserRole.SalesOfficer or UserRole.SupportAgent or UserRole.DeliveryAgent) tabs.TabPages.Add(BuildAdvancedTab());
        var logout = new Button { Text = "Sign out", Dock = DockStyle.Top, Height = 32 }; logout.Click += (_, _) => { currentUser = null; WindowState = FormWindowState.Normal; ShowLogin(); };
        Controls.Add(tabs); Controls.Add(logout); RefreshAll();
    }

    private TabPage BuildCatalogueTab()
    {
        var page = new TabPage("Catalogue"); var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(6) }; searchBox = new TextBox { Width = 260 }; var search = new Button { Text = "Search" }; search.Click += (_, _) => RefreshProducts(); top.Controls.Add(new Label { Text = "Search", AutoSize = true, Padding = new Padding(0, 7, 0, 0) }); top.Controls.Add(searchBox); top.Controls.Add(search);
        var add = new Button { Text = "Add selected to cart" }; add.Click += (_, _) => AddSelectedToCart(); top.Controls.Add(add); productGrid = CreateGrid(); page.Controls.Add(productGrid); page.Controls.Add(top); return page;
    }

    private TabPage BuildCartTab()
    {
        var page = new TabPage("Cart / Checkout"); var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(6) }; var remove = new Button { Text = "Remove selected" }; remove.Click += (_, _) => RemoveSelectedCart(); var checkout = new Button { Text = "Checkout" }; checkout.Click += (_, _) => Checkout(); top.Controls.Add(remove); top.Controls.Add(new Label { Text = "Payment:", Padding = new Padding(15, 7, 0, 0) }); paymentSelector = new ComboBox { Width = 150, DropDownStyle = ComboBoxStyle.DropDownList }; paymentSelector.Items.AddRange(["Card", "Mobile Money", "Bank Transfer", "Cash on Delivery"]); paymentSelector.SelectedIndex = 0; top.Controls.Add(paymentSelector); top.Controls.Add(checkout); cartGrid = CreateGrid(); page.Controls.Add(cartGrid); page.Controls.Add(top); return page;
    }

    private TabPage BuildOrdersTab() { var page = new TabPage("Orders"); orderGrid = CreateGrid(); page.Controls.Add(orderGrid); return page; }

    private TabPage BuildManagementTab()
    {
        var page = new TabPage("Products / Inventory"); var form = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 95, Padding = new Padding(6) }; var sku = new TextBox { PlaceholderText = "SKU", Width = 110 }; var name = new TextBox { PlaceholderText = "Product name", Width = 180 }; var category = new TextBox { PlaceholderText = "Category", Width = 110 }; var price = new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000, Width = 100 }; var stock = new NumericUpDown { Maximum = 1000000, Width = 80 }; var save = new Button { Text = "Save product" }; save.Click += (_, _) => { try { products.Save(new Product { Sku = sku.Text, Name = name.Text, Category = category.Text, Price = price.Value, StockQuantity = (int)stock.Value, ReorderLevel = 5 }, currentUser!); RefreshProducts(); MessageBox.Show("Product saved."); } catch (Exception ex) { MessageBox.Show(ex.Message); } }; form.Controls.AddRange([sku, name, category, price, stock, save]); form.Controls.Add(new Label { Text = "Use Catalogue to browse products; enter SKU, name, category, price and stock here.", AutoSize = true, Padding = new Padding(5, 7, 0, 0) }); page.Controls.Add(form); return page;
    }

    private TabPage BuildReportsTab() { var page = new TabPage("Reports"); var label = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopLeft, Font = new Font(Font.FontFamily, 14), Padding = new Padding(30) }; page.Tag = label; page.Controls.Add(label); page.Enter += (_, _) => { var r = reports.Dashboard(currentUser!); label.Text = $"TODAY'S DASHBOARD\n\nOrders today: {r.OrdersToday}\nRevenue today: {r.RevenueToday:C}\nPending orders: {r.PendingOrders}\nLow-stock products: {r.LowStockProducts}\nRegistered customers: {r.Customers}"; }; return page; }

    private TabPage BuildAdvancedTab()
    {
        var page = new TabPage("Advanced Modules"); var tabs = new TabControl { Dock = DockStyle.Fill };
        if (currentUser!.Role == UserRole.Admin) { tabs.TabPages.Add(BuildCustomersPage()); tabs.TabPages.Add(BuildCouponsPage()); tabs.TabPages.Add(BuildAuditPage()); }
        if (currentUser.Role is UserRole.Admin or UserRole.Seller) tabs.TabPages.Add(BuildSuppliersPage());
        tabs.TabPages.Add(BuildTicketsPage());
        if (currentUser.Role is UserRole.Admin or UserRole.SalesOfficer or UserRole.SupportAgent) { tabs.TabPages.Add(BuildReturnsPage()); tabs.TabPages.Add(BuildWarrantyPage()); }
        page.Controls.Add(tabs); return page;
    }

    private TabPage BuildCustomersPage()
    {
        var page = new TabPage("Customers"); var grid = CreateGrid(); var top = new FlowLayoutPanel { Dock=DockStyle.Top, Height=42, Padding=new Padding(6) }; var status = new ComboBox { Width=110, DropDownStyle=ComboBoxStyle.DropDownList }; status.Items.AddRange(["Active", "Suspended"]); status.SelectedIndex=0; var refresh = new Button { Text="Refresh" }; var update = new Button { Text="Update selected" }; Action load = () => grid.DataSource=advanced.Customers(currentUser!); refresh.Click += (_,_)=>load(); update.Click += (_,_)=> { if(grid.CurrentRow?.DataBoundItem is CustomerRecord customer) { try { advanced.SetCustomerStatus(currentUser!,customer.Id,status.Text); load(); } catch(Exception ex) { MessageBox.Show(ex.Message); } } }; top.Controls.Add(refresh); top.Controls.Add(status); top.Controls.Add(update); page.Controls.Add(grid); page.Controls.Add(top); load(); return page;
    }

    private TabPage BuildSuppliersPage()
    {
        var page=new TabPage("Suppliers"); var grid=CreateGrid(); var top=new FlowLayoutPanel { Dock=DockStyle.Top, Height=42, Padding=new Padding(6) }; var name=new TextBox { PlaceholderText="Supplier name", Width=150 }; var contact=new TextBox { PlaceholderText="Contact", Width=130 }; var email=new TextBox { PlaceholderText="Email", Width=170 }; var phone=new TextBox { PlaceholderText="Phone", Width=110 }; var save=new Button { Text="Save supplier" }; Action load=()=>grid.DataSource=advanced.Suppliers(currentUser!); save.Click+=(_,_)=>{try{advanced.SaveSupplier(currentUser!,new SupplierRecord{Name=name.Text,ContactName=contact.Text,Email=email.Text,Phone=phone.Text});load();}catch(Exception ex){MessageBox.Show(ex.Message);}}; top.Controls.AddRange([name,contact,email,phone,save]); page.Controls.Add(grid); page.Controls.Add(top); load(); return page;
    }

    private TabPage BuildCouponsPage()
    {
        var page=new TabPage("Coupons"); var grid=CreateGrid(); var top=new FlowLayoutPanel { Dock=DockStyle.Top, Height=42, Padding=new Padding(6) }; var code=new TextBox { PlaceholderText="Code", Width=100 }; var type=new ComboBox { Width=100, DropDownStyle=ComboBoxStyle.DropDownList }; type.Items.AddRange(["Percentage","Fixed"]); type.SelectedIndex=0; var value=new NumericUpDown { Maximum=100000, DecimalPlaces=2, Width=90 }; var minimum=new NumericUpDown { Maximum=100000, DecimalPlaces=2, Width=90 }; var save=new Button { Text="Save coupon" }; Action load=()=>grid.DataSource=advanced.Coupons(currentUser!); save.Click+=(_,_)=>{try{advanced.SaveCoupon(currentUser!,new CouponRecord{Code=code.Text,DiscountType=type.Text,Value=value.Value,MinimumOrderAmount=minimum.Value,ValidFrom=DateTime.Now,ValidUntil=DateTime.Now.AddDays(30)});load();}catch(Exception ex){MessageBox.Show(ex.Message);}}; top.Controls.AddRange([code,type,value,minimum,save]); top.Controls.Add(new Label { Text="Valid for 30 days; value and minimum order", AutoSize=true, Padding=new Padding(5,7,0,0) }); page.Controls.Add(grid); page.Controls.Add(top); load(); return page;
    }

    private TabPage BuildTicketsPage()
    {
        var page=new TabPage("Support Tickets"); var grid=CreateGrid(); var top=new FlowLayoutPanel { Dock=DockStyle.Top, Height=42, Padding=new Padding(6) }; var status=new ComboBox { Width=110, DropDownStyle=ComboBoxStyle.DropDownList }; status.Items.AddRange(["Open","In Progress","Closed"]); status.SelectedIndex=0; var update=new Button { Text="Update selected" }; Action load=()=>grid.DataSource=advanced.Tickets(currentUser!); update.Click+=(_,_)=>{if(grid.CurrentRow?.DataBoundItem is SupportTicketRecord ticket){try{advanced.UpdateTicket(currentUser!,ticket.Id,status.Text);load();}catch(Exception ex){MessageBox.Show(ex.Message);}}}; top.Controls.Add(status);top.Controls.Add(update);page.Controls.Add(grid);page.Controls.Add(top);load();return page;
    }

    private TabPage BuildReturnsPage()
    {
        var page=new TabPage("Returns"); var grid=CreateGrid(); var top=new FlowLayoutPanel { Dock=DockStyle.Top, Height=42, Padding=new Padding(6) }; var status=new ComboBox { Width=110, DropDownStyle=ComboBoxStyle.DropDownList }; status.Items.AddRange(["Approved","Rejected","Completed"]); status.SelectedIndex=0; var update=new Button { Text="Update selected" }; Action load=()=>grid.DataSource=advanced.Returns(currentUser!); update.Click+=(_,_)=>{if(grid.CurrentRow?.DataBoundItem is ReturnRecord item){try{advanced.UpdateReturn(currentUser!,item.Id,status.Text);load();}catch(Exception ex){MessageBox.Show(ex.Message);}}};top.Controls.Add(status);top.Controls.Add(update);page.Controls.Add(grid);page.Controls.Add(top);load();return page;
    }

    private TabPage BuildWarrantyPage()
    {
        var page=new TabPage("Warranty Claims"); var grid=CreateGrid(); var top=new FlowLayoutPanel { Dock=DockStyle.Top, Height=42, Padding=new Padding(6) }; var status=new ComboBox { Width=110, DropDownStyle=ComboBoxStyle.DropDownList }; status.Items.AddRange(["Received","Diagnosing","Repairing","Replaced","Rejected","Completed"]); status.SelectedIndex=0; var update=new Button { Text="Update selected" }; Action load=()=>grid.DataSource=advanced.WarrantyClaims(currentUser!); update.Click+=(_,_)=>{if(grid.CurrentRow?.DataBoundItem is WarrantyClaimRecord claim){try{advanced.UpdateWarranty(currentUser!,claim.Id,status.Text);load();}catch(Exception ex){MessageBox.Show(ex.Message);}}};top.Controls.Add(status);top.Controls.Add(update);page.Controls.Add(grid);page.Controls.Add(top);load();return page;
    }

    private TabPage BuildAuditPage()
    {
        var page=new TabPage("Audit Log"); var grid=CreateGrid(); var refresh=new Button { Text="Refresh", Dock=DockStyle.Top, Height=32 }; Action load=()=>grid.DataSource=advanced.AuditLogs(currentUser!); refresh.Click+=(_,_)=>load(); page.Controls.Add(grid);page.Controls.Add(refresh);load();return page;
    }

    private void RefreshAll() { RefreshProducts(); RefreshCart(); RefreshOrders(); }
    private void RefreshProducts() { if (productGrid is null) return; productGrid.DataSource = products.Search(searchBox?.Text); }
    private void RefreshCart() { if (cartGrid is null) return; cartGrid.DataSource = sales.Cart(currentUser!); }
    private void RefreshOrders() { if (orderGrid is null) return; orderGrid.DataSource = sales.Orders(currentUser!); }
    private void AddSelectedToCart() { if (productGrid.CurrentRow?.DataBoundItem is not Product p) return; try { sales.AddToCart(currentUser!, p.Id, 1); RefreshCart(); MessageBox.Show("Added to cart."); } catch (Exception ex) { MessageBox.Show(ex.Message); } }
    private void RemoveSelectedCart() { if (cartGrid.CurrentRow?.DataBoundItem is CartItem item) { sales.RemoveFromCart(currentUser!, item.ProductId); RefreshCart(); } }
    private void Checkout() { try { var number = sales.Checkout(currentUser!, paymentSelector.SelectedItem?.ToString() ?? "Card"); RefreshAll(); MessageBox.Show($"Order {number} was placed successfully."); } catch (Exception ex) { MessageBox.Show(ex.Message, "Checkout", MessageBoxButtons.OK, MessageBoxIcon.Warning); } }
    private static DataGridView CreateGrid() => new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
}
