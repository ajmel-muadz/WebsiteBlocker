using System.Data.SQLite;
using System.Windows;

namespace WebsiteBlocker
{
    public partial class MainWindow : Window
    {
        // Define our pages already.
        private MenuPage menuPage = new MenuPage();
        private BlockedSitesPage blockedSitesPage = new BlockedSitesPage();

        public MainWindow()
        {
            InitializeComponent();
            InitializeDatabase();

            NavigateToMenuPage(); // Navigate to the menu first upon program start.
        }

        public void NavigateToMenuPage()
        {
            frameMain.Navigate(menuPage);
        }

        public void NavigateToBlockedSitesPage()
        {
            frameMain.Navigate(blockedSitesPage);
        }

        public BlockedSitesPage GetBlockedSitesPage()
        {
            return blockedSitesPage;
        }

        public MenuPage GetMenuPage()
        {
            return menuPage;
        }

        // Code to initialise SQLite database for persitent storage of blocked websites.
        private void InitializeDatabase()
        {
            // DB is automatically created if it does not exist.
            string connectionString = "Data Source=app_blocker.db;Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // NOTE: @ is for multi-line.
                string query = @"CREATE TABLE IF NOT EXISTS blockedapps(
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    app_name TEXT
                               );";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
