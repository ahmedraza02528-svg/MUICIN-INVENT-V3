using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MUICIN_INVENT_V3.Models;
using MUICIN_INVENT_V3.Services;

namespace MUICIN_INVENT_V3.Purchases
{
    public partial class NewPurchasePage : Page
    {
        private readonly ObservableCollection<PurchaseLine> lines = new ObservableCollection<PurchaseLine>();

        private Purchase current;

        public NewPurchasePage(Models.Purchase editPurchase = null)
        {
            InitializeComponent();

            // populate product selector from central ProductService
            ProductSelector.ItemsSource = ProductService.Instance.Products;

            LinesDataGrid.ItemsSource = lines;

            SupplierComboBox.ItemsSource = SupplierService.Instance.Suppliers;

            if (editPurchase != null)
            {
                // edit existing purchase
                current = editPurchase;
                PurchaseNumberTextBox.Text = current.PurchaseNumber;
                PurchaseDatePicker.SelectedDate = current.PurchaseDate;
                if (!string.IsNullOrEmpty(current.LocationName)) LocationTextBox.Text = current.LocationName;
                NotesTextBox.Text = current.Notes;
                foreach (var l in current.Lines) lines.Add(l);
                RecalculateTotals();
            }
            else
            {
                current = PurchaseService.Instance.CreateDraftPurchase();
                PurchaseNumberTextBox.Text = current.PurchaseNumber;
                PurchaseDatePicker.SelectedDate = current.PurchaseDate;
            }
        }

        private void AddLine_Click(object sender, RoutedEventArgs e)
        {
            if (ProductSelector.SelectedItem is ProductRow prod)
            {
                if (!decimal.TryParse(QtyTextBox.Text.Trim(), out var qty) || qty <= 0)
                {
                    MessageBox.Show("Quantity must be a positive number.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(CostTextBox.Text.Trim(), out var cost) || cost < 0)
                {
                    MessageBox.Show("Cost must be numeric.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                decimal disc = 0m;
                decimal.TryParse(LineDiscountTextBox.Text.Trim(), out disc);

                var line = new PurchaseLine
                {
                    LineId = Guid.NewGuid().ToString(),
                    ProductId = prod.ProductId,
                    SKU = prod.SKU,
                    ProductName = prod.ProductName,
                    ProductImagePath = prod.ImagePath,
                    Quantity = qty,
                    CostPrice = cost,
                    DiscountPercent = disc,
                    DiscountAmount = Math.Round(cost * qty * (disc / 100m), 2),
                    LineTotal = Math.Round((cost * qty) - (cost * qty * (disc / 100m)), 2),
                    BatchNumber = prod.Batch,
                    ExpiryDate = prod.ExpiryDate,
                    LocationId = LocationTextBox.Text.Trim()
                };

                lines.Add(line);
                RecalculateTotals();
            }
            else
            {
                MessageBox.Show("Please select a product.", "Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RecalculateTotals()
        {
            var subtotal = lines.Sum(x => x.LineTotal);
            SubtotalText.Text = $"Rs. {subtotal:N2}";
            GrandTotalText.Text = $"Rs. {subtotal:N2}";
            current.Subtotal = subtotal;
            current.GrandTotal = subtotal;
        }

        private void SaveDraft_Click(object sender, RoutedEventArgs e)
        {
            current.PurchaseDate = PurchaseDatePicker.SelectedDate ?? DateTime.Now;
            if (SupplierComboBox.SelectedItem is SupplierRow s)
            {
                current.SupplierId = s.SupplierId;
                current.SupplierName = s.SupplierName;
            }
            current.SupplierInvoiceNumber = SupplierInvoiceTextBox.Text.Trim();
            current.LocationId = LocationTextBox.Text.Trim();
            current.LocationName = LocationTextBox.Text.Trim();
            current.Notes = NotesTextBox.Text.Trim();
            current.Lines = lines.ToList();

            // totals already set
            // ensure purchase is in central store
            if (!PurchaseService.Instance.Purchases.Contains(current))
            {
                PurchaseService.Instance.AddPurchase(current);
            }

            MessageBox.Show("Purchase saved as draft.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PostPurchase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // finalize fields
                current.PurchaseDate = PurchaseDatePicker.SelectedDate ?? DateTime.Now;
                if (SupplierComboBox.SelectedItem is SupplierRow s)
                {
                    current.SupplierId = s.SupplierId;
                    current.SupplierName = s.SupplierName;
                }
                current.SupplierInvoiceNumber = SupplierInvoiceTextBox.Text.Trim();
                current.LocationId = LocationTextBox.Text.Trim();
                current.LocationName = LocationTextBox.Text.Trim();
                current.Notes = NotesTextBox.Text.Trim();
                current.Lines = lines.ToList();

                PurchaseService.Instance.PostPurchase(current);
                MessageBox.Show("Purchase posted and stock updated.", "Posted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to post purchase: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
