namespace PMSCore.ViewModel;

public class OrderDetailsVM
{
    public int OrderId { get; set; }
    public DateOnly OrderDate { get; set; }
    public string CustomerName { get; set; }
    public string OrderStatus { get; set; }

    public string PaymentType { get; set; }
    public short? Rating { get; set; } = 0;
    public decimal TotalAmount { get; set; }
}

public class OrderExportDetails
{
    public int OrderId { get; set; }
    public string Status { get; set; }
    public string InvoiceNo { get; set; }
    public DateTime PaidOn { get; set; }
    public DateTime PlacedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public TimeOnly OrderDuration { get; set; }
    public string Tables { get; set; }
    public string Section { get; set; }
    public string PaymentMethod { get; set; }
    public decimal SubTotal { get; set; }
    public CustomerDetails CustomerInfo { get; set; }
    public List<OrderItemHelperModel> OrderItems { get; set; } = new List<OrderItemHelperModel>();
    public List<TaxDetailsHelperModel> taxDetails { get; set; } = new List<TaxDetailsHelperModel>();
    public decimal TotalAmountToPay { get; set; }
    public class OrderItemHelperModel
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderModiferHelperModel> Modifiers { get; set; }
        public class OrderModiferHelperModel
        {
            public string ModifierName { get; set; }
            public int ModiferQuantity { get; set; }
            public decimal ModifierPrice { get; set; }
            public decimal ModifierTotalPrice { get; set; }
        }
    }
    public class TaxDetailsHelperModel
    {
        public string TaxName { get; set; }
        public decimal TaxValue { get; set; }
    }
}

