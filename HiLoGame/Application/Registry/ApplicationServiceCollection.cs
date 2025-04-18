using Application.CommandHandlers;
using Application.Commands;
using Application.Contracts;
using Application.Providers;
using Domain.Contracts;
using Microsoft.Extensions.DependencyInjection;
using OneOf;

namespace Application.Registry;

using GuessMisteryNumberCommandResult = OneOf<NextTurnCommandResult, MisteryNumberGuessedCommandResult>;

public static class ApplicationServiceCollection
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddSingleton<ICommandHandler<CreateGameCommand, CreateGameCommandResult>, CreateGameCommandHandler>()
            .AddSingleton<ICommandHandler<AddPlayersCommand, AddPlayersCommandResult>, AddPlayersCommandHandler>()
            .AddSingleton<ICommandHandler<GuessMisteryNumberCommand, GuessMisteryNumberCommandResult>, GuessMisteryNumberCommandHandler>();

    public static IServiceCollection AddProviders(this IServiceCollection services) =>
        services
            .AddSingleton<IRandomProvider, DotNetRandomProvider>();
}
