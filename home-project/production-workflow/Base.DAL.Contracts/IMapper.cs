using Contracts;

namespace Base.DAL.Contracts;

public interface IMapper<TDalEntity, TDomainEntity> : IMapper<TDalEntity, TDomainEntity, Guid>
    where TDalEntity : class, IDomainId
    where TDomainEntity : class, IDomainId
{
    
}

public interface IMapper<TDalEntity, TDomainEntity, TKey>
    where TKey : IEquatable<TKey>
    where TDalEntity : class, IDomainId<TKey>
    where TDomainEntity : class, IDomainId<TKey>
{
    public TDomainEntity? Map(TDalEntity? entity);
    public TDalEntity? Map(TDomainEntity? entity);
}