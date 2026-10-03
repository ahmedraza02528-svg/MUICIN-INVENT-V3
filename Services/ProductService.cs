using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MUICIN_INVENT_V3.Services
{
    // Central in-memory product store to be shared across modules.
    public class ProductService
    {
        private static ProductService _instance;
        public static ProductService Instance => _instance ??= new ProductService();

        // Observable collection so UI can bind and observe changes.
        public ObservableCollection<ProductRow> Products { get; } = new ObservableCollection<ProductRow>();

        private int nextId = 1;

        private ProductService() { }

        public string GenerateNewProductId()
        {
            return $"PRD-{nextId++:0000}";
        }

        public ProductRow FindBySKU(string sku)
        {
            return Products.FirstOrDefault(p => string.Equals(p.SKU, sku, StringComparison.OrdinalIgnoreCase));
        }
    }
}
