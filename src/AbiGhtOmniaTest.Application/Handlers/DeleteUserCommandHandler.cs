using AbiGhtOmniaTest.Application.Commands;
using AbiGhtOmniaTest.Domain.Interfaces;
using MediatR;

namespace AbiGhtOmniaTest.Application.Handlers
{
    public class DeleteUserCommandHandler(IUserRepository userRepository) : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.Id) ?? throw new Exception("User not found.");
            await _userRepository.DeleteUserAsync(user.Id);
        }
    }
}
