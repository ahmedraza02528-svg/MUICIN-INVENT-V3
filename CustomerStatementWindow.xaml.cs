using System;
using System.Windows;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Globalization;

namespace MUICIN_INVENT_V3
{
    public partial class CustomerStatementWindow : Window
    {
        private string currentCustomerId = string.Empty;
        private string currentCustomerName = string.Empty;
        private decimal currentOpening = 0m;
        private decimal currentPayments = 0m;
        private int currentOrders = 0;

        public CustomerStatementWindow()
        {
            InitializeComponent();
        }

        private void ViewLastInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            // For now, we don't have sales/invoice data. Show a message if no invoice is available.
            MessageBox.Show("No invoice is available for this customer.", "View Last Invoice", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PrintStatementButton_Click(object sender, RoutedEventArgs e)
        {
            // Prepare the UI for printing using WPF PrintDialog and a simple A4 layout
            var pd = new System.Windows.Controls.PrintDialog();
            if (pd.ShowDialog() == true)
            {
                // Render the window content for printing
                pd.PrintVisual(this.Content as System.Windows.Media.Visual, $"Customer Statement - {currentCustomerName}");
            }
        }

        private void SaveStatementButton_Click(object sender, RoutedEventArgs e)
        {
            // Structure ready: serialize current statement to a JSON file in Documents folder
            try
            {
                var obj = new
                {
                    CustomerId = currentCustomerId,
                    CustomerName = currentCustomerName,
                    From = DateFromPicker.SelectedDate,
                    To = DateToPicker.SelectedDate,
                    Opening = currentOpening,
                    Orders = currentOrders,
                    Payments = currentPayments
                };
                var json = System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"Statement_{currentCustomerId}_{DateTime.Now:yyyyMMddHHmmss}.json");
                System.IO.File.WriteAllText(path, json);
                MessageBox.Show($"Statement saved to {path}", "Save Statement", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save statement: {ex.Message}", "Save Statement", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SavePdfButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a placeholder "PDF" export: prompt for filename but avoid introducing PDF libraries.
            try
            {
                var dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.Filter = "PDF Files|*.pdf";
                dlg.FileName = $"Statement_{currentCustomerId}_{DateTime.Now:yyyyMMdd}.pdf";
                if (dlg.ShowDialog() == true && !string.IsNullOrWhiteSpace(dlg.FileName))
                {
                    // Use QuestPDF to generate a professional A4 PDF
                    GeneratePdf(dlg.FileName);
                    MessageBox.Show($"PDF generated to {dlg.FileName}", "Save PDF", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save PDF placeholder: {ex.Message}", "Save PDF", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadCustomer(string id, string name, string phone, string address, decimal openingBalance, int totalOrders, decimal totalPayments)
        {
            currentCustomerId = id;
            currentCustomerName = name;
            currentOpening = openingBalance;
            currentOrders = totalOrders;
            currentPayments = totalPayments;

            StatementCustomerName.Text = name + " (" + id + ")";
            StatementCustomerInfo.Text = $"Phone: {phone} | Address: {address}";

            // Initialize date range to last 30 days by default
            DateToPicker.SelectedDate = DateTime.Today;
            DateFromPicker.SelectedDate = DateTime.Today.AddDays(-30);

            // Populate header summary info into the DataGrid footer area via a simple initial load
            RefreshStatement();
        }

        private void RefreshStatement()
        {
            // For now, create an empty set of transactions since sales/payments modules may be unconnected.
            var items = new System.Collections.ObjectModel.ObservableCollection<StatementRow>();

            // Opening balance row
            decimal running = currentOpening;
            items.Add(new StatementRow { Date = DateFromPicker.SelectedDate?.ToString("yyyy-MM-dd") ?? "", Type = "Opening", Reference = "", Debit = running != 0m ? running.ToString("N2") : "", Credit = "", Balance = running.ToString("N2") });

            // No transactions available yet - leave list with opening only

            StatementDataGrid.ItemsSource = items;
        }

        private class StatementRow
        {
            public string Date { get; set; }
            public string Type { get; set; }
            public string Reference { get; set; }
            public string Debit { get; set; }
            public string Credit { get; set; }
            public string Balance { get; set; }
        }

        private void GeneratePdf(string filename)
        {
            var fromDate = DateFromPicker.SelectedDate ?? DateTime.Today.AddDays(-30);
            var toDate = DateToPicker.SelectedDate ?? DateTime.Today;

            using (var document = new PdfDocument())
            {
                var page = document.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;
                page.Orientation = PdfSharpCore.PageOrientation.Portrait;

                var gfx = XGraphics.FromPdfPage(page);
                var regular = new XFont("Arial", 12, XFontStyle.Regular);
                var bold = new XFont("Arial", 14, XFontStyle.Bold);

                double y = 40;
                gfx.DrawString("MUICIN", bold, XBrushes.Black, new XRect(40, y, page.Width.Point - 80, 20), XStringFormats.TopLeft);
                y += 24;
                gfx.DrawString("Customer Statement", regular, XBrushes.Black, new XRect(40, y, page.Width.Point - 80, 20), XStringFormats.TopLeft);
                y += 28;

                gfx.DrawString($"Customer: {currentCustomerName}", regular, XBrushes.Black, new XRect(40, y, 350, 18), XStringFormats.TopLeft);
                gfx.DrawString($"ID: {currentCustomerId}", regular, XBrushes.Black, new XRect(400, y, 200, 18), XStringFormats.TopLeft);
                y += 18;
                gfx.DrawString(StatementCustomerInfo.Text, regular, XBrushes.Gray, new XRect(40, y, page.Width.Point - 80, 18), XStringFormats.TopLeft);
                y += 20;

                gfx.DrawString($"From: {fromDate:yyyy-MM-dd}", regular, XBrushes.Black, new XRect(40, y, 200, 18), XStringFormats.TopLeft);
                gfx.DrawString($"To:   {toDate:yyyy-MM-dd}", regular, XBrushes.Black, new XRect(240, y, 200, 18), XStringFormats.TopLeft);
                y += 20;

                gfx.DrawString($"Opening Balance: Rs. {currentOpening:N2}", regular, XBrushes.Black, new XRect(40, y, 300, 18), XStringFormats.TopLeft);
                gfx.DrawString($"Total Purchases: {currentOrders}", regular, XBrushes.Black, new XRect(300, y, 200, 18), XStringFormats.TopLeft);
                gfx.DrawString($"Total Payments: Rs. {currentPayments:N2}", regular, XBrushes.Black, new XRect(460, y, 200, 18), XStringFormats.TopLeft);
                y += 22;

                var outstanding = currentOpening + currentOrders - currentPayments;
                gfx.DrawString($"Outstanding Balance: Rs. {outstanding:N2}", bold, XBrushes.Black, new XRect(40, y, 400, 18), XStringFormats.TopLeft);
                y += 28;

                // Table header
                double x = 40;
                double[] colWidths = { 80, 80, 80, 180, 80, 80 };
                string[] headers = { "Date", "Type", "Invoice No.", "Description", "Debit", "Credit" };

                for (int i = 0; i < headers.Length; i++)
                {
                    gfx.DrawString(headers[i], regular, XBrushes.Black, new XRect(x, y, colWidths[i], 16), XStringFormats.TopLeft);
                    x += colWidths[i];
                }
                y += 18;

                // No transactions available
                gfx.DrawString("No transactions available for the selected period.", regular, XBrushes.Gray, new XRect(40, y, page.Width.Point - 80, 18), XStringFormats.TopLeft);
                y += 24;

                // Footer
                gfx.DrawString($"Generated by MUICIN Inventory V3 - {DateTime.Now:yyyy-MM-dd HH:mm}", regular, XBrushes.Gray, new XRect(40, page.Height.Point - 40, page.Width.Point - 80, 18), XStringFormats.Center);

                // Save PDF
                using (var stream = System.IO.File.Create(filename))
                {
                    // PdfSharpCore supports saving directly to stream via document.Save
                    document.Save(stream);
                }
            }
        }
    }
}
