using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MUICIN_INVENT_V3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Navigate to dashboard at startup
            MainContentFrame.Navigate(new DashboardPage());
            UpdateTitleForCurrentPage();

            MainContentFrame.Navigated += (s, e) =>
            {
                BackButton.IsEnabled = MainContentFrame.CanGoBack;
                UpdateTitleForCurrentPage();
            };
        }

        private void UpdateTitleForCurrentPage()
        {
            if (MainContentFrame.Content is Page p)
            {
                CurrentPageTitle.Text = p.Title ?? p.GetType().Name;
            }
        }

        private void Inventory_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new InventoryPage());
        }

        private void Purchases_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new PurchasesPage());
        }
        private void Sales_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new SalesPage());
        }

        private void Customers_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new CustomersPage());
        }

        private void Suppliers_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new SuppliersPage());
        }

        private void AccountsLedgers_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new AccountsLedgersPage());
        }

        private void ReportsAnalytics_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new ReportsAnalyticsPage());
        }

        private void Employees_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new EmployeesPage());
        }

        private void Partners_Click(object sender, RoutedEventArgs e)
        {
            // Partners page not implemented yet - no-op
        }

        private void TasksWork_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new TasksWorkPage());
        }

        private void Locations_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new LocationsPage());
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new SettingsPage());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainContentFrame.CanGoBack) MainContentFrame.GoBack();
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            // Undo manager not implemented fully; placeholder to be wired into UndoManager
            MessageBox.Show("Undo is not available yet.", "Undo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}