using CSharpFunctionalExtensions;
using Products.Core.Database;
using Shared.Core.Abstractions;
using Shared.Kernel;

namespace Products.Core.Features.Commands;

public record DeleteProductCommand(Guid ProductId) : ICommand;

/*public sealed class DeleteProductHandler(IProductsRepository productsRepository) : ICommandHandler<DeleteProductCommand>
{
    public async Task<UnitResult<Error>> Handle(DeleteProductCommand command, CancellationToken ct = default)
    {

    }
}*/