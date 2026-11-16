using OnlineElectronicSellingSystem.Controllers;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Repositories;
using OnlineElectronicSellingSystem.Views.Components;

namespace OnlineElectronicSellingSystem;

public sealed class LoginForm : Form
{
    private static readonly Color Navy = OessTheme.SidebarBackground;
    private static readonly Color RoyalBlue = OessTheme.PrimaryBlue;
    private static readonly Color Background = OessTheme.PageBackground;
    private readonly AuthController auth;
    private readonly TextBox emailBox = new() { Dock = DockStyle.Fill, PlaceholderText = "you@example.com" };
    private readonly TextBox passwordBox = new() { Dock = DockStyle.Fill, UseSystemPasswordChar = true, PlaceholderText = "Enter your password" };
    private readonly CheckBox rememberBox = new() { Text = "Remember me", AutoSize = true, ForeColor = Color.DimGray };
    private readonly CheckBox showPasswordBox = new() { Text = "Show password", AutoSize = true, ForeColor = Color.DimGray };
    private readonly Label messageLabel = new() { AutoSize = true, MaximumSize = new Size(390, 45), ForeColor = Color.Firebrick };
    private readonly Button submitButton = new() { Text = "Sign in", Height = 42, Dock = DockStyle.Top, BackColor = RoyalBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
    private readonly Button modeButton = new() { Text = "Create a new customer account", Height = 34, Dock = DockStyle.Top, FlatStyle = FlatStyle.Flat, BackColor = Color.White, ForeColor = RoyalBlue, Font = new Font("Segoe UI", 9, FontStyle.Underline) };

    public LoginForm()
    {
        auth = new AuthController(new OessRepository(new Database()));
        Text = "OESS | Secure sign in";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 590);
        ClientSize = new Size(980, 620);
        BackColor = Background;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BuildView();
        OessTheme.StyleButton(submitButton);
        OessTheme.StyleButton(modeButton, Color.White, true);
    }

