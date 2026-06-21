using SchedulingLib.Core.Results;
using SchedulingLib.Services.Interfaces;
using SchedulingLib.Services.ValueObjects;

namespace SchedulingLib.Services.Services;

/// <summary>
/// Default implementation of <see cref="IServiceTypeService"/>.
/// </summary>
public sealed class ServiceTypeService(IServiceTypeRepository repository) : IServiceTypeService
{
    /// <inheritdoc />
    public async Task<Result<ServiceType>> CreateAsync(string name, decimal price, TimeSpan duration, CancellationToken cancellationToken = default)
    {
        var serviceType = new ServiceType(Guid.NewGuid(), name, price, duration);
        await repository.SaveAsync(serviceType, cancellationToken);
        return Result.Ok(serviceType);
    }

    /// <inheritdoc />
    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return Result.Fail<bool>($"Service type '{id}' not found.");

        await repository.DeleteAsync(id, cancellationToken);
        return Result.Ok(true);
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<ServiceType>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var all = await repository.GetAllAsync(cancellationToken);
        return Result.Ok(all);
    }
}
