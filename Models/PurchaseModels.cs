using System;
using System.Collections.Generic;

namespace MUICIN_INVENT_V3.Models
{
    public enum PurchaseStatus { Draft, Posted, Cancelled, Returned }
    public enum PaymentStatus { Unpaid, PartiallyPaid, Paid }
    public enum MovementType { Purchase, PurchaseReturn }

    public class Purchase
    {
        public string PurchaseId { get; set; }
        public string PurchaseNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierInvoiceNumber { get; set; }
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public PurchaseStatus PurchaseStatus { get; set; } = PurchaseStatus.Draft;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsPosted { get; set; }

        public List<PurchaseLine> Lines { get; set; } = new List<PurchaseLine>();
        public List<PurchasePayment> Payments { get; set; } = new List<PurchasePayment>();
    }

    public class PurchaseLine
    {
        public string LineId { get; set; }
        public string ProductId { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string ProductImagePath { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal LineTotal { get; set; }
        public string LocationId { get; set; }
    }

    public class PurchasePayment
    {
        public string PaymentId { get; set; }
        public string PurchaseId { get; set; }
        public string SupplierId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Account { get; set; }
        public string Reference { get; set; }
        public string Notes { get; set; }
    }

    public class StockMovement
    {
        public string MovementId { get; set; }
        public DateTime DateTime { get; set; }
        public string ProductId { get; set; }
        public string SKU { get; set; }
        public string LocationId { get; set; }
        public MovementType MovementType { get; set; }
        public decimal QuantityIn { get; set; }
        public decimal QuantityOut { get; set; }
        public string ReferenceType { get; set; }
        public string ReferenceNumber { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string User { get; set; }
        public string Notes { get; set; }
    }

    public class ProductBatch
    {
        public string BatchId { get; set; }
        public string ProductId { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string LocationId { get; set; }
        public decimal Quantity { get; set; }
    }
}
