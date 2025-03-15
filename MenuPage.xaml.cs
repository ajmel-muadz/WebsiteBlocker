using System.Collections;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Path = System.IO.Path;

namespace WebsiteBlocker
{
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();

            DisplayBlockingStatus();
        }

        // When anything in text input is changed, we change visibility of placeholder text.
        private void tbUrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbUrl.Text))
            {
                tblPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                tblPlaceholder.Visibility = Visibility.Hidden;
            }
        }

        // When grid is clicked, we clear focus from the url text box.
        private void gridMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Removes focus from textbox if input is focused.
            Keyboard.ClearFocus();
        }

        // Used to see list of blocked websites.
        private void btnSee_Click(object sender, RoutedEventArgs e)
        {
            // Credits to this link: https://stackoverflow.com/questions/17001486/how-to-access-wpf-mainwindow-controls-from-my-own-cs-file
            // Author is Nathan Phillips. I will have to use MVVM soon this is a horrible app.
            MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;

            BlockedSitesPage blockedSitesPage = mainWindow.GetBlockedSitesPage();
            blockedSitesPage.LoadBlockedSitesFromDb();  // Before going to the page we load the blocked sites to keep it updated.
            mainWindow.NavigateToBlockedSitesPage();
        }

        // When this button is clicked we add blocked sites to SQL database
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Add the user input in url text box to database ONLY if string input is not empty.
            // Also only allow if blocking is not in progress.
            string? rootDirectory = Path.GetPathRoot(Environment.SystemDirectory);
            string hostsPath = $"{rootDirectory}\\Windows\\System32\\drivers\\etc\\hosts";
            int numOfLinesInHostsFile = GetNumOfLinesInHostsFile(hostsPath);

            if (numOfLinesInHostsFile == 0)
            {
                if (!string.IsNullOrEmpty(tbUrl.Text.Trim()))
                {
                    AddSiteToDb(tbUrl.Text);
                }

                tbUrl.Clear();
            }
            else
            {
                MessageBox.Show("You cannot add during a blocking session!", "Simple Website Blocker notice",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Stop blocking session
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            // A blocking session is happening if the hosts file is populated. Thus, we check
            // if the hosts file is populated as a determinant.
            string? rootDirectory = Path.GetPathRoot(Environment.SystemDirectory);
            string hostsPath = $"{rootDirectory}\\Windows\\System32\\drivers\\etc\\hosts";
            int numOfLinesInHostsFile = GetNumOfLinesInHostsFile(hostsPath);

            if (numOfLinesInHostsFile != 0)
            {
                MessageBoxResult confirmationMessage = MessageBox.Show("Are you sure you want to stop the block session?",
                    "Simple Website Blocker notice", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirmationMessage == MessageBoxResult.Yes)
                {
                    // Clearing hosts file means we stop it basically.
                    ClearHostsFile(hostsPath);
                }
            }
            else
            {
                MessageBox.Show("You cannot stop a session that is not in progress!", "Simple Website Blocker notice",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            DisplayBlockingStatus();
        }

        // Start blocking session
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // In order to block website URLs we must access the host file and add the URLs.
            // Credit to: https://learn.microsoft.com/en-us/troubleshoot/developer/visualstudio/csharp/language-compilers/read-write-text-file
            string? rootDirectory = Path.GetPathRoot(Environment.SystemDirectory);
            string hostsPath = $"{rootDirectory}\\Windows\\System32\\drivers\\etc\\hosts";
            var allWebsiteUrls = GetWebsiteUrls();
            int numOfLinesInHostsFile = GetNumOfLinesInHostsFile(hostsPath);

            if (numOfLinesInHostsFile != 0)
            {
                MessageBox.Show("Session has already started!", "Simple Website Blocker notice",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (allWebsiteUrls.Count == 0)
            {
                MessageBox.Show("You cannot start a session with an empty block list!", "Simple Website Blocker notice",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            // We only allow modifying hosts file if stuff is empty
            else if (allWebsiteUrls.Count > 0)
            {
                // Ensure we can ask for user confirmation before starting block session.
                MessageBoxResult confirmationMessage = MessageBox.Show("Are you sure you want to start a block session?",
                    "Simple Website Blocker notice", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirmationMessage == MessageBoxResult.Yes)
                {
                    // Before writing to hosts file, clear it so we start from a blank slate.
                    ClearHostsFile(hostsPath);

                    // Now we just write each website URL in database to the hosts file.
                    foreach (string url in allWebsiteUrls)
                    {
                        WriteToHostsFile(url, hostsPath);
                    }

                    MessageBox.Show("Block session has started.", "Simple Website Blocker notice",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            DisplayBlockingStatus();
        }

        // Function to add data to SQL database. Converted to SQLite.
        private void AddSiteToDb(string siteName)
        {
            string connectionString = "Data Source=app_blocker.db;Version=3;";
            string query = "INSERT INTO blockedapps (app_name) VALUES (@siteName);";

            using (var connection = new SQLiteConnection(connectionString))
            {
                using (var command = new SQLiteCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@siteName", siteName);  // Anything with a parameter requires this
                    command.ExecuteNonQuery();  // Must use for UPDATE, INSERT, DELETE: Similar to one in python at uni.
                }
            }
        }

        private void ClearHostsFile(string path)
        {
            try
            {
                File.WriteAllText(path, string.Empty);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        private void WriteToHostsFile(string websiteUrl, string path)
        {
            try
            {
                StreamWriter sw = new StreamWriter(path, true);  // True ensures append, not write.
                sw.WriteLine("0.0.0.0" + " " + websiteUrl);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        public int GetNumOfLinesInHostsFile(string path)
        {
            int numOfLinesInFile = 0;
            string? line;
            try
            {
                StreamReader sr = new StreamReader(path);
                line = sr.ReadLine();
                while (line != null)
                {
                    line = sr.ReadLine();
                    numOfLinesInFile++;
                }
                sr.Close();
                Console.ReadLine();
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return numOfLinesInFile;
        }

        // Converted to SQLite
        private ArrayList GetWebsiteUrls()
        {
            var allWebsiteUrls = new ArrayList();

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
                            allWebsiteUrls.Add(appName);
                        }
                    }
                }
            }

            return allWebsiteUrls;
        }

        private void DisplayBlockingStatus()
        {
            string? rootDirectory = Path.GetPathRoot(Environment.SystemDirectory);
            string hostsPath = $"{rootDirectory}\\Windows\\System32\\drivers\\etc\\hosts";
            int numOfLinesInHostsFile = GetNumOfLinesInHostsFile(hostsPath);

            if (numOfLinesInHostsFile != 0)
            {
                tbBlockStatus.Text = "(Website blocking is currently active)";
                tbBlockStatus.Foreground = Brushes.Green;
            }
            else
            {
                tbBlockStatus.Text = "(Website blocking is currently inactive)";
                tbBlockStatus.Foreground = Brushes.Red;
            }
        }
    }
}