    private void BuildView()
    {
        var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = Color.White };
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42)); shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        shell.Controls.Add(BuildBrandPanel(), 0, 0); shell.Controls.Add(BuildLoginPanel(), 1, 0);
        Controls.Add(shell);
        AcceptButton = submitButton;
    }

    private Control BuildBrandPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill, BackColor = Navy, Padding = new Padding(42) };
        var logo = new Label { Text = "OESS", AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 32, FontStyle.Bold), Location = new Point(42, 42) };
        var line = new Panel { BackColor = Color.FromArgb(92, 143, 235), Size = new Size(72, 4), Location = new Point(46, 105) };
        var title = new Label { Text = "Sell smarter.\nServe better.", AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 25, FontStyle.Bold), Location = new Point(46, 150) };
        var text = new Label { Text = "A unified workspace for products, orders, customers, inventory and sales operations.", AutoSize = false, Size = new Size(300, 75), ForeColor = Color.FromArgb(210, 221, 240), Font = new Font("Segoe UI", 11), Location = new Point(48, 285) };
        var footer = new Label { Text = "ONLINE ELECTRONIC SELLING SYSTEM\nSecure business management", AutoSize = true, ForeColor = Color.FromArgb(167, 187, 217), Font = new Font("Segoe UI", 8), Location = new Point(48, 500) };
        panel.Controls.AddRange([logo, line, title, text, footer]);
        return panel;
    }

    private Control BuildLoginPanel()
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(58, 45, 58, 35), ColumnCount = 1, RowCount = 10, BackColor = Color.White };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        for (var i = 2; i < 10; i++) panel.RowStyles.Add(new RowStyle(SizeType.Absolute, i == 5 ? 48 : i == 8 ? 76 : i == 9 ? 34 : 42));
        panel.Controls.Add(new Label { Text = "Welcome back", Font = new Font("Segoe UI", 23, FontStyle.Bold), ForeColor = Navy, Dock = DockStyle.Fill }, 0, 0);
        panel.Controls.Add(new Label { Text = "Sign in to continue to your OESS workspace.", ForeColor = Color.DimGray, Dock = DockStyle.Fill }, 0, 1);
        panel.Controls.Add(new Label { Text = "Email address", AutoSize = true, ForeColor = Color.DimGray }, 0, 2); panel.Controls.Add(emailBox, 0, 3);
        panel.Controls.Add(new Label { Text = "Password", AutoSize = true, ForeColor = Color.DimGray }, 0, 4); panel.Controls.Add(passwordBox, 0, 5);
        var options = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false }; options.Controls.Add(rememberBox); options.Controls.Add(showPasswordBox); panel.Controls.Add(options, 0, 6);
        panel.Controls.Add(messageLabel, 0, 7);
        var actions = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 }; actions.RowStyles.Add(new RowStyle(SizeType.Absolute, 42)); actions.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        submitButton.Click += (_, _) => Submit();
        modeButton.Click += (_, _) => ShowRegistrationPage();
        actions.Controls.Add(submitButton, 0, 0); actions.Controls.Add(modeButton, 0, 1); panel.Controls.Add(actions, 0, 8);
        showPasswordBox.CheckedChanged += (_, _) => passwordBox.UseSystemPasswordChar = !showPasswordBox.Checked;
        var credentials = new Label { Text = "Development administrator\nEmail: admin@oess.local    Password: admin", AutoSize = true, ForeColor = Color.FromArgb(98, 108, 124), Font = new Font("Segoe UI", 8), Padding = new Padding(0, 12, 0, 0) };
        panel.Controls.Add(credentials, 0, 9);
        return panel;
    }

    private void ShowRegistrationPage()
    {
        var name = new TextBox { Dock = DockStyle.Fill, PlaceholderText = "Full name" };
        var confirm = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true, PlaceholderText = "Repeat your password" };
        var error = new Label { AutoSize = true, MaximumSize = new Size(390, 45), ForeColor = Color.Firebrick };
        var create = new Button { Text = "Create customer account", Height = 42, Dock = DockStyle.Top, BackColor = RoyalBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        var back = new Button { Text = "Back to sign in", Height = 34, Dock = DockStyle.Top, FlatStyle = FlatStyle.Flat, BackColor = Color.White, ForeColor = RoyalBlue, Font = new Font("Segoe UI", 9, FontStyle.Underline) };
        create.FlatAppearance.BorderSize = 0;
        OessTheme.StyleButton(create);
        OessTheme.StyleButton(back, Color.White, true);
        OessTheme.StyleInput(name);
        OessTheme.StyleInput(emailBox);
        OessTheme.StyleInput(passwordBox);
        OessTheme.StyleInput(confirm);
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(58, 35, 58, 25), ColumnCount = 1, RowCount = 12, BackColor = Color.White };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        for (var i = 2; i < 12; i++) panel.RowStyles.Add(new RowStyle(SizeType.Absolute, i is 3 or 5 or 7 or 9 ? 42 : i == 11 ? 76 : 34));
        panel.Controls.Add(new Label { Text = "Create your account", Font = new Font("Segoe UI", 23, FontStyle.Bold), ForeColor = Navy, Dock = DockStyle.Fill }, 0, 0);
        panel.Controls.Add(new Label { Text = "Register as a new OESS customer.", ForeColor = Color.DimGray, Dock = DockStyle.Fill }, 0, 1);
        panel.Controls.Add(new Label { Text = "Full name", AutoSize = true, ForeColor = Color.DimGray }, 0, 2); panel.Controls.Add(name, 0, 3);
        panel.Controls.Add(new Label { Text = "Email address", AutoSize = true, ForeColor = Color.DimGray }, 0, 4); panel.Controls.Add(emailBox, 0, 5);
        panel.Controls.Add(new Label { Text = "Password", AutoSize = true, ForeColor = Color.DimGray }, 0, 6); panel.Controls.Add(passwordBox, 0, 7);
        panel.Controls.Add(new Label { Text = "Confirm password", AutoSize = true, ForeColor = Color.DimGray }, 0, 8); panel.Controls.Add(confirm, 0, 9);
        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill };
        actions.Controls.Add(create); actions.Controls.Add(back);
        panel.Controls.Add(error, 0, 10);
        panel.Controls.Add(actions, 0, 11);
        Controls.Clear();
        var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = Color.White };
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42)); shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        shell.Controls.Add(BuildBrandPanel(), 0, 0); shell.Controls.Add(panel, 1, 0); Controls.Add(shell);
        AcceptButton = create;
        showPasswordBox.CheckedChanged += (_, _) => { passwordBox.UseSystemPasswordChar = !showPasswordBox.Checked; confirm.UseSystemPasswordChar = !showPasswordBox.Checked; };
        back.Click += (_, _) => { Controls.Clear(); BuildView(); };
        create.Click += (_, _) =>
        {
            error.Text = "";
            if (passwordBox.Text != confirm.Text) { error.Text = "Passwords do not match."; return; }
            try
            {
                auth.RegisterCustomer(name.Text, emailBox.Text, passwordBox.Text);
                MessageBox.Show("Account created successfully. You can now sign in.", "Registration complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Controls.Clear(); BuildView();
            }
            catch (Exception ex) { error.Text = ex.Message; }
        };
    }

    private void Submit()
    {
        messageLabel.ForeColor = Color.Firebrick; messageLabel.Text = "";
        try
        {
            var user = auth.Login(emailBox.Text, passwordBox.Text);
            if (user is null) { messageLabel.Text = "Sign-in failed: check your email, password, and account status."; MessageBox.Show(messageLabel.Text, "Sign-in failed", MessageBoxButtons.OK, MessageBoxIcon.Warning); passwordBox.SelectAll(); passwordBox.Focus(); return; }
            Hide(); using (var main = new MainForm(user, new Database())) main.ShowDialog(this); Show(); passwordBox.Clear();
        }
        catch (Exception ex) { messageLabel.Text = $"Login error: {ex.Message}"; MessageBox.Show(messageLabel.Text, "Sign-in error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
