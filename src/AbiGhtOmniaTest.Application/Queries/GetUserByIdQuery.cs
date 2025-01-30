using AbiGhtOmniaTest.Domain.Entities;
using MediatR;

namespace AbiGhtOmniaTest.Application.Queries;

public class GetUserByIdQuery(Guid id) : IRequest<User>
{
    public Guid Id { get; set; } = id;
}
