using System;
using System.Windows;
using System.Windows.Controls;

namespace MUICIN_INVENT_V3
{
    /// <summary>
    /// Interaction logic for AddCustomerWindow.xaml
    /// </summary>
    public partial class AddCustomerWindow : Window
    {
        public string CustomerName { get; private set; } = "";
        public string Phone { get; private set; } = "";
        public string City { get; private set; } = "";
        public string ContactPerson { get; private set; } = "";
        public string Email { get; private set; } = "";
        public string NTN { get; private set; } = "";
        public string OpeningBalance { get; private set; } = "0";
        public string CreditLimit { get; private set; } = "0";
        public string Address { get; private set; } = "";
        public string PaymentTerms { get; private set; } = "";
        public string Notes { get; private set; } = "";
        public string Status { get; private set; } = "Active";

        public AddCustomerWindow()
        {
            InitializeComponent();
        }

        public void LoadCustomer(object row)
        {
            if (row == null) return;

            // Try to read properties dynamically (works with CustomerRow from CustomersPage)
            try
            {
                dynamic d = row;
                CustomerNameTextBox.Text = d.CustomerName ?? "";
                ContactPersonTextBox.Text = d.ContactPerson ?? "";
                PhoneTextBox.Text = d.Phone ?? "";
                EmailTextBox.Text = d.Email ?? "";
                CityTextBox.Text = d.City ?? "";
                TaxNumberTextBox.Text = d.NTN ?? "";
                OpeningBalanceTextBox.Text = (d.OpeningBalance != null) ? d.OpeningBalance.ToString() : "0";
                CreditLimitTextBox.Text = (d.CreditLimit != null) ? d.CreditLimit.ToString() : "0";
                AddressTextBox.Text = d.Address ?? "";
                NotesTextBox.Text = d.Notes ?? "";

                string pt = d.PaymentTerms ?? "";
                for (int i = 0; i < PaymentTermsComboBox.Items.Count; i++)
                {
                    if ((PaymentTermsComboBox.Items[i] as ComboBoxItem)?.Content?.ToString() == pt)
                    {
                        PaymentTermsComboBox.SelectedIndex = i;
                        break;
                    }
                }

                string st = d.Status ?? "Active";
                for (int i = 0; i < StatusComboBox.Items.Count; i++)
                {
                    if ((StatusComboBox.Items[i] as ComboBoxItem)?.Content?.ToString() == st)
                    {
                        StatusComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch
            {
                // ignore if dynamic access fails
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CustomerNameTextBox.Text))
            {
                MessageBox.Show(
                    "Please enter Customer / Business Name.",
                    "Required Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                CustomerNameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show(
                    "Please enter customer phone number.",
                    "Required Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                PhoneTextBox.Focus();
                return;
            }

            // validate numeric fields
            if (!decimal.TryParse(OpeningBalanceTextBox.Text.Trim(), out var ob))
            {
                MessageBox.Show(
                    "Opening Balance must be numeric.",
                    "Invalid Input",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                OpeningBalanceTextBox.Focus();
                return;
            }

            if (!decimal.TryParse(CreditLimitTextBox.Text.Trim(), out var cl))
            {
                MessageBox.Show(
                    "Credit Limit must be numeric.",
                    "Invalid Input",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                CreditLimitTextBox.Focus();
                return;
            }

            CustomerName = CustomerNameTextBox.Text.Trim();
            Phone = PhoneTextBox.Text.Trim();
            City = CityTextBox.Text.Trim();
            ContactPerson = ContactPersonTextBox.Text.Trim();
            Email = EmailTextBox.Text.Trim();
            NTN = TaxNumberTextBox.Text.Trim();
            OpeningBalance = ob.ToString();
            CreditLimit = cl.ToString();
            Address = AddressTextBox.Text.Trim();
            PaymentTerms = (PaymentTermsComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            Notes = NotesTextBox.Text.Trim();

            if (StatusComboBox.SelectedItem is ComboBoxItem selectedStatus)
            {
                Status = selectedStatus.Content.ToString() ?? "Active";
            }

            DialogResult = true;
            Close();
        }
    }
}
