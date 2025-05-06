using App.DAL.DTO;
using App.Domain;
using Base.DAL.Contracts;

namespace App.DAL.Contracts;

public interface IOrderItemRepository : IBaseRepository<OrderItemDto>
{
    
}