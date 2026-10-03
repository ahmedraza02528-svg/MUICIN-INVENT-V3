using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace MUICIN_INVENT_V3
{
    public partial class AddProductWindow : Window
    {
        public string ProductName { get; private set; } = "";
        public string SKU { get; private set; } = "";
        public string Barcode { get; private set; } = "";
        public string Category { get; private set; } = "";
        public string Brand { get; private set; } = "";
        public string SellingPrice { get; private set; } = "0";
        public string CostPrice { get; private set; } = "0";
        public string Discount { get; private set; } = "0";
        public string OpeningStock { get; private set; } = "0";
        public string MinStock { get; private set; } = "0";
        public string Location { get; private set; } = "";
        public string Batch { get; private set; } = "";
        public DateTime? ManufacturingDate { get; private set; }
        public DateTime? ExpiryDate { get; private set; }
        public string Description { get; private set; } = "";
        public string ImagePath { get; private set; } = "";
        public string Status { get; private set; } = "Active";

        public AddProductWindow()
        {
            InitializeComponent();
        }

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp";
            if (dlg.ShowDialog() == true)
            {
                ImagePath = dlg.FileName;
                ProductImagePreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(ImagePath));
            }
        }

        public void LoadProduct(object p)
        {
            if (p == null) return;
            try
            {
                dynamic d = p;
                ProductNameTextBox.Text = d.ProductName ?? "";
                SKUTextBox.Text = d.SKU ?? "";
                BarcodeTextBox.Text = d.Barcode ?? "";
                CategoryTextBox.Text = d.Category ?? "";
                BrandTextBox.Text = d.Brand ?? "";
                SellingPriceTextBox.Text = (d.SellingPrice != null) ? d.SellingPrice.ToString() : "0";
                CostPriceTextBox.Text = (d.CostPrice != null) ? d.CostPrice.ToString() : "0";
                DiscountTextBox.Text = (d.DiscountPercent != null) ? d.DiscountPercent.ToString() : "0";
                OpeningStockTextBox.Text = (d.Stock != null) ? d.Stock.ToString() : "0";
                MinStockTextBox.Text = (d.MinStockLevel != null) ? d.MinStockLevel.ToString() : "0";
                LocationTextBox.Text = d.Location ?? "";
                BatchTextBox.Text = d.Batch ?? "";
                ManufacturingDatePicker.SelectedDate = d.ManufacturingDate;
                ExpiryDatePicker.SelectedDate = d.ExpiryDate;
                DescriptionTextBox.Text = d.Description ?? "";
                if (!string.IsNullOrEmpty((string)d.ImagePath))
                {
                    ImagePath = d.ImagePath;
                    ProductImagePreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(ImagePath));
                }
            }
            catch { }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
            {
                MessageBox.Show("Please enter Product Name.", "Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                ProductNameTextBox.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(SKUTextBox.Text))
            {
                MessageBox.Show("Please enter SKU.", "Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                SKUTextBox.Focus();
                return;
            }

            if (!decimal.TryParse(SellingPriceTextBox.Text.Trim(), out var sp))
            {
                MessageBox.Show("Selling Price must be numeric.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                SellingPriceTextBox.Focus();
                return;
            }
            if (!decimal.TryParse(CostPriceTextBox.Text.Trim(), out var cp))
            {
                MessageBox.Show("Cost Price must be numeric.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                CostPriceTextBox.Focus();
                return;
            }
            if (!decimal.TryParse(OpeningStockTextBox.Text.Trim(), out var st))
            {
                MessageBox.Show("Opening Stock must be numeric.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                OpeningStockTextBox.Focus();
                return;
            }

            ProductName = ProductNameTextBox.Text.Trim();
            SKU = SKUTextBox.Text.Trim();
            Barcode = BarcodeTextBox.Text.Trim();
            Category = CategoryTextBox.Text.Trim();
            Brand = BrandTextBox.Text.Trim();
            SellingPrice = sp.ToString();
            CostPrice = cp.ToString();
            Discount = DiscountTextBox.Text.Trim();
            OpeningStock = st.ToString();
            MinStock = MinStockTextBox.Text.Trim();
            Location = LocationTextBox.Text.Trim();
            Batch = BatchTextBox.Text.Trim();
            ManufacturingDate = ManufacturingDatePicker.SelectedDate;
            ExpiryDate = ExpiryDatePicker.SelectedDate;
            Description = DescriptionTextBox.Text.Trim();

            if (StatusComboBox.SelectedItem is ComboBoxItem si)
                Status = si.Content?.ToString() ?? "Active";

            DialogResult = true;
            Close();
        }
    }
}
