using AbiGhtOmniaTest.Application.Queries;
using AbiGhtOmniaTest.Domain.Entities;
using MediatR;
using MongoDB.Driver;

namespace AbiGhtOmniaTest.Application.Handlers;

public class GetUserByIdQueryHandler(IMongoClient mongoClient) : IRequestHandler<GetUserByIdQuery, User>
{
    private readonly IMongoClient _mongoClient = mongoClient;

    public async Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var database = _mongoClient.GetDatabase("DeveloperStore");
        var collection = database.GetCollection<User>("Users");
        var user = await collection.Find(u => u.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        return user;
    }
}
