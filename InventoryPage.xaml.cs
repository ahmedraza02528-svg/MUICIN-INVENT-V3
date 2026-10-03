using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System;
using System.Windows;
using System.ComponentModel;
using MUICIN_INVENT_V3.Services;

namespace MUICIN_INVENT_V3
{
    public partial class InventoryPage : Page
    {
        // Use central product service collection so Purchases and Inventory share the same products.
        private readonly ObservableCollection<ProductRow> allProducts = ProductService.Instance.Products;
        private ICollectionView productsView;

        public InventoryPage()
        {
            InitializeComponent();

            productsView = CollectionViewSource.GetDefaultView(allProducts);
            productsView.Filter = ProductFilter;

            if (this.FindName("ProductsDataGrid") is DataGrid dg) dg.ItemsSource = productsView;

            UpdateSummary();
        }

        private bool ProductFilter(object obj)
        {
            if (obj is ProductRow p)
            {
                string q = string.Empty;
                if (this.FindName("SearchTextBox") is TextBox tb) q = tb.Text?.Trim().ToLower() ?? string.Empty;
                if (string.IsNullOrEmpty(q)) return true;

                return (p.ProductName?.ToLower().Contains(q) == true)
                    || (p.SKU?.ToLower().Contains(q) == true)
                    || (p.Barcode?.ToLower().Contains(q) == true)
                    || (p.Category?.ToLower().Contains(q) == true)
                    || (p.Brand?.ToLower().Contains(q) == true);
            }
            return false;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            productsView?.Refresh();
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var w = new AddProductWindow();
            if (w.ShowDialog() == true)
            {
                // ensure SKU unique
                foreach (var ex in allProducts)
                {
                    if (string.Equals(ex.SKU, w.SKU, StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("SKU must be unique.", "Duplicate SKU", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var product = new ProductRow
                {
                    ProductId = ProductService.Instance.GenerateNewProductId(),
                    ProductName = w.ProductName,
                    SKU = w.SKU,
                    Barcode = w.Barcode,
                    Category = w.Category,
                    Brand = w.Brand,
                    SellingPrice = decimal.TryParse(w.SellingPrice, out var sp) ? sp : 0m,
                    CostPrice = decimal.TryParse(w.CostPrice, out var cp) ? cp : 0m,
                    DiscountPercent = decimal.TryParse(w.Discount, out var dp) ? dp : 0m,
                    Stock = decimal.TryParse(w.OpeningStock, out var st) ? st : 0m,
                    MinStockLevel = decimal.TryParse(w.MinStock, out var ms) ? ms : 0m,
                    Location = w.Location,
                    Batch = w.Batch,
                    ManufacturingDate = w.ManufacturingDate,
                    ExpiryDate = w.ExpiryDate,
                    Description = w.Description,
                    ImagePath = w.ImagePath,
                    Status = w.Status
                };

                allProducts.Add(product);
                UpdateSummary();
            }
        }

        private void ViewProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is ProductRow p)
            {
                string msg = $"Product: {p.ProductName}\nSKU: {p.SKU}\nStock: {p.Stock}\nLocation: {p.Location}\nBatch: {p.Batch}\nExpiry: {p.ExpiryDate:d}\nStatus: {p.Status}";
                MessageBox.Show(msg, "Product Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is ProductRow p)
            {
                var w = new AddProductWindow();
                w.LoadProduct(p);
                if (w.ShowDialog() == true)
                {
                    // update product
                    p.ProductName = w.ProductName;
                    p.SKU = w.SKU;
                    p.Barcode = w.Barcode;
                    p.Category = w.Category;
                    p.Brand = w.Brand;
                    p.SellingPrice = decimal.TryParse(w.SellingPrice, out var sp) ? sp : 0m;
                    p.CostPrice = decimal.TryParse(w.CostPrice, out var cp) ? cp : 0m;
                    p.DiscountPercent = decimal.TryParse(w.Discount, out var dp) ? dp : 0m;
                    p.Stock = decimal.TryParse(w.OpeningStock, out var st) ? st : p.Stock;
                    p.MinStockLevel = decimal.TryParse(w.MinStock, out var ms) ? ms : p.MinStockLevel;
                    p.Location = w.Location;
                    p.Batch = w.Batch;
                    p.ManufacturingDate = w.ManufacturingDate;
                    p.ExpiryDate = w.ExpiryDate;
                    p.Description = w.Description;
                    p.ImagePath = w.ImagePath;
                    p.Status = w.Status;

                    productsView?.Refresh();
                    UpdateSummary();
                }
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is ProductRow p)
            {
                var res = MessageBox.Show($"Delete product {p.ProductName}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    allProducts.Remove(p);
                    productsView?.Refresh();
                    UpdateSummary();
                }
            }
        }

        private void UpdateSummary()
        {
            if (this.FindName("TotalProductsText") is TextBlock tp) tp.Text = allProducts.Count.ToString();
            decimal totalStock = 0m; int lowCount = 0; int expiringCount = 0;
            var now = DateTime.Now;
            foreach (var p in allProducts)
            {
                totalStock += p.Stock;
                if (p.Stock <= p.MinStockLevel) lowCount++;
                if (p.ExpiryDate.HasValue)
                {
                    var days = (p.ExpiryDate.Value - now).TotalDays;
                    if (days <= 30 && days >= 0) expiringCount++;
                    if (days < 0) ; // expired - could be counted separately
                }
            }
            if (this.FindName("TotalStockText") is TextBlock ts) ts.Text = totalStock.ToString("N0");
            if (this.FindName("LowStockText") is TextBlock ls) ls.Text = lowCount.ToString();
            if (this.FindName("ExpiringSoonText") is TextBlock es) es.Text = expiringCount.ToString();
        }
    }

    public class ProductRow
    {
        public string ProductId { get; set; } = "";
        public string ProductName { get; set; } = "";
        public string SKU { get; set; } = "";
        public string Barcode { get; set; } = "";
        public string Category { get; set; } = "";
        public string Brand { get; set; } = "";
        public decimal SellingPrice { get; set; } = 0m;
        public decimal CostPrice { get; set; } = 0m;
        public decimal DiscountPercent { get; set; } = 0m;
        public decimal Stock { get; set; } = 0m;
        public decimal MinStockLevel { get; set; } = 0m;
        public string Location { get; set; } = "";
        public string Batch { get; set; } = "";
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Description { get; set; } = "";
        public string ImagePath { get; set; } = "";
        public string Status { get; set; } = "Normal";
    }
}