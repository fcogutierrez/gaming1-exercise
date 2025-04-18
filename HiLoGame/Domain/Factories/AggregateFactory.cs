using Domain.Contracts;
using Domain.Model.Base;

namespace Domain.Factories;

public sealed class AggregateFactory
{
    public static T Create<T>(IEnumerable<IDomainEvent> domainEvents)
        where T: AggregateBase
    {
        var ctor = typeof(T).GetConstructor([typeof(IList<IDomainEvent>)]);
        if (ctor is null)
        {
            throw new InvalidOperationException($"No constructor found for {typeof(T).Name} that takes IList<IDomainEvent>");
        }

        return (T)ctor.Invoke([domainEvents]);
    }
}
