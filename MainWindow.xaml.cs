using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;

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
    }
}
