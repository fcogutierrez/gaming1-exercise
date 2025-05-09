﻿using Domain.Contracts;
using Domain.Model.Entities;
using Domain.Model.ValueObjects;

namespace Domain.Events;

internal sealed record GameCreatedEvent(Guid AggregateId, MisteryNumber MisteryNumber) : IDomainEvent;
