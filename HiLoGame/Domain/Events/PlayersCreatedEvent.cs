using Domain.Contracts;
using Domain.Model.Entities;

namespace Domain.Events;

internal sealed record PlayersCreatedEvent(Guid Id, List<Player> Players) : IDomainEvent;
