using OnlineElectronicSellingSystem.Controllers;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem;

public sealed class RegisterForm : Form
{
    private static readonly Color Navy = Color.FromArgb(18, 42, 76);
    private static readonly Color RoyalBlue = Color.FromArgb(43, 91, 187);
    private readonly AuthController auth;
    private readonly TextBox nameBox = new() { Dock = DockStyle.Fill, PlaceholderText = "Full name" };
    private readonly TextBox emailBox = new() { Dock = DockStyle.Fill, PlaceholderText = "Email address" };
    private readonly TextBox passwordBox = new() { Dock = DockStyle.Fill, UseSystemPasswordChar = true, PlaceholderText = "At least 8 characters" };
    private readonly TextBox confirmBox = new() { Dock = DockStyle.Fill, UseSystemPasswordChar = true, PlaceholderText = "Repeat your password" };
    private readonly CheckBox showPassword = new() { Text = "Show passwords", AutoSize = true, ForeColor = Color.DimGray };
    private readonly Label message = new() { AutoSize = true, ForeColor = Color.Firebrick };

    public RegisterForm()
    {
        auth = new AuthController(new OessRepository(new Database()));
        Text = "OESS | Create customer account";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(720, 540);
        MinimumSize = new Size(720, 540);
        BackColor = Color.FromArgb(245, 247, 251);
        BuildView();
    }

    private void BuildView()
    {
        var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = Color.White };
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38)); shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
        var brand = new Panel { Dock = DockStyle.Fill, BackColor = Navy, Padding = new Padding(32) };
        brand.Controls.Add(new Label { Text = "OESS", AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 28, FontStyle.Bold), Location = new Point(32, 35) });
        brand.Controls.Add(new Label { Text = "Create your account\nand start shopping with confidence.", AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 17, FontStyle.Bold), Location = new Point(34, 125) });
        brand.Controls.Add(new Label { Text = "Your account gives you access to the catalogue, cart, wishlist, orders and support.", AutoSize = false, Size = new Size(220, 80), ForeColor = Color.FromArgb(205, 218, 239), Font = new Font("Segoe UI", 10), Location = new Point(36, 245) });
        shell.Controls.Add(brand, 0, 0);

        var form = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(42, 35, 42, 30), ColumnCount = 1, RowCount = 10, BackColor = Color.White };
        for (var i = 0; i < 10; i++) form.RowStyles.Add(new RowStyle(SizeType.Absolute, i == 0 ? 45 : i == 1 ? 35 : 42));
        form.Controls.Add(new Label { Text = "Create account", Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = Navy, Dock = DockStyle.Fill }, 0, 0);
        form.Controls.Add(new Label { Text = "Register as a customer", ForeColor = Color.DimGray, Dock = DockStyle.Fill }, 0, 1);
        form.Controls.Add(nameBox, 0, 2); form.Controls.Add(emailBox, 0, 3); form.Controls.Add(passwordBox, 0, 4); form.Controls.Add(confirmBox, 0, 5);
        form.Controls.Add(showPassword, 0, 6); form.Controls.Add(message, 0, 7);
        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill };
        var create = new Button { Text = "Create account", Width = 145, Height = 38, BackColor = RoyalBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        var cancel = new Button { Text = "Back to sign in", Width = 125, Height = 38, FlatStyle = FlatStyle.Flat, BackColor = Color.White, ForeColor = RoyalBlue };
        create.FlatAppearance.BorderSize = 0; cancel.FlatAppearance.BorderColor = RoyalBlue;
        create.Click += (_, _) => CreateAccount(); cancel.Click += (_, _) => DialogResult = DialogResult.Cancel;
        actions.Controls.Add(create); actions.Controls.Add(cancel); form.Controls.Add(actions, 0, 8);
        form.Controls.Add(new Label { Text = "Your password is securely hashed before storage.", AutoSize = true, ForeColor = Color.Gray, Font = new Font("Segoe UI", 8) }, 0, 9);
        showPassword.CheckedChanged += (_, _) => { passwordBox.UseSystemPasswordChar = !showPassword.Checked; confirmBox.UseSystemPasswordChar = !showPassword.Checked; };
        AcceptButton = create; CancelButton = cancel;
        shell.Controls.Add(form, 1, 0); Controls.Add(shell);
    }

    private void CreateAccount()
    {
        message.ForeColor = Color.Firebrick; message.Text = "";
        if (passwordBox.Text != confirmBox.Text) { message.Text = "Passwords do not match."; return; }
        try
        {
            auth.RegisterCustomer(nameBox.Text, emailBox.Text, passwordBox.Text);
            MessageBox.Show("Your account was created successfully. You can now sign in.", "Registration complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
        }
        catch (MySqlConnector.MySqlException ex) when (ex.Number == 1062) { message.Text = "An account with this email already exists."; }
        catch (Exception ex) { message.Text = ex.Message; }
    }
}
