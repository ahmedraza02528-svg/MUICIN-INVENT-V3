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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MUICIN_INVENT_V3
{
    /// <summary>
    /// Interaction logic for PurchasesPage.xaml
    /// </summary>
    public partial class PurchasesPage : Page
    {
        public PurchasesPage()
        {
            InitializeComponent();
        }

        private void NewPurchase_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to NewPurchasePage inside the current frame so sidebar/top remain visible
            this.NavigationService?.Navigate(new MUICIN_INVENT_V3.Purchases.NewPurchasePage());
        }
    }
}
