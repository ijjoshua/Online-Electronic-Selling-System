using OnlineElectronicSellingSystem.Controllers;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;
using OnlineElectronicSellingSystem.Views.Components;

namespace OnlineElectronicSellingSystem;

public sealed class MainForm : Form
{
    private static readonly Color Navy = OessTheme.SidebarBackground;
    private static readonly Color RoyalBlue = OessTheme.PrimaryBlue;
    private static readonly Color Background = OessTheme.PageBackground;
    private readonly User currentUser;
    private readonly ReportsController reports;
    private readonly ProductController products;
    private readonly SalesController sales;
    private readonly CatalogController catalog;
    private readonly UserManagementController userManagement;
    private readonly InventoryController inventory;
    private readonly WishlistController wishlist;
    private readonly PaymentController payments;
    private readonly ShipmentController shipments;
    private readonly AfterSalesController afterSales;
    private readonly ReviewController reviewController;
    private readonly SupplierController supplierController;
    private readonly SupportController supportController;
    private readonly SettingsController settingsController;
    private readonly ProductDetailController productDetails;
    private readonly AddressController addressController;
    private readonly ReportManagementController reportManagement;
    private readonly PromotionController promotionController;
    private readonly CouponController couponController;
    private readonly AdvancedController advancedController;
    private readonly Panel content = new() { Dock = DockStyle.Fill, BackColor = Background, Padding = new Padding(24), AutoScroll = true };
    private readonly Label pageTitle = new() { Dock = DockStyle.Top, Height = 34, AutoEllipsis = true, Font = OessTheme.PageTitleFont, ForeColor = OessTheme.MainText };
    private readonly Label breadcrumb = new() { Dock = DockStyle.Top, Height = 22, AutoEllipsis = true, ForeColor = OessTheme.SecondaryText, Font = OessTheme.SmallFont };

    public MainForm(User user, Database database)
    {
        currentUser = user;
        reports = new ReportsController(new OessRepository(database));
        products = new ProductController(new OessRepository(database));
        sales = new SalesController(new OessRepository(database));
        catalog = new CatalogController(new CatalogRepository(database));
        userManagement = new UserManagementController(new UserManagementRepository(database));
        inventory = new InventoryController(new InventoryRepository(database));
        wishlist = new WishlistController(new WishlistRepository(database));
        payments = new PaymentController(new PaymentRepository(database));
        shipments = new ShipmentController(new ShipmentRepository(database));
        afterSales = new AfterSalesController(new AfterSalesRepository(database));
        reviewController = new ReviewController(new ReviewRepository(database));
        supplierController = new SupplierController(new SupplierRepository(database));
        supportController = new SupportController(new SupportRepository(database));
        settingsController = new SettingsController(new SettingsRepository(database));
        productDetails = new ProductDetailController(new ProductDetailRepository(database));
        addressController = new AddressController(new AddressRepository(database));
        reportManagement = new ReportManagementController(new ReportRepository(database));
        promotionController = new PromotionController(new PromotionRepository(database));
        couponController = new CouponController(new CouponRepository(database));
        advancedController = new AdvancedController(new AdvancedRepository(database));
        Text = "OESS";
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(1000, 650);
        content.ControlAdded += (_, e) => StylePageControl(e.Control!);
        BuildShell();
        ShowDashboard();
    }

    private void BuildShell()
    {
        var sidebar = new Panel { Dock = DockStyle.Left, Width = 238, BackColor = Navy, Padding = new Padding(12, 16, 12, 12) };
        var logo = new Label { Text = "OESS\nONLINE ELECTRONIC\nSELLING SYSTEM", Dock = DockStyle.Top, Height = 82, ForeColor = Color.White, Font = new Font("Segoe UI", 11, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
        sidebar.Controls.Add(logo);
        var nav = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(0, 8, 0, 0), AutoScroll = true };
        AddNavSection(nav, "Workspace");
        AddNavButton(nav, "Dashboard", ShowDashboard);
        AddNavSection(nav, "Products");
        AddNavButton(nav, "Catalogue", ShowCatalogue);
        if (currentUser.Role is UserRole.Admin or UserRole.Seller) AddNavButton(nav, "Manage products", ShowProductManagement);
        if (currentUser.Role is UserRole.Admin or UserRole.Seller) AddNavButton(nav, "Product details", ShowProductDetails);
        if (currentUser.Role is UserRole.Admin or UserRole.Seller) AddNavButton(nav, "Brands & categories", ShowCatalogManagement);
        AddNavSection(nav, "Sales");
        AddNavButton(nav, "Shopping cart", ShowCart);
        AddNavButton(nav, "Wishlist", ShowWishlist);
        AddNavButton(nav, "Orders", ShowOrders);
        if (currentUser.Role == UserRole.Admin) AddNavButton(nav, "Customers & users", ShowUserManagement);
        if (currentUser.Role is UserRole.Admin or UserRole.Seller or UserRole.SalesOfficer) AddNavButton(nav, "Reports", ShowReports);
        if (currentUser.Role is UserRole.Admin or UserRole.SalesOfficer) AddNavButton(nav, "Payments & invoices", ShowPayments);
        AddNavSection(nav, "Inventory");
        if (currentUser.Role is UserRole.Admin or UserRole.Seller) AddNavButton(nav, "Inventory", ShowInventory);
        if (currentUser.Role is UserRole.Admin or UserRole.Seller) AddNavButton(nav, "Suppliers & orders", ShowSuppliers);
        AddNavSection(nav, "Delivery & support");
        if (currentUser.Role is UserRole.Admin or UserRole.DeliveryAgent or UserRole.SalesOfficer) AddNavButton(nav, "Shipments", ShowShipments);
        if (currentUser.Role is UserRole.Admin or UserRole.SupportAgent or UserRole.SalesOfficer) AddNavButton(nav, "Returns & warranty", ShowAfterSales);
        if (currentUser.Role is UserRole.Admin or UserRole.Seller or UserRole.SupportAgent) AddNavButton(nav, "Reviews & questions", ShowReviews);
        if (currentUser.Role is UserRole.Admin or UserRole.SupportAgent or UserRole.SalesOfficer) AddNavButton(nav, "Support tickets", ShowSupport);
        AddNavButton(nav, "Notifications", ShowNotifications);
        AddNavSection(nav, "Administration");
        if (currentUser.Role == UserRole.Admin) AddNavButton(nav, "Settings", ShowSettings);
        if (currentUser.Role == UserRole.Admin) AddNavButton(nav, "Promotions", ShowPromotions);
        if (currentUser.Role == UserRole.Admin) AddNavButton(nav, "Coupons", ShowCoupons);
        if (currentUser.Role == UserRole.Admin) AddNavButton(nav, "Audit logs", ShowAuditLogs);
        AddNavButton(nav, "Addresses", ShowAddresses);
        sidebar.Controls.Add(nav);
        var profile = new Panel { Dock = DockStyle.Bottom, Height = 70, BackColor = Color.FromArgb(29, 42, 65), Padding = new Padding(10, 8, 10, 8) };
        var signOutButton = new Button
        {
            Text = "  Sign out",
            Dock = DockStyle.Bottom,
            Height = 40,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(142, 45, 54),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            FlatAppearance = { BorderSize = 0 }
        };
        signOutButton.Click += (_, _) => SignOut();
        profile.Controls.Add(signOutButton);
        sidebar.Controls.Add(profile);

        var top = new Panel { Dock = DockStyle.Top, Height = 84, BackColor = Color.White, Padding = new Padding(24, 12, 24, 12) };
        var titleStack = new Panel { Dock = DockStyle.Left, Width = 430, Padding = new Padding(0, 0, 12, 0) };
        titleStack.Controls.Add(breadcrumb);
        titleStack.Controls.Add(pageTitle);
        top.Controls.Add(titleStack);
        var headerProfile = OessTheme.CreateProfileHeader(currentUser.FullName, currentUser.Role.ToString());
        headerProfile.Dock = DockStyle.Right;
        headerProfile.Width = 190;
        var search = new TextBox { Width = 220, Height = 34, PlaceholderText = "Search workspace...", Anchor = AnchorStyles.Top | AnchorStyles.Right, Location = new Point(0, 25) };
        var dateLabel = new Label { Text = DateTime.Now.ToString("ddd, MMM d, yyyy"), AutoSize = false, Width = 145, Dock = DockStyle.Right, ForeColor = OessTheme.SecondaryText, TextAlign = ContentAlignment.MiddleCenter, Padding = new Padding(8, 14, 0, 0) };
        top.Controls.Add(search);
        top.Resize += (_, _) => search.Left = Math.Max(titleStack.Right + 24, top.ClientSize.Width - search.Width - headerProfile.Width - dateLabel.Width - 16);
        top.Controls.Add(headerProfile);
        top.Controls.Add(dateLabel);
        top.PerformLayout();
        search.Left = Math.Max(titleStack.Right + 24, top.ClientSize.Width - search.Width - headerProfile.Width - dateLabel.Width - 16);
        Controls.Add(content); Controls.Add(top); Controls.Add(sidebar);
    }

    private void SignOut()
    {
        var result = MessageBox.Show(
            "Are you sure you want to sign out?",
            "Sign out",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2);

        if (result == DialogResult.Yes)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }
    }

