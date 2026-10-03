using System;
using System.Collections.ObjectModel;
using System.Linq;
using MUICIN_INVENT_V3.Models;
using MUICIN_INVENT_V3.Services;

namespace MUICIN_INVENT_V3.Services
{
    public class PurchaseService
    {
        private static PurchaseService _instance;
        public static PurchaseService Instance => _instance ??= new PurchaseService();

        public ObservableCollection<Purchase> Purchases { get; } = new ObservableCollection<Purchase>();
        public ObservableCollection<StockMovement> StockMovements { get; } = new ObservableCollection<StockMovement>();

        private int nextPurchaseNumber = 1;
        private int nextPurchaseId = 1;
        private int nextMovementId = 1;
        private int nextPaymentId = 1;

        private PurchaseService() { }

        public string GeneratePurchaseNumber()
        {
            return $"PUR-{nextPurchaseNumber++:000000}";
        }

        public string GeneratePurchaseId()
        {
            return $"P-{nextPurchaseId++:000000}";
        }

        public string GenerateMovementId()
        {
            return $"M-{nextMovementId++:000000}";
        }

        public string GeneratePaymentId()
        {
            return $"PMT-{nextPaymentId++:000000}";
        }

        public Purchase CreateDraftPurchase()
        {
            var p = new Purchase
            {
                PurchaseId = GeneratePurchaseId(),
                PurchaseNumber = GeneratePurchaseNumber(),
                PurchaseDate = DateTime.Now,
                PurchaseStatus = PurchaseStatus.Draft,
                IsPosted = false
            };
            Purchases.Add(p);
            return p;
        }

        public void AddPurchase(Purchase p)
        {
            if (string.IsNullOrWhiteSpace(p.PurchaseId)) p.PurchaseId = GeneratePurchaseId();
            if (string.IsNullOrWhiteSpace(p.PurchaseNumber)) p.PurchaseNumber = GeneratePurchaseNumber();
            Purchases.Add(p);
        }

        public void PostPurchase(Purchase p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            if (p.IsPosted) throw new InvalidOperationException("Purchase already posted.");

            // Basic validation
            if (p.Lines == null || p.Lines.Count == 0) throw new InvalidOperationException("Purchase must have at least one line.");

            // Calculate totals
            p.Subtotal = p.Lines.Sum(l => l.LineTotal);
            p.GrandTotal = p.Subtotal - p.DiscountAmount + p.TaxAmount;
            p.BalanceAmount = p.GrandTotal - p.PaidAmount;

            // Apply stock movements and update product stock exactly once
            foreach (var line in p.Lines)
            {
                var prod = ProductService.Instance.Products.FirstOrDefault(x => x.ProductId == line.ProductId || string.Equals(x.SKU, line.SKU, StringComparison.OrdinalIgnoreCase));
                if (prod == null) throw new InvalidOperationException($"Product not found: {line.ProductName} ({line.SKU})");

                // create movement
                var mov = new StockMovement
                {
                    MovementId = GenerateMovementId(),
                    DateTime = DateTime.Now,
                    ProductId = prod.ProductId,
                    SKU = prod.SKU,
                    LocationId = p.LocationId,
                    MovementType = MovementType.Purchase,
                    QuantityIn = line.Quantity,
                    QuantityOut = 0,
                    ReferenceType = "Purchase",
                    ReferenceNumber = p.PurchaseNumber,
                    BatchNumber = line.BatchNumber,
                    ExpiryDate = line.ExpiryDate,
                    User = Environment.UserName,
                    Notes = p.Notes
                };

                // update product stock (in-memory)
                prod.Stock += line.Quantity;

                StockMovements.Add(mov);
            }

            p.IsPosted = true;
            p.PurchaseStatus = PurchaseStatus.Posted;
            p.UpdatedAt = DateTime.Now;
        }

        public void AddPayment(Purchase p, PurchasePayment payment)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            if (payment == null) throw new ArgumentNullException(nameof(payment));

            payment.PaymentId = payment.PaymentId ?? GeneratePaymentId();
            payment.Date = payment.Date == default ? DateTime.Now : payment.Date;
            p.Payments.Add(payment);

            p.PaidAmount = p.Payments.Sum(x => x.Amount);
            p.BalanceAmount = Math.Max(0, p.GrandTotal - p.PaidAmount);

            if (p.PaidAmount == 0) p.PaymentStatus = PaymentStatus.Unpaid;
            else if (p.PaidAmount < p.GrandTotal) p.PaymentStatus = PaymentStatus.PartiallyPaid;
            else p.PaymentStatus = PaymentStatus.Paid;
        }
    }
}
