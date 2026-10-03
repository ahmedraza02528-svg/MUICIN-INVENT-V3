using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MUICIN_INVENT_V3.Models;
using MUICIN_INVENT_V3.Services;

namespace MUICIN_INVENT_V3.Purchases
{
    public partial class ViewPurchasePage : Page
    {
        private Purchase purchase;

        public ViewPurchasePage(Purchase p)
        {
            InitializeComponent();
            purchase = p;
            Load();
        }

        private void Load()
        {
            PurchaseNumberText.Text = $"Purchase: {purchase.PurchaseNumber}";
            SupplierText.Text = $"Supplier: {purchase.SupplierName} ({purchase.SupplierId})";
            DateText.Text = $"Date: {purchase.PurchaseDate:d}";
            TotalsText.Text = $"Grand Total: Rs. {purchase.GrandTotal:N2} | Paid: Rs. {purchase.PaidAmount:N2} | Balance: Rs. {purchase.BalanceAmount:N2}";
            LinesGrid.ItemsSource = purchase.Lines;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.GoBack();
        }

        private void AddPayment_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new AddPaymentPage(purchase));
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            // navigate to a simple return page or perform a quick return dialog
            this.NavigationService?.Navigate(new ReturnPurchasePage(purchase));
        }
    }
}