    private static void AddNavButton(FlowLayoutPanel nav, string text, Action action)
    {
        var button = new Button { Text = text, Width = 214, Height = 36, FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderSize = 0 }, BackColor = Navy, ForeColor = Color.FromArgb(215, 224, 239), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 4, 0), Font = new Font("Segoe UI", 9.25f) };
        button.MouseEnter += (_, _) => button.BackColor = Color.FromArgb(38, 53, 78);
        button.MouseLeave += (_, _) => button.BackColor = Navy;
        button.Click += (_, _) => { button.BackColor = Color.FromArgb(53, 106, 230); action(); };
        nav.Controls.Add(button);
    }

    private static void AddNavSection(FlowLayoutPanel nav, string text)
    {
        nav.Controls.Add(OessTheme.CreateNavigationSection(text));
    }

    private void PreparePage(string title)
    {
        pageTitle.Text = title;
        breadcrumb.Text = $"Workspace  /  {title}";
        content.Controls.Clear();
    }

    private static void StylePageControl(Control control)
    {
        StylePageControlRecursive(control);
    }

    private static void StylePageControlRecursive(Control control)
    {
        if (control is FlowLayoutPanel toolbar) OessTheme.StyleToolbar(toolbar);
        else if (control is TabControl tabs)
        {
            tabs.Font = OessTheme.BodyFont;
            tabs.Padding = new Point(14, 6);
            tabs.BackColor = OessTheme.PageBackground;
        }
        else if (control is TabPage tabPage)
        {
            tabPage.BackColor = OessTheme.PageBackground;
            tabPage.Padding = new Padding(8);
        }
        else StyleInputControl(control);

        foreach (Control child in control.Controls) StylePageControlRecursive(child);
    }

    private static void StyleInputControl(Control control)
    {
        if (control is Button button)
        {
            OessTheme.StyleButton(button);
            return;
        }
        OessTheme.StyleInput(control);
        if (control is Label label) label.ForeColor = OessTheme.SecondaryText;
        if (control is FlowLayoutPanel nestedToolbar) OessTheme.StyleToolbar(nestedToolbar);
        return;
        /*
        switch (control)
        {
            case Button button:
                button.Height = Math.Max(button.Height, 34);
                button.Padding = new Padding(12, 0, 12, 0);
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = Color.FromArgb(210, 218, 230);
                button.FlatAppearance.BorderSize = 1;
                button.BackColor = RoyalBlue;
                button.ForeColor = Color.White;
                button.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                break;
            case TextBox textBox:
                textBox.Height = 34;
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.Font = new Font("Segoe UI", 9.5f);
                break;
            case ComboBox comboBox:
                comboBox.Height = 34;
                comboBox.Font = new Font("Segoe UI", 9.5f);
                break;
            case NumericUpDown numeric:
                numeric.Height = 34;
                numeric.Font = new Font("Segoe UI", 9.5f);
                break;
            case CheckBox checkBox:
                checkBox.Margin = new Padding(8, 8, 8, 0);
                break;
            case Label label:
                label.ForeColor = Color.FromArgb(75, 85, 99);
                break;
        }
        */
    }

    private void ShowDashboard()
    {
        PreparePage("Dashboard");
        var welcomeHeader = new Panel { Dock = DockStyle.Top, Height = 76, BackColor = Background, Padding = new Padding(6, 0, 6, 12) };
        var greeting = DateTime.Now.Hour < 12 ? "Good morning" : DateTime.Now.Hour < 18 ? "Good afternoon" : "Good evening";
        welcomeHeader.Controls.Add(new Label { Text = $"{greeting}, {currentUser.FullName}", AutoSize = true, Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = OessTheme.MainText });
        welcomeHeader.Controls.Add(new Label { Text = $"Here is your OESS workspace overview for {DateTime.Today:MMMM d, yyyy}.", AutoSize = true, Location = new Point(2, 34), Font = OessTheme.BodyFont, ForeColor = OessTheme.SecondaryText });
        content.Controls.Add(welcomeHeader);
        if (currentUser.Role == UserRole.Customer)
        {
            content.Controls.Add(new Label { Text = "Browse the catalogue or open your shopping cart to continue.", AutoSize = true, Font = OessTheme.SectionTitleFont, ForeColor = OessTheme.MainText, Padding = new Padding(8, 24, 0, 0) });
            return;
        }
        try
        {
            var summary = reports.Dashboard(currentUser);
            var layout = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 4, RowCount = 5, BackColor = Background, Padding = new Padding(0, 4, 0, 0) };
            for (var i = 0; i < 4; i++) layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            var salesHeader = OessTheme.CreateDashboardSection("Sales overview");
            layout.Controls.Add(salesHeader, 0, 0); layout.SetColumnSpan(salesHeader, 4);
            AddCard(layout, "Orders today", summary.OrdersToday.ToString(), RoyalBlue, 0, 1);
            AddCard(layout, "Revenue today", summary.RevenueToday.ToString("C"), OessTheme.Success, 1, 1);
            AddCard(layout, "Pending orders", summary.PendingOrders.ToString(), OessTheme.Warning, 2, 1);
            AddCard(layout, "Total sales", summary.TotalSales.ToString("C"), OessTheme.Info, 3, 1);
            var operationsHeader = OessTheme.CreateDashboardSection("Operations overview");
            layout.Controls.Add(operationsHeader, 0, 2); layout.SetColumnSpan(operationsHeader, 4);
            AddCard(layout, "Registered customers", summary.Customers.ToString(), Color.FromArgb(111, 78, 170), 0, 3);
            AddCard(layout, "Total products", summary.TotalProducts.ToString(), RoyalBlue, 1, 3);
            AddCard(layout, "Low-stock products", summary.LowStockProducts.ToString(), OessTheme.Danger, 2, 3);
            AddCard(layout, "Pending support", summary.PendingSupportTickets.ToString(), OessTheme.Warning, 3, 3);
            var welcome = new Label { Text = "Select a module from the navigation panel to manage your store.", AutoSize = true, Font = OessTheme.BodyFont, ForeColor = OessTheme.SecondaryText, Padding = new Padding(8, 12, 0, 0) };
            layout.Controls.Add(welcome, 0, 4); layout.SetColumnSpan(welcome, 4);
            content.Controls.Add(layout);
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private static void AddCard(TableLayoutPanel layout, string title, string value, Color accent, int column, int row)
    {
        var card = OessTheme.CreateCard(title, value, accent);
        layout.Controls.Add(card, column, row);
    }

    private void ShowCatalogue()
    {
        PreparePage("Catalogue");
        AddPageDescription("Browse products, add items to your cart, or save them for later.");
        var grid = CreateGrid();
        var search = new TextBox { Width = 280, PlaceholderText = "Search products..." };
        var refresh = new Button { Text = "Refresh", AutoSize = true };
        var quantity = new NumericUpDown { Minimum = 1, Maximum = 100, Value = 1, Width = 60 };
        var add = new Button { Text = "Add selected to cart", AutoSize = true };
        var save = new Button { Text = "Add to wishlist", AutoSize = true };
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 58, FlowDirection = FlowDirection.LeftToRight };
        toolbar.Controls.Add(search); toolbar.Controls.Add(refresh); toolbar.Controls.Add(new Label { Text = "Quantity", AutoSize = true, Padding = new Padding(12, 7, 0, 0) }); toolbar.Controls.Add(quantity); toolbar.Controls.Add(add); toolbar.Controls.Add(save);
        void LoadProducts()
        {
            try { grid.DataSource = products.Search(search.Text); }
            catch (Exception ex) { ShowError(ex); }
        }
        refresh.Click += (_, _) => LoadProducts();
        search.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) LoadProducts(); };
        add.Click += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not Product product) { ShowError(new InvalidOperationException("Select a product first.")); return; }
            try { sales.AddToCart(currentUser, product.Id, (int)quantity.Value); MessageBox.Show("Product added to cart.", "OESS", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            catch (Exception ex) { ShowError(ex); }
        };
        save.Click += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not Product product) { ShowError(new InvalidOperationException("Select a product first.")); return; }
            try { wishlist.Add(currentUser, product.Id); MessageBox.Show("Product added to wishlist.", "OESS", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            catch (Exception ex) { ShowError(ex); }
        };
        content.Controls.Add(grid); content.Controls.Add(toolbar); LoadProducts();
    }

    private void ShowProductManagement()
    {
        PreparePage("Product management");
        AddPageDescription("Maintain product pricing, stock levels, warranty information, and publishing status.");
        var grid = CreateGrid();
        var sku = new TextBox { PlaceholderText = "SKU", Width = 100 };
        var name = new TextBox { PlaceholderText = "Product name", Width = 170 };
        var category = new TextBox { PlaceholderText = "Category", Width = 110 };
        var price = new NumericUpDown { DecimalPlaces = 2, Maximum = 10000000, Width = 100 };
        var stock = new NumericUpDown { Maximum = 1000000, Width = 80 };
        var reorder = new NumericUpDown { Maximum = 1000000, Width = 75, Value = 5 };
        var warranty = new NumericUpDown { Maximum = 120, Width = 75 };
        var status = new ComboBox { Width = 110, DropDownStyle = ComboBoxStyle.DropDownList };
        status.Items.AddRange(["Published", "Draft", "Archived"]); status.SelectedIndex = 0;
        var active = new CheckBox { Text = "Active", Checked = true, AutoSize = true };
        var featured = new CheckBox { Text = "Featured", AutoSize = true };
        var newArrival = new CheckBox { Text = "New arrival", AutoSize = true };
        var save = new Button { Text = "Save product", AutoSize = true };
        var clear = new Button { Text = "Clear", AutoSize = true };
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 88, WrapContents = true };
        toolbar.Controls.AddRange([sku, name, category, price, stock, reorder, warranty, status, active, featured, newArrival, save, clear]);

        void ClearFields()
        {
            sku.Clear(); name.Clear(); category.Clear(); price.Value = 0; stock.Value = 0; reorder.Value = 5; warranty.Value = 0;
            status.SelectedIndex = 0; active.Checked = true; featured.Checked = false; newArrival.Checked = false; grid.ClearSelection();
        }
        void LoadProducts()
        {
            try { grid.DataSource = products.Search(null); }
            catch (Exception ex) { ShowError(ex); }
        }
        grid.SelectionChanged += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not Product item) return;
            sku.Text = item.Sku; name.Text = item.Name; category.Text = item.Category; price.Value = item.Price; stock.Value = item.StockQuantity;
            reorder.Value = item.ReorderLevel; warranty.Value = item.WarrantyMonths; status.Text = item.Status; active.Checked = item.IsActive;
            featured.Checked = item.IsFeatured; newArrival.Checked = item.IsNewArrival;
        };
        save.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(sku.Text) || string.IsNullOrWhiteSpace(name.Text) || string.IsNullOrWhiteSpace(category.Text)) { ShowError(new ArgumentException("SKU, name, and category are required.")); return; }
            try
            {
                products.Save(new Product { Sku = sku.Text.Trim(), Name = name.Text.Trim(), Category = category.Text.Trim(), Price = price.Value, StockQuantity = (int)stock.Value, ReorderLevel = (int)reorder.Value, WarrantyMonths = (int)warranty.Value, Status = status.Text, IsActive = active.Checked, IsFeatured = featured.Checked, IsNewArrival = newArrival.Checked }, currentUser);
                LoadProducts(); ClearFields();
                MessageBox.Show("Product saved.", "OESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { ShowError(ex); }
        };
        clear.Click += (_, _) => ClearFields();
        content.Controls.Add(grid); content.Controls.Add(toolbar); LoadProducts();
    }

    private void AddPageDescription(string description)
    {
        content.Controls.Add(new Label
        {
            Text = description,
            Dock = DockStyle.Top,
            MinimumSize = new Size(0, 36),
            AutoSize = true,
            ForeColor = OessTheme.SecondaryText,
            Font = OessTheme.BodyFont,
            Padding = new Padding(4, 2, 12, 10)
        });
    }

    private void ShowProductDetails()
    {
        PreparePage("Product variants, specifications and images");
        var tabs=new TabControl{Dock=DockStyle.Fill};
        var variants=new TabPage("Variants");var variantGrid=CreateGrid();var variantRefresh=new Button{Text="Refresh",AutoSize=true};var variantToolbar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=48};variantToolbar.Controls.Add(variantRefresh);variantRefresh.Click+=(_,_)=>{try{variantGrid.DataSource=productDetails.Variants(currentUser);}catch(Exception ex){ShowError(ex);}};variants.Controls.Add(variantGrid);variants.Controls.Add(variantToolbar);
        var specs=new TabPage("Specifications");var specGrid=CreateGrid();var productId=new NumericUpDown{Minimum=1,Maximum=1000000,Width=80};var specName=new TextBox{PlaceholderText="Name",Width=150};var specValue=new TextBox{PlaceholderText="Value",Width=200};var specSave=new Button{Text="Save specification",AutoSize=true};var specRefresh=new Button{Text="Refresh",AutoSize=true};var specToolbar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=48};specToolbar.Controls.AddRange([productId,specName,specValue,specSave,specRefresh]);void LoadSpecs(){try{specGrid.DataSource=productDetails.Specifications(currentUser);}catch(Exception ex){ShowError(ex);}}specSave.Click+=(_,_)=>{try{productDetails.AddSpecification(currentUser,(int)productId.Value,specName.Text,specValue.Text);LoadSpecs();}catch(Exception ex){ShowError(ex);}};specRefresh.Click+=(_,_)=>LoadSpecs();specs.Controls.Add(specGrid);specs.Controls.Add(specToolbar);
        var images=new TabPage("Images");var imageGrid=CreateGrid();var imageProductId=new NumericUpDown{Minimum=1,Maximum=1000000,Width=80};var imageUrl=new TextBox{PlaceholderText="Image URL",Width=280};var main=new CheckBox{Text="Main image",AutoSize=true};var order=new NumericUpDown{Minimum=0,Maximum=100000,Width=70};var imageSave=new Button{Text="Add image",AutoSize=true};var imageRefresh=new Button{Text="Refresh",AutoSize=true};var imageToolbar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=48};imageToolbar.Controls.AddRange([imageProductId,imageUrl,main,order,imageSave,imageRefresh]);void LoadImages(){try{imageGrid.DataSource=productDetails.Images(currentUser);}catch(Exception ex){ShowError(ex);}}imageSave.Click+=(_,_)=>{try{productDetails.AddImage(currentUser,(int)imageProductId.Value,imageUrl.Text,main.Checked,(int)order.Value);LoadImages();}catch(Exception ex){ShowError(ex);}};imageRefresh.Click+=(_,_)=>LoadImages();images.Controls.Add(imageGrid);images.Controls.Add(imageToolbar);
        tabs.TabPages.Add(variants);tabs.TabPages.Add(specs);tabs.TabPages.Add(images);content.Controls.Add(tabs);try{variantGrid.DataSource=productDetails.Variants(currentUser);}catch(Exception ex){ShowError(ex);}LoadSpecs();LoadImages();
    }

    private void ShowAddresses()
    {
        PreparePage("Addresses"); var grid=CreateGrid(); var recipient=new TextBox{PlaceholderText="Recipient",Width=140};var street=new TextBox{PlaceholderText="Street",Width=180};var city=new TextBox{PlaceholderText="City",Width=110};var region=new TextBox{PlaceholderText="Region",Width=100};var country=new TextBox{PlaceholderText="Country",Width=110};var phone=new TextBox{PlaceholderText="Phone",Width=110};var isDefault=new CheckBox{Text="Default",AutoSize=true};var save=new Button{Text="Add address",AutoSize=true};var refresh=new Button{Text="Refresh",AutoSize=true};var toolbar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=82,WrapContents=true};toolbar.Controls.AddRange([recipient,street,city,region,country,phone,isDefault,save,refresh]);
        void LoadAddresses(){try{grid.DataSource=addressController.Addresses(currentUser);}catch(Exception ex){ShowError(ex);}}save.Click+=(_,_)=>{try{addressController.Save(currentUser,new AddressRecord{UserId=currentUser.Id,Recipient=recipient.Text,Street=street.Text,City=city.Text,Region=region.Text,Country=country.Text,Phone=phone.Text,IsDefault=isDefault.Checked});LoadAddresses();}catch(Exception ex){ShowError(ex);}};refresh.Click+=(_,_)=>LoadAddresses();content.Controls.Add(grid);content.Controls.Add(toolbar);LoadAddresses();
    }

    private void ShowCatalogManagement()
    {
        PreparePage("Brands and categories");
        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(BuildBrandsPage());
        tabs.TabPages.Add(BuildCategoriesPage());
        content.Controls.Add(tabs);
    }

    private void ShowUserManagement()
    {
        PreparePage("User management");
        var grid = CreateGrid();
        var search = new TextBox { PlaceholderText = "Search name, email or phone", Width = 220 };
        var name = new TextBox { PlaceholderText = "Full name", Width = 150 };
        var email = new TextBox { PlaceholderText = "Email", Width = 180 };
        var phone = new TextBox { PlaceholderText = "Phone", Width = 110 };
        var password = new TextBox { PlaceholderText = "Password (new user/change only)", Width = 190, UseSystemPasswordChar = true };
        var role = new ComboBox { Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
        role.Items.AddRange(Enum.GetNames<UserRole>()); role.SelectedItem = UserRole.Customer.ToString();
        var status = new ComboBox { Width = 90, DropDownStyle = ComboBoxStyle.DropDownList };
        status.Items.AddRange(["Active", "Inactive"]); status.SelectedIndex = 0;
        var add = new Button { Text = "Add user", AutoSize = true };
        var update = new Button { Text = "Update selected", AutoSize = true };
        var updateRole = new Button { Text = "Update role", AutoSize = true };
        var refresh = new Button { Text = "Refresh", AutoSize = true };
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 105, WrapContents = true };
        toolbar.Controls.AddRange([search, name, email, phone, password, role, status, add, update, updateRole, refresh]);

        void LoadUsers()
        {
            try { grid.DataSource = userManagement.Search(currentUser, search.Text); }
            catch (Exception ex) { ShowError(ex); }
        }
        void ClearFields() { name.Clear(); email.Clear(); phone.Clear(); password.Clear(); role.SelectedItem = UserRole.Customer.ToString(); status.SelectedIndex = 0; grid.ClearSelection(); }
        grid.SelectionChanged += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not UserRecord item) return;
            name.Text = item.FullName; email.Text = item.Email; phone.Text = item.Phone ?? ""; role.Text = item.Role; status.Text = item.Status; password.Clear();
        };
        search.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) LoadUsers(); };
        add.Click += (_, _) =>
        {
            try { userManagement.Add(currentUser, new UserRecord { FullName = name.Text, Email = email.Text, Phone = phone.Text, Role = role.Text, Status = status.Text }, password.Text); LoadUsers(); ClearFields(); MessageBox.Show("User added.", "OESS", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            catch (MySqlConnector.MySqlException ex) when (ex.Number == 1062) { ShowError(new InvalidOperationException("The email or phone number is already in use.", ex)); }
            catch (Exception ex) { ShowError(ex); }
        };
        updateRole.Click += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not UserRecord item) { ShowError(new InvalidOperationException("Select a user first.")); return; }
            if (!Enum.TryParse<UserRole>(role.Text, true, out var selectedRole)) { ShowError(new ArgumentException("Select a valid role.")); return; }
            try
            {
                userManagement.UpdateRole(currentUser, item.Id, selectedRole);
                LoadUsers();
                ClearFields();
                MessageBox.Show("User role updated.", "OESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { ShowError(ex); }
        };
        update.Click += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not UserRecord item) { ShowError(new InvalidOperationException("Select a user first.")); return; }
            try { userManagement.Update(currentUser, new UserRecord { Id = item.Id, FullName = name.Text, Email = email.Text, Phone = phone.Text, Role = role.Text, Status = status.Text }, password.Text); LoadUsers(); ClearFields(); MessageBox.Show("User updated.", "OESS", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            catch (MySqlConnector.MySqlException ex) when (ex.Number == 1062) { ShowError(new InvalidOperationException("The email or phone number is already in use.", ex)); }
            catch (Exception ex) { ShowError(ex); }
        };
        refresh.Click += (_, _) => LoadUsers();
        content.Controls.Add(grid); content.Controls.Add(toolbar); LoadUsers();
    }

    private void ShowInventory()
    {
        PreparePage("Inventory");
        AddPageDescription("Monitor stock levels, make controlled adjustments, and review movement history.");
        var tabs = new TabControl { Dock = DockStyle.Fill };
        var inventoryPage = new TabPage("Stock");
        var historyPage = new TabPage("Movement history");
        var stockGrid = CreateGrid();
        var historyGrid = CreateGrid();
        var search = new TextBox { PlaceholderText = "Search SKU or product", Width = 220 };
        var quantity = new NumericUpDown { Minimum = 1, Maximum = 1000000, Width = 90, Value = 1 };
        var reason = new TextBox { PlaceholderText = "Reason", Width = 220 };
        var add = new Button { Text = "Add stock", AutoSize = true };
        var remove = new Button { Text = "Remove stock", AutoSize = true };
        var adjust = new Button { Text = "Adjust stock", AutoSize = true };
        var refresh = new Button { Text = "Refresh", AutoSize = true };
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 82, WrapContents = true };
        toolbar.Controls.AddRange([search, quantity, reason, add, remove, adjust, refresh]);
        void LoadInventory() { try { stockGrid.DataSource = inventory.Inventory(currentUser, search.Text); } catch (Exception ex) { ShowError(ex); } }
        void LoadHistory() { try { historyGrid.DataSource = inventory.Movements(currentUser); } catch (Exception ex) { ShowError(ex); } }
        void Apply(int delta, string type)
        {
            if (stockGrid.CurrentRow?.DataBoundItem is not InventoryRecord item) { ShowError(new InvalidOperationException("Select a product first.")); return; }
            if (string.IsNullOrWhiteSpace(reason.Text)) { ShowError(new ArgumentException("A stock movement reason is required.")); return; }
            try { inventory.Adjust(currentUser, item.ProductId, delta, type, reason.Text); LoadInventory(); LoadHistory(); MessageBox.Show("Stock updated.", "OESS", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            catch (Exception ex) { ShowError(ex); }
        }
        add.Click += (_, _) => Apply((int)quantity.Value, "StockIn");
        remove.Click += (_, _) => Apply(-(int)quantity.Value, "StockOut");
        adjust.Click += (_, _) => Apply((int)quantity.Value, "Adjustment");
        refresh.Click += (_, _) => { LoadInventory(); LoadHistory(); };
        search.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) LoadInventory(); };
        inventoryPage.Controls.Add(stockGrid); inventoryPage.Controls.Add(toolbar);
        historyPage.Controls.Add(historyGrid);
        tabs.TabPages.Add(inventoryPage); tabs.TabPages.Add(historyPage); content.Controls.Add(tabs);
        LoadInventory(); LoadHistory();
    }

    private TabPage BuildBrandsPage()
    {
        var page = new TabPage("Brands");
        var grid = CreateGrid();
        var name = new TextBox { PlaceholderText = "Brand name", Width = 180 };
        var logo = new TextBox { PlaceholderText = "Logo URL", Width = 220 };
        var description = new TextBox { PlaceholderText = "Description", Width = 220 };
        var active = new CheckBox { Text = "Active", Checked = true, AutoSize = true };
        var save = new Button { Text = "Save brand", AutoSize = true };
        var toggle = new Button { Text = "Activate / deactivate", AutoSize = true };
        var refresh = new Button { Text = "Refresh", AutoSize = true };
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48 };
        toolbar.Controls.AddRange([name, logo, description, active, save, toggle, refresh]);
        void LoadBrands() { try { grid.DataSource = catalog.Brands(currentUser); } catch (Exception ex) { ShowError(ex); } }
        grid.SelectionChanged += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is Brand item) { name.Text = item.Name; logo.Text = item.LogoUrl ?? ""; description.Text = item.Description ?? ""; active.Checked = item.IsActive; }
        };
        save.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(name.Text)) { ShowError(new ArgumentException("Brand name is required.")); return; }
            try { catalog.SaveBrand(currentUser, new Brand { Name = name.Text, LogoUrl = logo.Text, Description = description.Text, IsActive = active.Checked }); LoadBrands(); }
            catch (Exception ex) { ShowError(ex); }
        };
        toggle.Click += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not Brand item) { ShowError(new InvalidOperationException("Select a brand first.")); return; }
            try { catalog.SetBrandActive(currentUser, item.Id, !item.IsActive); LoadBrands(); } catch (Exception ex) { ShowError(ex); }
        };
        refresh.Click += (_, _) => LoadBrands();
        page.Controls.Add(grid); page.Controls.Add(toolbar); LoadBrands(); return page;
    }

    private TabPage BuildCategoriesPage()
    {
        var page = new TabPage("Categories");
        var grid = CreateGrid();
        var name = new TextBox { PlaceholderText = "Category name", Width = 180 };
        var description = new TextBox { PlaceholderText = "Description", Width = 220 };
        var displayOrder = new NumericUpDown { Minimum = 0, Maximum = 100000, Width = 80 };
        var active = new CheckBox { Text = "Active", Checked = true, AutoSize = true };
        var save = new Button { Text = "Save category", AutoSize = true };
        var toggle = new Button { Text = "Activate / deactivate", AutoSize = true };
        var refresh = new Button { Text = "Refresh", AutoSize = true };
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48 };
        toolbar.Controls.AddRange([name, description, displayOrder, active, save, toggle, refresh]);
        void LoadCategories() { try { grid.DataSource = catalog.Categories(currentUser); } catch (Exception ex) { ShowError(ex); } }
        grid.SelectionChanged += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is Category item) { name.Text = item.Name; description.Text = item.Description ?? ""; displayOrder.Value = item.DisplayOrder; active.Checked = item.IsActive; }
        };
        save.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(name.Text)) { ShowError(new ArgumentException("Category name is required.")); return; }
            try { catalog.SaveCategory(currentUser, new Category { Name = name.Text, Description = description.Text, DisplayOrder = (int)displayOrder.Value, IsActive = active.Checked }); LoadCategories(); }
            catch (Exception ex) { ShowError(ex); }
        };
        toggle.Click += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not Category item) { ShowError(new InvalidOperationException("Select a category first.")); return; }
            try { catalog.SetCategoryActive(currentUser, item.Id, !item.IsActive); LoadCategories(); } catch (Exception ex) { ShowError(ex); }
        };
        refresh.Click += (_, _) => LoadCategories();
        page.Controls.Add(grid); page.Controls.Add(toolbar); LoadCategories(); return page;
    }

    private void ShowCart()
    {
        PreparePage("Shopping cart");
        var grid = CreateGrid();
        var payment = new ComboBox { Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
        payment.Items.AddRange(["Card", "Mobile Money", "Bank Transfer", "Cash on Delivery"]); payment.SelectedIndex = 0;
        var refresh = new Button { Text = "Refresh", AutoSize = true };
        var remove = new Button { Text = "Remove selected", AutoSize = true };
        var checkout = new Button { Text = "Checkout", AutoSize = true };
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48 };
        toolbar.Controls.Add(refresh); toolbar.Controls.Add(remove); toolbar.Controls.Add(new Label { Text = "Payment", AutoSize = true, Padding = new Padding(12, 7, 0, 0) }); toolbar.Controls.Add(payment); toolbar.Controls.Add(checkout);
        void LoadCart() { try { grid.DataSource = sales.Cart(currentUser); } catch (Exception ex) { ShowError(ex); } }
        refresh.Click += (_, _) => LoadCart();
        remove.Click += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not CartItem item) { ShowError(new InvalidOperationException("Select a cart item first.")); return; }
            try { sales.RemoveFromCart(currentUser, item.ProductId); LoadCart(); } catch (Exception ex) { ShowError(ex); }
        };
        var quantity = new NumericUpDown { Minimum = 1, Maximum = 100000, Value = 1, Width = 75 };
        var updateQuantity = new Button { Text = "Update quantity", AutoSize = true };
        toolbar.Controls.Add(quantity); toolbar.Controls.Add(updateQuantity);
        updateQuantity.Click += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not CartItem item) { ShowError(new InvalidOperationException("Select a cart item first.")); return; }
            try { sales.UpdateCartQuantity(currentUser, item.ProductId, (int)quantity.Value); LoadCart(); } catch (Exception ex) { ShowError(ex); }
        };
        checkout.Click += (_, _) =>
        {
            if (MessageBox.Show("Place this order now?", "Confirm checkout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { var number = sales.Checkout(currentUser, payment.Text); MessageBox.Show($"Order {number} was created.", "Checkout", MessageBoxButtons.OK, MessageBoxIcon.Information); LoadCart(); } catch (Exception ex) { ShowError(ex); }
        };
        content.Controls.Add(grid); content.Controls.Add(toolbar); LoadCart();
    }

    private void ShowPayments()
    {
        PreparePage("Payments and invoices");
        AddPageDescription("Review payment activity and create invoices from completed orders.");
        var tabs = new TabControl { Dock = DockStyle.Fill };
        var paymentPage = new TabPage("Payments"); var invoicePage = new TabPage("Invoices");
        var paymentGrid = CreateGrid(); var invoiceGrid = CreateGrid();
        var status = new ComboBox { Width = 120, DropDownStyle = ComboBoxStyle.DropDownList }; status.Items.AddRange(["Pending", "Confirmed", "Failed", "Refunded"]); status.SelectedIndex = 0;
        var reference = new TextBox { PlaceholderText = "Payment reference", Width = 180 };
        var update = new Button { Text = "Update selected payment", AutoSize = true }; var refreshPayments = new Button { Text = "Refresh", AutoSize = true };
        var paymentToolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48 }; paymentToolbar.Controls.AddRange([status, reference, update, refreshPayments]);
        var invoiceNumber = new TextBox { PlaceholderText = "Invoice number", Width = 170 }; var createInvoice = new Button { Text = "Create invoice for selected payment", AutoSize = true }; var refreshInvoices = new Button { Text = "Refresh", AutoSize = true };
        var invoiceToolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48 }; invoiceToolbar.Controls.AddRange([invoiceNumber, createInvoice, refreshInvoices]);
        void LoadPayments() { try { paymentGrid.DataSource = payments.Payments(currentUser); } catch (Exception ex) { ShowError(ex); } }
        void LoadInvoices() { try { invoiceGrid.DataSource = payments.Invoices(currentUser); } catch (Exception ex) { ShowError(ex); } }
        update.Click += (_, _) =>
        {
            if (paymentGrid.CurrentRow?.DataBoundItem is not PaymentRecord item) { ShowError(new InvalidOperationException("Select a payment first.")); return; }
            try { payments.UpdateStatus(currentUser, item.Id, status.Text, reference.Text); LoadPayments(); } catch (Exception ex) { ShowError(ex); }
        };
        createInvoice.Click += (_, _) =>
        {
            if (paymentGrid.CurrentRow?.DataBoundItem is not PaymentRecord item) { ShowError(new InvalidOperationException("Select a payment first.")); return; }
            if (string.IsNullOrWhiteSpace(invoiceNumber.Text)) { ShowError(new ArgumentException("Enter an invoice number.")); return; }
            try { payments.CreateInvoice(currentUser, item.OrderId, invoiceNumber.Text); LoadInvoices(); MessageBox.Show("Invoice created.", "OESS", MessageBoxButtons.OK, MessageBoxIcon.Information); } catch (Exception ex) { ShowError(ex); }
        };
        refreshPayments.Click += (_, _) => LoadPayments(); refreshInvoices.Click += (_, _) => LoadInvoices();
        paymentPage.Controls.Add(paymentGrid); paymentPage.Controls.Add(paymentToolbar); invoicePage.Controls.Add(invoiceGrid); invoicePage.Controls.Add(invoiceToolbar);
        tabs.TabPages.Add(paymentPage); tabs.TabPages.Add(invoicePage); content.Controls.Add(tabs); LoadPayments(); LoadInvoices();
    }

    private void ShowReportsPage()
    {
        PreparePage("Reports");
        AddPageDescription("Review database-backed sales, payment, and inventory reports with CSV export.");
        var tabs=new TabControl{Dock=DockStyle.Fill}; var salesPage=new TabPage("Sales");var paymentPage=new TabPage("Payments");var inventoryPage=new TabPage("Inventory");
        var from=new DateTimePicker{Format=DateTimePickerFormat.Short,Value=DateTime.Today.AddDays(-30)};var to=new DateTimePicker{Format=DateTimePickerFormat.Short,Value=DateTime.Today};
        var salesGrid=CreateGrid();var salesRefresh=new Button{Text="Load sales",AutoSize=true};var salesExport=new Button{Text="Export CSV",AutoSize=true};var salesToolbar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=48};salesToolbar.Controls.AddRange([new Label{Text="From",AutoSize=true,Padding=new Padding(0,7,0,0)},from,new Label{Text="To",AutoSize=true,Padding=new Padding(10,7,0,0)},to,salesRefresh,salesExport]);
        void LoadSales(){try{salesGrid.DataSource=reportManagement.Sales(currentUser,from.Value.Date,to.Value.Date.AddDays(1));}catch(Exception ex){ShowError(ex);}}salesRefresh.Click+=(_,_)=>LoadSales();salesExport.Click+=(_,_)=>ExportGrid(salesGrid,"sales-report.csv");salesPage.Controls.Add(salesGrid);salesPage.Controls.Add(salesToolbar);
        var paymentGrid=CreateGrid();var paymentRefresh=new Button{Text="Load payments",AutoSize=true};var paymentToolbar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=48};paymentToolbar.Controls.Add(paymentRefresh);void LoadPaymentsReport(){try{paymentGrid.DataSource=reportManagement.Payments(currentUser,from.Value.Date,to.Value.Date.AddDays(1));}catch(Exception ex){ShowError(ex);}}paymentRefresh.Click+=(_,_)=>LoadPaymentsReport();paymentPage.Controls.Add(paymentGrid);paymentPage.Controls.Add(paymentToolbar);
        var inventoryGrid=CreateGrid();var inventoryRefresh=new Button{Text="Refresh",AutoSize=true};var inventoryToolbar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=48};inventoryToolbar.Controls.Add(inventoryRefresh);void LoadInventoryReport(){try{inventoryGrid.DataSource=reportManagement.Inventory(currentUser);}catch(Exception ex){ShowError(ex);}}inventoryRefresh.Click+=(_,_)=>LoadInventoryReport();inventoryPage.Controls.Add(inventoryGrid);inventoryPage.Controls.Add(inventoryToolbar);
        tabs.TabPages.Add(salesPage);tabs.TabPages.Add(paymentPage);tabs.TabPages.Add(inventoryPage);content.Controls.Add(tabs);LoadSales();LoadPaymentsReport();LoadInventoryReport();
    }

    private static void ExportGrid(DataGridView grid,string defaultName)
    {
        using var dialog=new SaveFileDialog{FileName=defaultName,Filter="CSV files (*.csv)|*.csv|All files (*.*)|*.*"}; if(dialog.ShowDialog()!=DialogResult.OK)return;
        var lines=new List<string>(); lines.Add(string.Join(",",grid.Columns.Cast<DataGridViewColumn>().Select(c=>$"\"{c.HeaderText.Replace("\"","\"\"")}\""))); foreach(DataGridViewRow row in grid.Rows)if(!row.IsNewRow)lines.Add(string.Join(",",row.Cells.Cast<DataGridViewCell>().Select(c=>$"\"{(c.Value?.ToString()??"").Replace("\"","\"\"")}\""))); File.WriteAllLines(dialog.FileName,lines); MessageBox.Show("Report exported.","OESS",MessageBoxButtons.OK,MessageBoxIcon.Information);
    }

    private void ShowShipments()
    {
        PreparePage("Shipments and delivery");
        AddPageDescription("Track delivery progress, agents, tracking numbers, and shipment status.");
        var grid = CreateGrid();
        var status = new ComboBox { Width = 120, DropDownStyle = ComboBoxStyle.DropDownList }; status.Items.AddRange(["Pending", "Packed", "Shipped", "OutForDelivery", "Delivered", "Failed"]); status.SelectedIndex = 0;
        var tracking = new TextBox { PlaceholderText = "Tracking number", Width = 170 };
        var note = new TextBox { PlaceholderText = "Delivery note", Width = 220 };
        var update = new Button { Text = "Update selected shipment", AutoSize = true }; var refresh = new Button { Text = "Refresh", AutoSize = true };
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48 }; toolbar.Controls.AddRange([status, tracking, note, update, refresh]);
        void LoadShipments() { try { grid.DataSource = shipments.Shipments(currentUser); } catch (Exception ex) { ShowError(ex); } }
        grid.SelectionChanged += (_, _) => { if (grid.CurrentRow?.DataBoundItem is ShipmentRecord item) { status.Text = item.Status; tracking.Text = item.TrackingNumber ?? ""; note.Text = item.DeliveryNote ?? ""; } };
        update.Click += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not ShipmentRecord item) { ShowError(new InvalidOperationException("Select a shipment first.")); return; }
            try { shipments.UpdateStatus(currentUser,item.Id,status.Text,tracking.Text,note.Text); LoadShipments(); MessageBox.Show("Shipment updated.", "OESS", MessageBoxButtons.OK, MessageBoxIcon.Information); } catch (Exception ex) { ShowError(ex); }
        };
        refresh.Click += (_, _) => LoadShipments();
        content.Controls.Add(grid); content.Controls.Add(toolbar); LoadShipments();
    }

    private void ShowAfterSales()
    {
        PreparePage("Returns and warranty");
        AddPageDescription("Manage return requests, warranty records, and customer claims through their workflow stages.");
        var tabs = new TabControl { Dock = DockStyle.Fill };

        var returnPage = new TabPage("Returns");
        var returnGrid = CreateGrid();
        var returnStatus = new ComboBox { Width = 110, DropDownStyle = ComboBoxStyle.DropDownList };
        returnStatus.Items.AddRange(["Requested", "Approved", "Rejected", "Completed"]); returnStatus.SelectedIndex = 0;
        var returnUpdate = new Button { Text = "Update return", AutoSize = true }; var returnRefresh = new Button { Text = "Refresh", AutoSize = true };
        var returnToolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48 }; returnToolbar.Controls.AddRange([returnStatus, returnUpdate, returnRefresh]);
        void LoadReturns() { try { returnGrid.DataSource = afterSales.Returns(currentUser); } catch (Exception ex) { ShowError(ex); } }
        returnGrid.SelectionChanged += (_, _) => { if (returnGrid.CurrentRow?.DataBoundItem is ReturnRecord item) returnStatus.Text = item.Status; };
        returnUpdate.Click += (_, _) => { if (returnGrid.CurrentRow?.DataBoundItem is not ReturnRecord item) { ShowError(new InvalidOperationException("Select a return first.")); return; } try { afterSales.UpdateReturn(currentUser,item.Id,returnStatus.Text); LoadReturns(); } catch(Exception ex) { ShowError(ex); } };
        returnRefresh.Click += (_, _) => LoadReturns(); returnPage.Controls.Add(returnGrid); returnPage.Controls.Add(returnToolbar);

        var warrantyPage = new TabPage("Warranties"); var warrantyGrid = CreateGrid(); var warrantyRefresh = new Button { Text = "Refresh", AutoSize = true }; var warrantyToolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48 }; warrantyToolbar.Controls.Add(warrantyRefresh);
        void LoadWarranties() { try { warrantyGrid.DataSource = afterSales.Warranties(currentUser); } catch (Exception ex) { ShowError(ex); } }
        warrantyRefresh.Click += (_, _) => LoadWarranties(); warrantyPage.Controls.Add(warrantyGrid); warrantyPage.Controls.Add(warrantyToolbar);

        var claimPage = new TabPage("Warranty claims"); var claimGrid = CreateGrid();
        var claimStatus = new ComboBox { Width = 110, DropDownStyle = ComboBoxStyle.DropDownList }; claimStatus.Items.AddRange(["Received", "InReview", "Approved", "Rejected", "Resolved"]); claimStatus.SelectedIndex = 0;
        var resolution = new TextBox { PlaceholderText = "Resolution", Width = 220 }; var claimUpdate = new Button { Text = "Update claim", AutoSize = true }; var claimRefresh = new Button { Text = "Refresh", AutoSize = true };
        var claimToolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48 }; claimToolbar.Controls.AddRange([claimStatus, resolution, claimUpdate, claimRefresh]);
        void LoadClaims() { try { claimGrid.DataSource = afterSales.Claims(currentUser); } catch (Exception ex) { ShowError(ex); } }
        claimGrid.SelectionChanged += (_, _) => { if (claimGrid.CurrentRow?.DataBoundItem is WarrantyClaimRecord item) { claimStatus.Text = item.Status; resolution.Clear(); } };
        claimUpdate.Click += (_, _) => { if (claimGrid.CurrentRow?.DataBoundItem is not WarrantyClaimRecord item) { ShowError(new InvalidOperationException("Select a warranty claim first.")); return; } try { afterSales.UpdateClaim(currentUser,item.Id,claimStatus.Text,resolution.Text); LoadClaims(); } catch(Exception ex) { ShowError(ex); } };
        claimRefresh.Click += (_, _) => LoadClaims(); claimPage.Controls.Add(claimGrid); claimPage.Controls.Add(claimToolbar);

        tabs.TabPages.Add(returnPage); tabs.TabPages.Add(warrantyPage); tabs.TabPages.Add(claimPage); content.Controls.Add(tabs);
        LoadReturns(); LoadWarranties(); LoadClaims();
    }

    private void ShowReviews()
    {
        PreparePage("Reviews and product questions");
        AddPageDescription("Moderate customer reviews and answer product questions from one workspace.");
        var tabs = new TabControl { Dock = DockStyle.Fill };
        var reviewPage = new TabPage("Reviews"); var reviewGrid = CreateGrid();
        var reviewStatus = new ComboBox { Width = 110, DropDownStyle = ComboBoxStyle.DropDownList }; reviewStatus.Items.AddRange(["Pending", "Approved", "Rejected"]); reviewStatus.SelectedIndex = 0;
        var reviewUpdate = new Button { Text = "Update review status", AutoSize = true }; var reviewRefresh = new Button { Text = "Refresh", AutoSize = true };
        var reviewToolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48 }; reviewToolbar.Controls.AddRange([reviewStatus, reviewUpdate, reviewRefresh]);
        void LoadReviews() { try { reviewGrid.DataSource = reviewController.Reviews(currentUser); } catch (Exception ex) { ShowError(ex); } }
        reviewGrid.SelectionChanged += (_, _) => { if (reviewGrid.CurrentRow?.DataBoundItem is ReviewRecord item) reviewStatus.Text = item.Status; };
        reviewUpdate.Click += (_, _) => { if (reviewGrid.CurrentRow?.DataBoundItem is not ReviewRecord item) { ShowError(new InvalidOperationException("Select a review first.")); return; } try { reviewController.UpdateReview(currentUser,item.Id,reviewStatus.Text); LoadReviews(); } catch(Exception ex) { ShowError(ex); } };
        reviewRefresh.Click += (_, _) => LoadReviews(); reviewPage.Controls.Add(reviewGrid); reviewPage.Controls.Add(reviewToolbar);

        var questionPage = new TabPage("Product questions"); var questionGrid = CreateGrid(); var answer = new TextBox { PlaceholderText = "Answer", Width = 300 }; var answerButton = new Button { Text = "Answer selected question", AutoSize = true }; var questionRefresh = new Button { Text = "Refresh", AutoSize = true }; var questionToolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48 }; questionToolbar.Controls.AddRange([answer, answerButton, questionRefresh]);
        void LoadQuestions() { try { questionGrid.DataSource = reviewController.Questions(currentUser); } catch (Exception ex) { ShowError(ex); } }
        answerButton.Click += (_, _) => { if (questionGrid.CurrentRow?.DataBoundItem is not ProductQuestionRecord item) { ShowError(new InvalidOperationException("Select a question first.")); return; } try { reviewController.Answer(currentUser,item.Id,answer.Text); LoadQuestions(); answer.Clear(); } catch(Exception ex) { ShowError(ex); } };
        questionRefresh.Click += (_, _) => LoadQuestions(); questionPage.Controls.Add(questionGrid); questionPage.Controls.Add(questionToolbar);
        tabs.TabPages.Add(reviewPage); tabs.TabPages.Add(questionPage); content.Controls.Add(tabs); LoadReviews(); LoadQuestions();
    }

    private void ShowSuppliers()
    {
        PreparePage("Suppliers and purchase orders");
        AddPageDescription("Maintain supplier records and monitor purchase-order progress.");
        var tabs = new TabControl { Dock = DockStyle.Fill };
        var supplierPage = new TabPage("Suppliers"); var supplierGrid = CreateGrid();
        var name = new TextBox { PlaceholderText = "Supplier name", Width = 150 }; var contact = new TextBox { PlaceholderText = "Contact", Width = 120 }; var email = new TextBox { PlaceholderText = "Email", Width = 170 }; var phone = new TextBox { PlaceholderText = "Phone", Width = 110 }; var address = new TextBox { PlaceholderText = "Address", Width = 180 }; var active = new CheckBox { Text = "Active", Checked = true, AutoSize = true }; var save = new Button { Text = "Save supplier", AutoSize = true }; var refresh = new Button { Text = "Refresh", AutoSize = true };
        var supplierToolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 82, WrapContents = true }; supplierToolbar.Controls.AddRange([name,contact,email,phone,address,active,save,refresh]);
        void LoadSuppliers(){try{supplierGrid.DataSource=supplierController.Suppliers(currentUser);}catch(Exception ex){ShowError(ex);}}
        supplierGrid.SelectionChanged += (_,_)=>{if(supplierGrid.CurrentRow?.DataBoundItem is SupplierManagementRecord s){name.Text=s.Name;contact.Text=s.ContactName??"";email.Text=s.Email??"";phone.Text=s.Phone??"";address.Text=s.Address??"";active.Checked=s.IsActive;}};
        save.Click += (_,_)=>{try{supplierController.Save(currentUser,new SupplierManagementRecord{Name=name.Text,ContactName=contact.Text,Email=email.Text,Phone=phone.Text,Address=address.Text,IsActive=active.Checked});LoadSuppliers();}catch(Exception ex){ShowError(ex);}}; refresh.Click+=(_,_)=>LoadSuppliers(); supplierPage.Controls.Add(supplierGrid);supplierPage.Controls.Add(supplierToolbar);
        var poPage = new TabPage("Purchase orders"); var poGrid=CreateGrid(); var poSupplier=new NumericUpDown{Minimum=1,Maximum=1000000,Width=75};var poNumber=new TextBox{PlaceholderText="PO number",Width=140};var poProduct=new NumericUpDown{Minimum=1,Maximum=1000000,Width=75};var poQuantity=new NumericUpDown{Minimum=1,Maximum=1000000,Width=75,Value=1};var poCost=new NumericUpDown{Minimum=0,Maximum=10000000,DecimalPlaces=2,Width=90};var poNotes=new TextBox{PlaceholderText="Notes",Width=150};var poCreate=new Button{Text="Create purchase order",AutoSize=true};var poStatus=new ComboBox{Width=110,DropDownStyle=ComboBoxStyle.DropDownList};poStatus.Items.AddRange(["Draft","Ordered","Received","Cancelled"]);poStatus.SelectedIndex=0;var poUpdate=new Button{Text="Update selected order",AutoSize=true};var poRefresh=new Button{Text="Refresh",AutoSize=true};var poToolbar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=82,WrapContents=true};poToolbar.Controls.AddRange([poSupplier,poNumber,poProduct,poQuantity,poCost,poNotes,poCreate,poStatus,poUpdate,poRefresh]);
        void LoadOrders(){try{poGrid.DataSource=supplierController.PurchaseOrders(currentUser);}catch(Exception ex){ShowError(ex);}} poGrid.SelectionChanged+=(_,_)=>{if(poGrid.CurrentRow?.DataBoundItem is PurchaseOrderRecord p)poStatus.Text=p.Status;};poCreate.Click+=(_,_)=>{try{supplierController.CreatePurchaseOrder(currentUser,(int)poSupplier.Value,poNumber.Text,(int)poProduct.Value,(int)poQuantity.Value,poCost.Value,poNotes.Text);LoadOrders();}catch(Exception ex){ShowError(ex);}};poUpdate.Click+=(_,_)=>{if(poGrid.CurrentRow?.DataBoundItem is not PurchaseOrderRecord p){ShowError(new InvalidOperationException("Select a purchase order first."));return;}try{supplierController.UpdatePurchaseOrderStatus(currentUser,p.Id,poStatus.Text);LoadOrders();}catch(Exception ex){ShowError(ex);}};poRefresh.Click+=(_,_)=>LoadOrders();poPage.Controls.Add(poGrid);poPage.Controls.Add(poToolbar);
        tabs.TabPages.Add(supplierPage);tabs.TabPages.Add(poPage);content.Controls.Add(tabs);LoadSuppliers();LoadOrders();
    }

    private void ShowSupport()
    {
        PreparePage("Support tickets");
        AddPageDescription("Review customer requests, update ticket status, and respond from one organized workspace.");
        var ticketGrid = CreateGrid();
        var messageGrid = CreateGrid();
        var status = new ComboBox { Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
        status.Items.AddRange(["Open", "InProgress", "Resolved", "Closed"]);
        status.SelectedIndex = 0;
        var update = new Button { Text = "Update status", AutoSize = true };
        var refresh = new Button { Text = "Refresh tickets", AutoSize = true };
        var ticketToolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 54 };
        ticketToolbar.Controls.AddRange([new Label { Text = "Status", AutoSize = true, Padding = new Padding(4, 10, 0, 0) }, status, update, refresh]);
        var message = new TextBox { PlaceholderText = "Write a reply to the selected ticket...", Width = 300 };
        var reply = new Button { Text = "Send reply", AutoSize = true };
        var messageRefresh = new Button { Text = "Refresh messages", AutoSize = true };
        var messageToolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 54 };
        messageToolbar.Controls.AddRange([message, reply, messageRefresh]);
        void LoadTickets()
        {
            try
            {
                ticketGrid.DataSource = supportController.Tickets(currentUser);
                if (ticketGrid.Rows.Count == 0) messageGrid.DataSource = null;
            }
            catch (Exception ex) { ShowError(ex); }
        }
        void LoadMessages()
        {
            if (ticketGrid.CurrentRow?.DataBoundItem is SupportTicketRecord ticket)
            {
                try { messageGrid.DataSource = supportController.Messages(currentUser, ticket.Id); }
                catch (Exception ex) { ShowError(ex); }
            }
            else messageGrid.DataSource = null;
        }
        ticketGrid.SelectionChanged += (_, _) =>
        {
            if (ticketGrid.CurrentRow?.DataBoundItem is SupportTicketRecord ticket)
            {
                status.Text = ticket.Status;
                LoadMessages();
            }
        };
        update.Click += (_, _) =>
        {
            if (ticketGrid.CurrentRow?.DataBoundItem is not SupportTicketRecord ticket) { ShowError(new InvalidOperationException("Select a ticket first.")); return; }
            try { supportController.UpdateTicket(currentUser, ticket.Id, status.Text); LoadTickets(); } catch (Exception ex) { ShowError(ex); }
        };
        refresh.Click += (_, _) => LoadTickets();
        reply.Click += (_, _) =>
        {
            if (ticketGrid.CurrentRow?.DataBoundItem is not SupportTicketRecord ticket) { ShowError(new InvalidOperationException("Select a ticket first.")); return; }
            if (string.IsNullOrWhiteSpace(message.Text)) { ShowError(new ArgumentException("Enter a reply message.")); return; }
            try { supportController.AddMessage(currentUser, ticket.Id, message.Text.Trim()); message.Clear(); LoadMessages(); } catch (Exception ex) { ShowError(ex); }
        };
        messageRefresh.Click += (_, _) => LoadMessages();
        var ticketsPanel = new Panel { Dock = DockStyle.Fill, BackColor = OessTheme.CardBackground, Padding = new Padding(12) };
        var ticketsTitle = new Label { Text = "Customer requests", Dock = DockStyle.Top, Height = 30, Font = OessTheme.SectionTitleFont, ForeColor = OessTheme.MainText, Padding = new Padding(0, 0, 0, 6) };
        ticketsPanel.Controls.Add(ticketGrid);
        ticketsPanel.Controls.Add(ticketToolbar);
        ticketsPanel.Controls.Add(ticketsTitle);

        var messagesPanel = new Panel { Dock = DockStyle.Fill, BackColor = OessTheme.CardBackground, Padding = new Padding(12) };
        var messagesTitle = new Label { Text = "Ticket conversation", Dock = DockStyle.Top, Height = 30, Font = OessTheme.SectionTitleFont, ForeColor = OessTheme.MainText, Padding = new Padding(0, 0, 0, 6) };
        messagesPanel.Controls.Add(messageGrid);
        messagesPanel.Controls.Add(messageToolbar);
        messagesPanel.Controls.Add(messagesTitle);

        var workspace = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = OessTheme.PageBackground,
            Padding = new Padding(0, 0, 0, 0)
        };
        workspace.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        workspace.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        workspace.Controls.Add(ticketsPanel, 0, 0);
        workspace.Controls.Add(messagesPanel, 1, 0);
        content.Controls.Add(workspace);
        LoadTickets();
    }

    private void ShowNotifications()
    {
        PreparePage("Notifications"); var grid=CreateGrid(); var refresh=new Button{Text="Refresh",AutoSize=true};var mark=new Button{Text="Mark selected as read",AutoSize=true};var toolbar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=48};toolbar.Controls.AddRange([refresh,mark]);void LoadNotifications(){try{grid.DataSource=supportController.Notifications(currentUser);}catch(Exception ex){ShowError(ex);}}refresh.Click+=(_,_)=>LoadNotifications();mark.Click+=(_,_)=>{if(grid.CurrentRow?.DataBoundItem is not NotificationRecord n){ShowError(new InvalidOperationException("Select a notification first."));return;}try{supportController.MarkNotification(currentUser,n.Id);LoadNotifications();}catch(Exception ex){ShowError(ex);}};content.Controls.Add(grid);content.Controls.Add(toolbar);LoadNotifications();
        AddPageDescription("Review system and customer notifications and mark completed items as read.");
    }

    private void ShowSettings()
    {
        PreparePage("Store settings"); var grid=CreateGrid(); var key=new TextBox{PlaceholderText="Setting key",Width=180}; var value=new TextBox{PlaceholderText="Setting value",Width=260}; var save=new Button{Text="Save setting",AutoSize=true}; var refresh=new Button{Text="Refresh",AutoSize=true}; var toolbar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=48};toolbar.Controls.AddRange([key,value,save,refresh]);
        AddPageDescription("Manage configurable store settings using the existing database-backed configuration.");
        void LoadSettings(){try{grid.DataSource=settingsController.All(currentUser);}catch(Exception ex){ShowError(ex);}} grid.SelectionChanged+=(_,_)=>{if(grid.CurrentRow?.DataBoundItem is StoreSettingRecord s){key.Text=s.SettingKey;value.Text=s.SettingValue??"";}};save.Click+=(_,_)=>{try{settingsController.Save(currentUser,key.Text,value.Text);LoadSettings();}catch(Exception ex){ShowError(ex);}};refresh.Click+=(_,_)=>LoadSettings();content.Controls.Add(grid);content.Controls.Add(toolbar);LoadSettings();
    }

    private void ShowPromotions()
    {
        PreparePage("Promotions"); var grid=CreateGrid();var name=new TextBox{PlaceholderText="Promotion name",Width=160};var description=new TextBox{PlaceholderText="Description",Width=180};var type=new ComboBox{Width=100,DropDownStyle=ComboBoxStyle.DropDownList};type.Items.AddRange(["Percentage","Fixed"]);type.SelectedIndex=0;var value=new NumericUpDown{Maximum=10000000,DecimalPlaces=2,Width=90};var from=new DateTimePicker{Format=DateTimePickerFormat.Short};var until=new DateTimePicker{Format=DateTimePickerFormat.Short,Value=DateTime.Today.AddDays(30)};var active=new CheckBox{Text="Active",Checked=true,AutoSize=true};var save=new Button{Text="Save promotion",AutoSize=true};var refresh=new Button{Text="Refresh",AutoSize=true};var toolbar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=82,WrapContents=true};toolbar.Controls.AddRange([name,description,type,value,from,until,active,save,refresh]);void LoadPromotions(){try{grid.DataSource=promotionController.All(currentUser);}catch(Exception ex){ShowError(ex);}}grid.SelectionChanged+=(_,_)=>{if(grid.CurrentRow?.DataBoundItem is PromotionRecord p){name.Text=p.Name;description.Text=p.Description??"";type.Text=p.DiscountType;value.Value=p.DiscountValue;from.Value=p.ValidFrom;until.Value=p.ValidUntil;active.Checked=p.IsActive;}};save.Click+=(_,_)=>{try{promotionController.Save(currentUser,new PromotionRecord{Name=name.Text,Description=description.Text,DiscountType=type.Text,DiscountValue=value.Value,ValidFrom=from.Value,ValidUntil=until.Value,IsActive=active.Checked});LoadPromotions();}catch(Exception ex){ShowError(ex);}};refresh.Click+=(_,_)=>LoadPromotions();content.Controls.Add(grid);content.Controls.Add(toolbar);LoadPromotions();
    }

    private void ShowCoupons()
    {
        PreparePage("Coupons"); var grid=CreateGrid();var code=new TextBox{PlaceholderText="Coupon code",Width=130};var type=new ComboBox{Width=100,DropDownStyle=ComboBoxStyle.DropDownList};type.Items.AddRange(["Percentage","Fixed"]);type.SelectedIndex=0;var value=new NumericUpDown{Maximum=10000000,DecimalPlaces=2,Width=85};var minimum=new NumericUpDown{Maximum=10000000,DecimalPlaces=2,Width=90};var limit=new NumericUpDown{Minimum=0,Maximum=100000000,Width=80};var from=new DateTimePicker{Format=DateTimePickerFormat.Short};var until=new DateTimePicker{Format=DateTimePickerFormat.Short,Value=DateTime.Today.AddDays(30)};var active=new CheckBox{Text="Active",Checked=true,AutoSize=true};var save=new Button{Text="Save coupon",AutoSize=true};var refresh=new Button{Text="Refresh",AutoSize=true};var toolbar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=82,WrapContents=true};toolbar.Controls.AddRange([code,type,value,minimum,limit,from,until,active,save,refresh]);void LoadCoupons(){try{grid.DataSource=couponController.All(currentUser);}catch(Exception ex){ShowError(ex);}}grid.SelectionChanged+=(_,_)=>{if(grid.CurrentRow?.DataBoundItem is CouponManagementRecord c){code.Text=c.Code;type.Text=c.DiscountType;value.Value=c.Value;minimum.Value=c.MinimumOrderAmount;limit.Value=c.UsageLimit??0;from.Value=c.ValidFrom;until.Value=c.ValidUntil;active.Checked=c.IsActive;}};save.Click+=(_,_)=>{try{couponController.Save(currentUser,new CouponManagementRecord{Code=code.Text,DiscountType=type.Text,Value=value.Value,MinimumOrderAmount=minimum.Value,UsageLimit=limit.Value==0?null:(int?)limit.Value,ValidFrom=from.Value,ValidUntil=until.Value,IsActive=active.Checked});LoadCoupons();}catch(Exception ex){ShowError(ex);}};refresh.Click+=(_,_)=>LoadCoupons();content.Controls.Add(grid);content.Controls.Add(toolbar);LoadCoupons();
    }

    private void ShowAuditLogs()
    {
        PreparePage("Audit logs"); var grid=CreateGrid();var refresh=new Button{Text="Refresh",AutoSize=true};var toolbar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=48};toolbar.Controls.Add(refresh);void LoadLogs(){try{grid.DataSource=advancedController.AuditLogs(currentUser);}catch(Exception ex){ShowError(ex);}}refresh.Click+=(_,_)=>LoadLogs();content.Controls.Add(grid);content.Controls.Add(toolbar);LoadLogs();
    }

    private void ShowWishlist()
    {
        PreparePage("Wishlist");
        var grid = CreateGrid();
        var refresh = new Button { Text = "Refresh", AutoSize = true };
        var remove = new Button { Text = "Remove selected", AutoSize = true };
        var move = new Button { Text = "Move selected to cart", AutoSize = true };
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48 };
        toolbar.Controls.Add(refresh); toolbar.Controls.Add(remove); toolbar.Controls.Add(move);
        void LoadItems() { try { grid.DataSource = wishlist.Items(currentUser); } catch (Exception ex) { ShowError(ex); } }
        remove.Click += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not WishlistItemRecord item) { ShowError(new InvalidOperationException("Select a wishlist item first.")); return; }
            try { wishlist.Remove(currentUser, item.ProductId); LoadItems(); } catch (Exception ex) { ShowError(ex); }
        };
        move.Click += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is not WishlistItemRecord item) { ShowError(new InvalidOperationException("Select a wishlist item first.")); return; }
            if (item.StockQuantity < 1) { ShowError(new InvalidOperationException("This product is out of stock.")); return; }
            try { sales.AddToCart(currentUser, item.ProductId, 1); wishlist.Remove(currentUser, item.ProductId); LoadItems(); MessageBox.Show("Product moved to cart.", "OESS", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            catch (Exception ex) { ShowError(ex); }
        };
        refresh.Click += (_, _) => LoadItems();
        content.Controls.Add(grid); content.Controls.Add(toolbar); LoadItems();
    }

    private void ShowOrders()
    {
        PreparePage("Orders");
        var grid = CreateGrid();
        var refresh = new Button { Text = "Refresh", AutoSize = true }; var status = new ComboBox { Width = 120, DropDownStyle = ComboBoxStyle.DropDownList }; status.Items.AddRange(["Pending", "Confirmed", "Processing", "Shipped", "Delivered", "Cancelled"]); status.SelectedIndex = 0; var update = new Button { Text = "Update selected status", AutoSize = true };
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48 }; toolbar.Controls.Add(refresh); toolbar.Controls.Add(status); toolbar.Controls.Add(update);
        void LoadOrders() { try { grid.DataSource = sales.Orders(currentUser); } catch (Exception ex) { ShowError(ex); } }
        grid.SelectionChanged += (_, _) => { if (grid.CurrentRow?.DataBoundItem is OrderSummary item) status.Text = item.Status; };
        refresh.Click += (_, _) => LoadOrders();
        update.Click += (_, _) => { if (grid.CurrentRow?.DataBoundItem is not OrderSummary item) { ShowError(new InvalidOperationException("Select an order first.")); return; } try { sales.UpdateOrderStatus(currentUser, item.Id, status.Text); LoadOrders(); } catch (Exception ex) { ShowError(ex); } };
        content.Controls.Add(grid); content.Controls.Add(toolbar); LoadOrders();
    }
    private void ShowReports() => ShowReportsPage();
    private void ShowMessagePage(string title, string message) { PreparePage(title); content.Controls.Add(new Label { Text = message, AutoSize = true, Font = new Font("Segoe UI", 12), ForeColor = Color.DimGray }); }
    private static DataGridView CreateGrid()
    {
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None,
            MinimumSize = new Size(280, 180),
            Margin = new Padding(0, 0, 0, 8)
        };
        OessTheme.StyleGrid(grid);
        return grid;
    }
    private static void ShowError(Exception ex) => MessageBox.Show(ex.Message, "OESS", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
