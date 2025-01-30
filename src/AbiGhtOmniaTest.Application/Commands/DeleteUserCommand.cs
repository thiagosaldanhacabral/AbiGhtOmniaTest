using MediatR;

namespace AbiGhtOmniaTest.Application.Commands
{
    public class DeleteUserCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
