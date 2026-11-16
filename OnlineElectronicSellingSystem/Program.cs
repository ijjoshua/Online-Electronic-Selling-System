namespace OnlineElectronicSellingSystem
{
    using Data;

    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            try
            {
                var database = new Data.Database();
                new Data.DevelopmentAdminProvisioner(database).EnsureAdmin();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"OESS could not connect to the database.\n\n{ex.Message}", "Database connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Run(new LoginForm());
        }
    }
}