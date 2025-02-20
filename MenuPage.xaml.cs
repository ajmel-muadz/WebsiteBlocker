using AppBlocker;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            // Add the user input in url text box to database ONLY if string input is not empty
            if (!string.IsNullOrEmpty(tbUrl.Text.Trim()))
            {
                AddSiteToDb(tbUrl.Text);
            }

            tbUrl.Clear();
        }

        // Start blocking session for some amount of time
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // In order to block website URLs we must access the host file and add the URLs.
            // Credit to: https://learn.microsoft.com/en-us/troubleshoot/developer/visualstudio/csharp/language-compilers/read-write-text-file
            string hostsPath = "C:\\Windows\\System32\\drivers\\etc\\hosts";
            var allWebsiteUrls = GetWebsiteUrls();

            // Before writing to hosts file, clear it so we start from a blank slate.
            ClearHostsFile(hostsPath);

            // Now we just write each website URL in database to the hosts file.
            foreach (string url in allWebsiteUrls)
            {
                WriteToHostsFile(url, hostsPath);
            }
        }

        // Function to add data to SQL database.
        private void AddSiteToDb(string siteName)
        {
            string connectionString = $"SERVER=localhost;DATABASE=app_blocker_db;UID={DbConfig.SqlUid};PASSWORD={DbConfig.SqlPassword};";
            string query = "INSERT INTO blockedapps (app_name) VALUES (@siteName)";

            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@siteName", siteName);  // Anything with a parameter requires this
                    command.ExecuteNonQuery();  // Must use for UPDATE, INSERT and DELETE
                                                // Similar to one in python at uni.
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

        private ArrayList GetWebsiteUrls()
        {
            var allWebsiteUrls = new ArrayList();

            string connectionString = $"SERVER=localhost;DATABASE=app_blocker_db;UID={DbConfig.SqlUid};PASSWORD={DbConfig.SqlPassword};";
            string query = "SELECT * FROM blockedapps";

            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader =
                        command.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            string appName = reader.GetString("app_name");
                            allWebsiteUrls.Add(appName);
                        }
                    }
                }
            }

            return allWebsiteUrls;
        }
    }
}
