namespace Application
{
    public abstract class AggregateRootBase
        : IAggregateRoot
    {
        public Guid Id { get; protected set; }
        public long Version { get; protected set; }
        public IList<IDomainEvent> Changes { get; }

        public AggregateRootBase()
        {
            Version = 0;
            Changes = new List<IDomainEvent>();
        }
    }
}
