using AbiGhtOmniaTest.Domain.Entities;
using MediatR;

namespace AbiGhtOmniaTest.Application.Queries;

public class GetSaleQuery(Guid saleId) : IRequest<Sale>
{
    public Guid SaleId { get; set; } = saleId;
}
