using System.Drawing.Drawing2D;

namespace OnlineElectronicSellingSystem.Views.Components;

public static class OessTheme
{
    public static readonly Color PageBackground = Color.FromArgb(245, 247, 251);
    public static readonly Color SidebarBackground = Color.FromArgb(23, 32, 51);
    public static readonly Color PrimaryBlue = Color.FromArgb(53, 106, 230);
    public static readonly Color SecondaryBlue = Color.FromArgb(91, 141, 239);
    public static readonly Color CardBackground = Color.White;
    public static readonly Color MainText = Color.FromArgb(31, 41, 55);
    public static readonly Color SecondaryText = Color.FromArgb(132, 144, 165);
    public static readonly Color Border = Color.FromArgb(232, 236, 242);
    public static readonly Color Success = Color.FromArgb(22, 163, 106);
    public static readonly Color Warning = Color.FromArgb(242, 169, 59);
    public static readonly Color Danger = Color.FromArgb(224, 82, 82);
    public static readonly Color Info = Color.FromArgb(38, 151, 213);

    public static Font PageTitleFont => new("Segoe UI", 22, FontStyle.Bold);
    public static Font SectionTitleFont => new("Segoe UI", 13, FontStyle.Bold);
    public static Font BodyFont => new("Segoe UI", 9.5f);
    public static Font SmallFont => new("Segoe UI", 8.5f);

    public static void StyleButton(Button button, Color? background = null, bool outlined = false)
    {
        button.AutoSize = true;
        button.MinimumSize = new Size(92, 36);
        button.Padding = new Padding(14, 0, 14, 0);
        button.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = outlined ? 1 : 0;
        button.FlatAppearance.BorderColor = Border;
        button.BackColor = background ?? PrimaryBlue;
        button.ForeColor = outlined ? MainText : Color.White;
        button.Cursor = Cursors.Hand;
    }

    public static Label CreateNavigationSection(string text)
    {
        return new Label
        {
            Text = text.ToUpperInvariant(),
            Width = 214,
            Height = 28,
            ForeColor = Color.FromArgb(139, 158, 187),
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            TextAlign = ContentAlignment.BottomLeft,
            Padding = new Padding(8, 0, 0, 4),
            Margin = new Padding(0, 12, 0, 0)
        };
    }

    public static Panel CreatePageHeader(string title, string description)
    {
        var header = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = PageBackground, Padding = new Padding(4, 0, 4, 8) };
        header.Controls.Add(new Label { Text = title, AutoSize = true, Font = SectionTitleFont, ForeColor = MainText });
        header.Controls.Add(new Label { Text = description, AutoSize = true, Location = new Point(1, 28), Font = SmallFont, ForeColor = SecondaryText });
        return header;
    }

    public static Panel CreateProfileHeader(string fullName, string role)
    {
        var initials = string.Concat(fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(part => part[0])).ToUpperInvariant();
        var panel = new Panel { Dock = DockStyle.Fill, BackColor = CardBackground, Padding = new Padding(8, 2, 0, 2) };
        var avatar = new Label
        {
            Text = initials,
            Dock = DockStyle.Left,
            Width = 38,
            BackColor = PrimaryBlue,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Margin = new Padding(0, 4, 8, 4)
        };
        var details = new Label
        {
            Text = $"{fullName}\n{role}",
            Dock = DockStyle.Fill,
            ForeColor = MainText,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(10, 0, 0, 0)
        };
        panel.Controls.Add(details);
        panel.Controls.Add(avatar);
        return panel;
    }

    public static Label CreateDashboardSection(string text)
    {
        return new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            Font = SectionTitleFont,
            ForeColor = MainText,
            TextAlign = ContentAlignment.BottomLeft,
            Padding = new Padding(4, 0, 0, 6)
        };
    }

    public static void StyleInput(Control control)
    {
        control.Font = BodyFont;
        control.Margin = new Padding(4, 4, 4, 4);
        switch (control)
        {
            case TextBox textBox:
                textBox.Height = 34;
                textBox.BorderStyle = BorderStyle.FixedSingle;
                break;
            case ComboBox comboBox:
                comboBox.Height = 34;
                comboBox.FlatStyle = FlatStyle.Flat;
                break;
            case NumericUpDown numeric:
                numeric.Height = 34;
                break;
            case DateTimePicker date:
                date.Height = 34;
                break;
            case CheckBox checkBox:
                checkBox.AutoSize = true;
                checkBox.Margin = new Padding(8, 10, 8, 4);
                break;
        }
    }

    public static void StyleToolbar(FlowLayoutPanel toolbar)
    {
        toolbar.BackColor = CardBackground;
        toolbar.Padding = new Padding(12, 8, 12, 8);
        toolbar.Margin = new Padding(0, 0, 0, 10);
        toolbar.WrapContents = true;
        toolbar.AutoScroll = true;
        toolbar.AutoSize = true;
        toolbar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        toolbar.MinimumSize = new Size(0, 52);
        toolbar.BorderStyle = BorderStyle.FixedSingle;
        foreach (Control control in toolbar.Controls)
        {
            if (control is Button button) StyleButton(button);
            else StyleInput(control);
        }
    }

    public static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = CardBackground;
        grid.BorderStyle = BorderStyle.FixedSingle;
        grid.GridColor = Border;
        grid.EnableHeadersVisualStyles = false;
        grid.RowHeadersVisible = false;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.ReadOnly = true;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        grid.AllowUserToResizeColumns = true;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.AutoGenerateColumns = true;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        grid.Font = BodyFont;
        grid.RowTemplate.Height = 38;
        grid.ColumnHeadersHeight = 48;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = SidebarBackground,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Alignment = DataGridViewContentAlignment.MiddleLeft,
            Padding = new Padding(10, 6, 10, 6),
            WrapMode = DataGridViewTriState.True
        };
        grid.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = CardBackground,
            ForeColor = MainText,
            SelectionBackColor = Color.FromArgb(226, 235, 255),
            SelectionForeColor = MainText,
            Padding = new Padding(10, 7, 10, 7),
            WrapMode = DataGridViewTriState.True,
            NullValue = ""
        };
        grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(249, 250, 252) };
    }

    public static Panel CreateCard(string title, string value, Color accent)
    {
        var card = new Panel { Dock = DockStyle.Fill, BackColor = CardBackground, Margin = new Padding(7), Padding = new Padding(18, 14, 14, 12) };
        card.Paint += (_, e) =>
        {
            using var pen = new Pen(Border);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };
        card.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 5, BackColor = accent });
        card.Controls.Add(new Label { Text = title, AutoSize = true, Location = new Point(18, 14), ForeColor = SecondaryText, Font = SmallFont });
        card.Controls.Add(new Label { Text = value, AutoSize = true, Location = new Point(18, 39), ForeColor = MainText, Font = new Font("Segoe UI", 20, FontStyle.Bold) });
        return card;
    }
}
