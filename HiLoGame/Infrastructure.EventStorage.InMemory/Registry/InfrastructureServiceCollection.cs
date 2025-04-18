using Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.EventStorage.InMemory.Registry;

public static class InfrastructureServiceCollection
{
    public static IServiceCollection AddInMemoryEventStorage(this IServiceCollection services) =>
        services
            .AddSingleton<IEventStorage, InMemoryEventStorage>();
}
