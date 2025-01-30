using AbiGhtOmniaTest.Domain.Entities;

namespace AbiGhtOmniaTest.Application.Interfaces;

public interface ISaleService
{
    Task<Guid> ExecuteAsync(Sale sale);
}