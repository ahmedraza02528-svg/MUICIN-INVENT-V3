using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MUICIN_INVENT_V3
{
    /// <summary>
    /// Interaction logic for AddSupplierWindow.xaml
    /// </summary>
    public partial class AddSupplierWindow : Window
    {
        public string SupplierName { get; private set; } = "";
        public string Phone { get; private set; } = "";
        public string City { get; private set; } = "";
        public string ContactPerson { get; private set; } = "";
        public string Email { get; private set; } = "";
        public string NTN { get; private set; } = "";
        public string OpeningBalance { get; private set; } = "0";
        public string Address { get; private set; } = "";
        public string PaymentTerms { get; private set; } = "";
        public string Notes { get; private set; } = "";
        public string Status { get; private set; } = "Active";

        public AddSupplierWindow()
        {
            InitializeComponent();
        }
        public void LoadSupplier(SupplierRow row)
        {
            if (row == null) return;

            SupplierNameTextBox.Text = row.SupplierName;
            ContactPersonTextBox.Text = row.ContactPerson;
            PhoneTextBox.Text = row.Phone;
            EmailTextBox.Text = row.Email;
            CityTextBox.Text = row.City;
            TaxNumberTextBox.Text = row.NTN;
            OpeningBalanceTextBox.Text = row.OpeningBalance.ToString();
            AddressTextBox.Text = row.Address;
            NotesTextBox.Text = row.Notes;

            // select payment terms if present
            for (int i = 0; i < PaymentTermsComboBox.Items.Count; i++)
            {
                if ((PaymentTermsComboBox.Items[i] as ComboBoxItem)?.Content?.ToString() == row.PaymentTerms)
                {
                    PaymentTermsComboBox.SelectedIndex = i;
                    break;
                }
            }

            // select status
            for (int i = 0; i < StatusComboBox.Items.Count; i++)
            {
                if ((StatusComboBox.Items[i] as ComboBoxItem)?.Content?.ToString() == row.Status)
                {
                    StatusComboBox.SelectedIndex = i;
                    break;
                }
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SaveSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SupplierNameTextBox.Text))
            {
                MessageBox.Show(
                    "Please enter Supplier / Company Name.",
                    "Required Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                SupplierNameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show(
                    "Please enter supplier phone number.",
                    "Required Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                PhoneTextBox.Focus();
                return;
            }

            SupplierName = SupplierNameTextBox.Text.Trim();
            Phone = PhoneTextBox.Text.Trim();
            City = CityTextBox.Text.Trim();
            ContactPerson = ContactPersonTextBox.Text.Trim();
            Email = EmailTextBox.Text.Trim();
            NTN = TaxNumberTextBox.Text.Trim();
            OpeningBalance = OpeningBalanceTextBox.Text.Trim();
            // validate opening balance is numeric
            if (!decimal.TryParse(OpeningBalance, out var ob))
            {
                MessageBox.Show(
                    "Opening Balance must be a numeric value.",
                    "Invalid Input",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                OpeningBalanceTextBox.Focus();
                return;
            }
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
