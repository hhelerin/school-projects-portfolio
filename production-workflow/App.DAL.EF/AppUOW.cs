using App.DAL.Contracts;
using App.DAL.EF;
using App.DAL.EF.Repositories;
using Base.DAL.EF;


public class AppUOW : BaseUOW<AppDbContext>, IAppUOW
{
    public AppUOW(AppDbContext uowDbContext) : base(uowDbContext)
    {
    }

    private IMaterialRepository? _materialRepository;
    public IMaterialRepository MaterialRepository =>
        _materialRepository ??= new MaterialRepository(UOWDbContext);

    private ICustomersUsersRepository? _customersUsersRepository;
    public ICustomersUsersRepository CustomersUsersRepository =>
        _customersUsersRepository ??= new CustomersUsersRepository(UOWDbContext);

    private ICustomerRepository? _customerRepository;
    public ICustomerRepository CustomerRepository =>
        _customerRepository ??= new CustomerRepository(UOWDbContext);
    
    private IOrderRepository? _orderRepository;
    public IOrderRepository OrderRepository =>
        _orderRepository ??= new OrderRepository(UOWDbContext);
    
    private IOperationMappingRepository? _operationMappingRepository;
    public IOperationMappingRepository OperationMappingRepository =>
        _operationMappingRepository ??= new OperationMappingRepository(UOWDbContext);
    
    private IOrderItemRepository? _orderItemRepository;
    public IOrderItemRepository OrderItemRepository =>
        _orderItemRepository ??= new OrderItemRepository(UOWDbContext);
    
    private IProcessingStepRepository? _processingStepRepository;
    public IProcessingStepRepository ProcessingStepRepository =>
        _processingStepRepository ??= new ProcessingStepRepository(UOWDbContext);
    
    private IShipmentRepository? _shipmentRepository;
    public IShipmentRepository ShipmentRepository =>
        _shipmentRepository ??= new ShipmentRepository(UOWDbContext);
}