using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace MUICIN_INVENT_V3
{
    public partial class CustomersPage : Page
    {
        private readonly ObservableCollection<CustomerRow> allCustomers = new ObservableCollection<CustomerRow>();
        private ICollectionView customersView;

        public CustomersPage()
        {
            InitializeComponent();

            customersView = CollectionViewSource.GetDefaultView(allCustomers);
            customersView.Filter = CustomerFilter;

            // If the DataGrid is defined in XAML, connect its ItemsSource
            if (this.FindName("CustomersDataGrid") is DataGrid dg)
            {
                dg.ItemsSource = customersView;
            }

            UpdateSummary();
        }

        private bool CustomerFilter(object obj)
        {
            if (obj is CustomerRow c)
            {
                string q = string.Empty;
                if (this.FindName("SearchTextBox") is TextBox tb)
                {
                    q = tb.Text?.Trim().ToLower() ?? string.Empty;
                }
                if (string.IsNullOrEmpty(q)) return true;

                return (c.CustomerId?.ToLower().Contains(q) == true)
                    || (c.CustomerName?.ToLower().Contains(q) == true)
                    || (c.ContactPerson?.ToLower().Contains(q) == true)
                    || (c.Phone?.ToLower().Contains(q) == true)
                    || (c.City?.ToLower().Contains(q) == true);
            }
            return false;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            customersView?.Refresh();
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var w = new AddCustomerWindow();
            if (w.ShowDialog() == true)
            {
                decimal opening = 0m;
                decimal.TryParse(w.OpeningBalance, out opening);
                decimal credit = 0m;
                decimal.TryParse(w.CreditLimit, out credit);

                allCustomers.Add(new CustomerRow
                {
                    CustomerId = $"CUS-{allCustomers.Count + 1:0000}",
                    CustomerName = w.CustomerName,
                    ContactPerson = w.ContactPerson,
                    Phone = w.Phone,
                    City = w.City,
                    Email = w.Email,
                    NTN = w.NTN,
                    Address = w.Address,
                    Notes = w.Notes,
                    PaymentTerms = w.PaymentTerms,
                    OpeningBalance = opening,
                    CreditLimit = credit,
                    TotalOrders = 0,
                    TotalSpent = 0m,
                    Status = w.Status
                });

                UpdateSummary();
            }
        }



        private void ViewCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is CustomerRow row)
            {
                string details = $"Customer ID: {row.CustomerId}\nName: {row.CustomerName}\nContact: {row.ContactPerson}\nPhone: {row.Phone}\nCity: {row.City}\nOpening Balance: Rs. {row.OpeningBalance:N2}\nCredit Limit: Rs. {row.CreditLimit:N2}\nStatus: {row.Status}";
                MessageBox.Show(details, "Customer Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is CustomerRow row)
            {
                var w = new AddCustomerWindow();
                w.LoadCustomer(row);

                if (w.ShowDialog() == true)
                {
                    // Update customer from dialog results
                    row.CustomerName = w.CustomerName;
                    row.ContactPerson = w.ContactPerson;
                    row.Phone = w.Phone;
                    row.City = w.City;
                    row.Email = w.Email;
                    row.NTN = w.NTN;
                    row.Address = w.Address;
                    row.Notes = w.Notes;
                    row.PaymentTerms = w.PaymentTerms;
                    if (decimal.TryParse(w.OpeningBalance, out var ob)) row.OpeningBalance = ob;
                    if (decimal.TryParse(w.CreditLimit, out var cl)) row.CreditLimit = cl;
                    row.Status = w.Status;

                    customersView?.Refresh();
                    UpdateSummary();
                }
            }
        }

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is CustomerRow row)
            {
                var res = MessageBox.Show($"Delete customer {row.CustomerName}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    allCustomers.Remove(row);
                    customersView?.Refresh();
                    UpdateSummary();
                }
            }
        }

        private void StatementCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is CustomerRow row)
            {
                var s = new CustomerStatementWindow();
                // Pass basic customer info plus current opening balance and totals for display
                s.LoadCustomer(row.CustomerId, row.CustomerName, row.Phone, row.Address, row.OpeningBalance, row.TotalOrders, row.TotalSpent);
                s.ShowDialog();
            }
        }

        private void UpdateSummary()
        {
            if (this.FindName("TotalCustomersText") is TextBlock tc)
            {
                tc.Text = allCustomers.Count.ToString();
            }
            int active = 0; int totalOrders = 0; decimal receivable = 0m;
            foreach (var c in allCustomers)
            {
                if (c.Status == "Active") active++;
                totalOrders += c.TotalOrders;
                receivable += c.OpeningBalance + c.TotalSpent;
            }
            if (this.FindName("ActiveCustomersText") is TextBlock ac) ac.Text = active.ToString();
            if (this.FindName("TotalOrdersText") is TextBlock to) to.Text = totalOrders.ToString();
            if (this.FindName("ReceivableText") is TextBlock rt) rt.Text = $"Rs. {receivable:N2}";
        }
    }



    public class CustomerRow
    {
        public string CustomerId { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string ContactPerson { get; set; } = "";
        public string Phone { get; set; } = "";
        public string City { get; set; } = "";
        public decimal TotalSpent { get; set; } = 0m;
        public int TotalOrders { get; set; } = 0;
        public decimal OpeningBalance { get; set; } = 0m;
        public decimal CreditLimit { get; set; } = 0m;
        public string Email { get; set; } = "";
        public string NTN { get; set; } = "";
        public string Address { get; set; } = "";
        public string Notes { get; set; } = "";
        public string PaymentTerms { get; set; } = "";
        public string Status { get; set; } = "Active";
    }
}
