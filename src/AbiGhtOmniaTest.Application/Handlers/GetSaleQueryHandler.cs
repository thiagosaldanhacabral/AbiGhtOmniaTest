using AbiGhtOmniaTest.Application.Queries;
using AbiGhtOmniaTest.Domain.Entities;
using MediatR;
using MongoDB.Driver;

namespace AbiGhtOmniaTest.Application.Handlers;

public class GetSaleQueryHandler : IRequestHandler<GetSaleQuery, Sale>
{
    private readonly IMongoCollection<Sale> _mongoCollection;

    public GetSaleQueryHandler(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("DeveloperStore");
        _mongoCollection = database.GetCollection<Sale>("Sales");
    }

    public async Task<Sale> Handle(GetSaleQuery request, CancellationToken cancellationToken)
    {
        var sale = await _mongoCollection.Find(s => s.SaleId == request.SaleId).FirstOrDefaultAsync(cancellationToken) ?? throw new Exception("Sale not found.");
        return sale;
    }
}
