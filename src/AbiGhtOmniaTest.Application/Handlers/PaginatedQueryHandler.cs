using AbiGhtOmniaTest.Application.Queries;
using AbiGhtOmniaTest.Domain.Entities;
using MediatR;
using MongoDB.Driver;

namespace AbiGhtOmniaTest.Application.Handlers;

public class PaginatedQueryHandler : IRequestHandler<PaginatedQuery, PaginatedResult<Sale>>
{
    private readonly IMongoCollection<Sale> _mongoCollection;

    public PaginatedQueryHandler(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("DeveloperStore");
        _mongoCollection = database.GetCollection<Sale>("Sales");
    }

    public async Task<PaginatedResult<Sale>> Handle(PaginatedQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await _mongoCollection.CountDocumentsAsync(_ => true, cancellationToken: cancellationToken);
        var items = await _mongoCollection.Find(_ => true)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Limit(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Sale>
        {
            Items = items,
            TotalCount = (int)totalCount
        };
    }
}
