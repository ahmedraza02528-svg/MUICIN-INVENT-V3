using System;
using System.Windows;
using System.Windows.Controls;
using MUICIN_INVENT_V3.Models;
using MUICIN_INVENT_V3.Services;

namespace MUICIN_INVENT_V3.Purchases
{
    public partial class AddPaymentPage : Page
    {
        private Purchase purchase;

        public AddPaymentPage(Purchase p)
        {
            InitializeComponent();
            purchase = p;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.GoBack();
        }

        private void AddPayment_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(AmountTextBox.Text.Trim(), out var amt) || amt <= 0)
            {
                MessageBox.Show("Enter a valid amount.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var method = (MethodComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Cash";

            var pay = new PurchasePayment
            {
                PaymentId = null,
                PurchaseId = purchase.PurchaseId,
                SupplierId = purchase.SupplierId,
                Date = DateTime.Now,
                Amount = amt,
                PaymentMethod = method,
                Reference = ReferenceTextBox.Text.Trim()
            };

            PurchaseService.Instance.AddPayment(purchase, pay);
            MessageBox.Show("Payment added.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            this.NavigationService?.GoBack();
        }
    }
}
