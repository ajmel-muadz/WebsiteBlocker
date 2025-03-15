using System.Collections;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Path = System.IO.Path;

namespace WebsiteBlocker
{
    /// <summary>
    /// Interaction logic for BlockedSitesPage.xaml
    /// </summary>
    public partial class BlockedSitesPage : Page
    {
        public BlockedSitesPage()
        {
            InitializeComponent();
        }

        // Back button to go back to menu
        private void imgBackArrow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Credits to this link: https://stackoverflow.com/questions/17001486/how-to-access-wpf-mainwindow-controls-from-my-own-cs-file
            // Author is Nathan Phillips. I will have to use MVVM soon this is a horrible app.
            MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            mainWindow.NavigateToMenuPage();
        }

        // Delete a number of items on the list. Converted to SQLite.
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            MenuPage menuPage = mainWindow.GetMenuPage();

            string? rootDirectory = Path.GetPathRoot(Environment.SystemDirectory);
            string hostsPath = $"{rootDirectory}\\Windows\\System32\\drivers\\etc\\hosts";
            int numOfLinesInHostsFile = menuPage.GetNumOfLinesInHostsFile(hostsPath);

            if (numOfLinesInHostsFile == 0)
            {
                var items = lvEntries.SelectedItems;
                var itemsList = new ArrayList(items);

                string connectionString = "Data Source=app_blocker.db;Version=3;";

                string query;
                foreach (var item in itemsList)
                {
                    query = "DELETE FROM blockedapps WHERE app_name=@item";

                    using (var connection = new SQLiteConnection(connectionString))
                    {
                        using (var command = new SQLiteCommand(query, connection))
                        {
                            connection.Open();
                            command.Parameters.AddWithValue("@item", item);  // Anything with a parameter requires this
                            command.ExecuteNonQuery();  // Must use for UPDATE, INSERT and DELETE just like python one in uni.
                        }
                    }
                }

                LoadBlockedSitesFromDb();
            }
            else
            {
                MessageBox.Show("You cannot delete during a blocking session!", "Simple Website Blocker notice",
    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Converted to SQLite.
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            MenuPage menuPage = mainWindow.GetMenuPage();

            string? rootDirectory = Path.GetPathRoot(Environment.SystemDirectory);
            string hostsPath = $"{rootDirectory}\\Windows\\System32\\drivers\\etc\\hosts";
            int numOfLinesInHostsFile = menuPage.GetNumOfLinesInHostsFile(hostsPath);

            if (numOfLinesInHostsFile == 0)
            {
                string connectionString = "Data Source=app_blocker.db;Version=3;";
                string query = "DELETE FROM blockedapps;";

                using (var connection = new SQLiteConnection(connectionString))
                {
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();  // Must use for UPDATE, INSERT and DELETE similar to one in python at uni.
                    }
                }

                LoadBlockedSitesFromDb();
            }
            else
            {
                MessageBox.Show("You cannot clear during a blocking session!", "Simple Website Blocker notice",
MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Simply load the blocked sites from SQL database.
        // Any other add/delete functionality SHOULD only happen via database, not UI.
        // Converted to SQLite.
        public void LoadBlockedSitesFromDb()
        {
            lvEntries.Items.Clear();  // Refresh list view every time getting from DB.

            string connectionString = "Data Source=app_blocker.db;Version=3;";
            string query = "SELECT * FROM blockedapps;";

            using (var connection = new SQLiteConnection(connectionString))
            {
                using (var command = new SQLiteCommand(query, connection))
                {
                    connection.Open();
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string appName = reader.GetString(1);
                            lvEntries.Items.Add(appName);
                        }
                    }
                }
            }
        }
    }
}
