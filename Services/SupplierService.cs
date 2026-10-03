using System.Collections.ObjectModel;

namespace MUICIN_INVENT_V3.Services
{
    public class SupplierService
    {
        private static SupplierService _instance;
        public static SupplierService Instance => _instance ??= new SupplierService();

        public ObservableCollection<SupplierRow> Suppliers { get; } = new ObservableCollection<SupplierRow>();

        private int nextSupplierId = 1;

        private SupplierService() { }

        public string GenerateSupplierId()
        {
            return $"SUP-{nextSupplierId++:0000}";
        }
    }
}
