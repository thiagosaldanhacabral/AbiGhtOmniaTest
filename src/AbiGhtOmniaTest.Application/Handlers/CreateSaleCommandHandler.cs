using AbiGhtOmniaTest.Application.Commands;
using AbiGhtOmniaTest.Domain.Entities;
using AbiGhtOmniaTest.Domain.Interfaces;
using AutoMapper;
using MediatR;
using Serilog;

namespace AbiGhtOmniaTest.Application.Handlers;

public class CreateSaleCommandHandler(ISaleRepository saleRepository, IMapper mapper, ILogger logger) : IRequestHandler<CreateSaleCommand, Guid>
{
    private readonly ISaleRepository _saleRepository = saleRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger _logger = logger;

    public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var sale = _mapper.Map<Sale>(request);

            foreach (var item in sale.Items)
            {
                if (item.Quantity > 20)
                    throw new Exception("Cannot sell more than 20 items of a single product.");

                item.Discount = item.Quantity >= 10 ? item.Quantity * item.UnitPrice * 0.20m : item.Quantity >= 4 ? item.Quantity * item.UnitPrice * 0.10m : 0;
                item.CalculateTotalAmount();
            }

            sale.CalculateTotalAmount();
            return await _saleRepository.AddSaleAsync(sale);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while creating sale");
            throw;
        }
    }
}
