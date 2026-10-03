using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace MUICIN_INVENT_V3
{
    public partial class SuppliersPage : Page
    {
        private readonly System.Collections.ObjectModel.ObservableCollection<SupplierRow> allSuppliers =
            MUICIN_INVENT_V3.Services.SupplierService.Instance.Suppliers;

        private System.ComponentModel.ICollectionView suppliersView;

        public SuppliersPage()
        {
            InitializeComponent();

            suppliersView = System.Windows.Data.CollectionViewSource.GetDefaultView(allSuppliers);
            suppliersView.Filter = SupplierFilter;

            SuppliersDataGrid.ItemsSource = suppliersView;

            UpdateSummary();
        }

        private bool SupplierFilter(object obj)
        {
            if (obj is SupplierRow s)
            {
                string q = SearchTextBox?.Text?.Trim().ToLower() ?? "";
                if (string.IsNullOrEmpty(q)) return true;

                return (s.SupplierId?.ToLower().Contains(q) == true)
                    || (s.SupplierName?.ToLower().Contains(q) == true)
                    || (s.ContactPerson?.ToLower().Contains(q) == true)
                    || (s.Phone?.ToLower().Contains(q) == true)
                    || (s.City?.ToLower().Contains(q) == true);
            }

            return false;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            suppliersView.Refresh();
        }

        private void AddSupplier_Click(object sender, RoutedEventArgs e)
        {
            AddSupplierWindow addSupplierWindow = new AddSupplierWindow();

            if (addSupplierWindow.ShowDialog() == true)
            {
                decimal opening = 0m;
                decimal.TryParse(addSupplierWindow.OpeningBalance, out opening);

                allSuppliers.Add(new SupplierRow
                {
                    SupplierId = MUICIN_INVENT_V3.Services.SupplierService.Instance.GenerateSupplierId(),
                    SupplierName = addSupplierWindow.SupplierName,
                    ContactPerson = addSupplierWindow.ContactPerson,
                    Phone = addSupplierWindow.Phone,
                    City = addSupplierWindow.City,
                    TotalPurchases = 0m,
                    OpeningBalance = opening,
                    Status = addSupplierWindow.Status
                });

                UpdateSummary();
            }
        }

        private void ViewSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is SupplierRow row)
            {
                // Show a simple details dialog for now
                string details = $"Supplier ID: {row.SupplierId}\n" +
                                 $"Name: {row.SupplierName}\n" +
                                 $"Contact: {row.ContactPerson}\n" +
                                 $"Phone: {row.Phone}\n" +
                                 $"City: {row.City}\n" +
                                 $"Opening Balance: Rs. {row.OpeningBalance:N2}\n" +
                                 $"Status: {row.Status}\n";

                MessageBox.Show(details, "Supplier Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is SupplierRow row)
            {
                AddSupplierWindow w = new AddSupplierWindow();
                w.LoadSupplier(row);

                if (w.ShowDialog() == true)
                {
                    // update values
                    row.SupplierName = w.SupplierName;
                    row.ContactPerson = w.ContactPerson;
                    row.Phone = w.Phone;
                    row.City = w.City;
                    decimal opening = 0m;
                    decimal.TryParse(w.OpeningBalance, out opening);
                    row.OpeningBalance = opening;
                    row.Status = w.Status;

                    suppliersView.Refresh();
                    UpdateSummary();
                }
            }
        }

        private void DeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is SupplierRow row)
            {
                var res = MessageBox.Show($"Delete supplier {row.SupplierName}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    allSuppliers.Remove(row);
                    suppliersView.Refresh();
                    UpdateSummary();
                }
            }
        }

        private void UpdateSummary()
        {
            TotalSuppliersText.Text = allSuppliers.Count.ToString();

            int activeCount = 0;
            decimal totalOutstanding = 0m;
            decimal totalPurchases = 0m; // placeholder until purchases added

            foreach (var s in allSuppliers)
            {
                if (s.Status == "Active") activeCount++;
                totalOutstanding += s.OpeningBalance;
                totalPurchases += s.TotalPurchases;
            }

            ActiveSuppliersText.Text = activeCount.ToString();
            TotalPurchasesText.Text = $"Rs. {totalPurchases:N2}";
            OutstandingPayableText.Text = $"Rs. {totalOutstanding:N2}";
        }
    }

    public class SupplierRow
    {
        public string SupplierId { get; set; } = "";
        public string SupplierName { get; set; } = "";
        public string ContactPerson { get; set; } = "";
        public string Phone { get; set; } = "";
        public string City { get; set; } = "";
        public decimal TotalPurchases { get; set; } = 0m;
        public decimal OpeningBalance { get; set; } = 0m;
        public string Email { get; set; } = "";
        public string NTN { get; set; } = "";
        public string Address { get; set; } = "";
        public string Notes { get; set; } = "";
        public string PaymentTerms { get; set; } = "";
        public string Status { get; set; } = "Active";
    }
}
