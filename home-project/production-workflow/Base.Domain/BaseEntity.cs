using Contracts;

namespace Base.Domain;

public abstract class BaseEntity : BaseEntity<Guid>, IDomainId
{
    public BaseEntity()
    {
        Id = Guid.NewGuid();
    }
}

public abstract class BaseEntity<TKey>: IDomainId<TKey>
    where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default!;
}