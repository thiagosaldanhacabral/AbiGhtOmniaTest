using MediatR;

namespace AbiGhtOmniaTest.Application.Commands;

public class CreateSaleCommand : IRequest<Guid>
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public List<SaleItemDto> Items { get; set; } = new();
}

public class SaleItemDto
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
