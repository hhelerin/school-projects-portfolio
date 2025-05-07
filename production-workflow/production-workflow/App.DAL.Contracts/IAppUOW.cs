using Base.DAL.Contracts;

namespace App.DAL.Contracts;

public interface IAppUOW : IBaseUOW
{
    IMaterialRepository MaterialRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    ICustomersUsersRepository CustomersUsersRepository { get; }
    IOperationMappingRepository OperationMappingRepository { get; }
    IOrderRepository OrderRepository { get; }
    IOrderItemRepository OrderItemRepository { get; }
    IProcessingStepRepository ProcessingStepRepository { get; }
    IShipmentRepository ShipmentRepository { get; }
}