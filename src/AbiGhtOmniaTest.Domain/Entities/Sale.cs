namespace AbiGhtOmniaTest.Domain.Entities;

public class Sale
{
    public Guid SaleId { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; } = false;
    public List<SaleItem> Items { get; set; } = new();

    public void CalculateTotalAmount()
    {
        TotalAmount = Items.Sum(item => item.TotalAmount);
    }
}

public class SaleItem
{
    public Guid SaleItemId { get; set; }
    public Guid SaleId { get; set; }
    public Sale Sale { get; set; } = null!;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }

    public void CalculateTotalAmount()
    {
        TotalAmount = (UnitPrice * Quantity) - Discount;
    }
}