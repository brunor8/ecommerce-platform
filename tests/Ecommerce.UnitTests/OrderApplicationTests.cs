using Ecommerce.Application.Common.Abstractions;
using Ecommerce.Application.Common.Validation;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Commands;
using Ecommerce.Application.Orders.Exceptions;
using Ecommerce.Application.Orders.Handlers;
using Ecommerce.Application.Orders.Queries;
using Ecommerce.Application.Orders.Validators;
using Ecommerce.Domain.Orders;

namespace Ecommerce.UnitTests.Application;

public class OrderApplicationTests
{
    [Fact]
    public async Task CreateOrder_ClienteValido_CriaPedidoEPersiste()
    {
        var repository = new InMemoryOrderRepository();
        var unitOfWork = new InMemoryUnitOfWork();
        var handler = new CreateOrderCommandHandler(
            repository,
            unitOfWork,
            new CreateOrderCommandValidator());
        var customerId = Guid.NewGuid();

        var result = await handler.HandleAsync(new CreateOrderCommand(customerId));

        Assert.Equal(customerId, result.CustomerId);
        Assert.Equal(nameof(OrderStatus.Pending), result.Status);
        Assert.Single(repository.Orders);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task CreateOrder_ClienteInvalido_LancaValidationExceptionSemPersistir()
    {
        var repository = new InMemoryOrderRepository();
        var unitOfWork = new InMemoryUnitOfWork();
        var handler = new CreateOrderCommandHandler(
            repository,
            unitOfWork,
            new CreateOrderCommandValidator());

        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => handler.HandleAsync(new CreateOrderCommand(Guid.Empty)));

        Assert.Contains(exception.Errors, error => error.Code == "customer_id.required");
        Assert.Empty(repository.Orders);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task AddOrderItem_ComandoValido_AdicionaItemEPersiste()
    {
        var (repository, unitOfWork, order) = CreateOrder();
        var handler = new AddOrderItemCommandHandler(
            repository,
            unitOfWork,
            new AddOrderItemCommandValidator());
        var productId = Guid.NewGuid();

        var result = await handler.HandleAsync(
            new AddOrderItemCommand(order.Id, productId, "Teclado", 120m, 2));

        var item = Assert.Single(result.Items);
        Assert.Equal(productId, item.ProductId);
        Assert.Equal(240m, result.Total);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task AddOrderItem_QuantidadeInvalida_LancaValidationExceptionSemPersistir()
    {
        var (repository, unitOfWork, order) = CreateOrder();
        var handler = new AddOrderItemCommandHandler(
            repository,
            unitOfWork,
            new AddOrderItemCommandValidator());

        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => handler.HandleAsync(
                new AddOrderItemCommand(order.Id, Guid.NewGuid(), "Teclado", 120m, 0)));

        Assert.Contains(exception.Errors, error => error.Code == "quantity.invalid");
        Assert.Empty(order.Items);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task RemoveApproveCancel_PedidoValido_ExecutaTransicoesEPersiste()
    {
        var (repository, unitOfWork, order) = CreateOrder();
        var productId = Guid.NewGuid();
        order.AddItem(productId, "Mouse", 80m, 1);

        var removeHandler = new RemoveOrderItemCommandHandler(
            repository,
            unitOfWork,
            new RemoveOrderItemCommandValidator());
        await removeHandler.HandleAsync(new RemoveOrderItemCommand(order.Id, productId));
        Assert.Empty(order.Items);

        order.AddItem(productId, "Mouse", 80m, 1);
        var approveHandler = new ApproveOrderCommandHandler(
            repository,
            unitOfWork,
            new ApproveOrderCommandValidator());
        await approveHandler.HandleAsync(new ApproveOrderCommand(order.Id));
        Assert.Equal(OrderStatus.Approved, order.Status);

        var cancelHandler = new CancelOrderCommandHandler(
            repository,
            unitOfWork,
            new CancelOrderCommandValidator());
        var result = await cancelHandler.HandleAsync(
            new CancelOrderCommand(order.Id, "Solicitação do cliente."));

        Assert.Equal(nameof(OrderStatus.Cancelled), result.Status);
        Assert.Equal("Solicitação do cliente.", result.ReasonCancelled);
        Assert.Equal(3, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task CancelOrder_MotivoMaiorQue200Caracteres_LancaValidationException()
    {
        var (repository, unitOfWork, order) = CreateOrder();
        var handler = new CancelOrderCommandHandler(
            repository,
            unitOfWork,
            new CancelOrderCommandValidator());

        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => handler.HandleAsync(new CancelOrderCommand(order.Id, new string('x', 201))));

        Assert.Contains(exception.Errors, error => error.Code == "motive.max_length");
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(0, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public async Task GetOrdersByCustomerId_ClienteValido_RetornaPedidosComoDtos()
    {
        var (repository, _, order) = CreateOrder();
        order.AddItem(Guid.NewGuid(), "Monitor", 800m, 1);
        var handler = new GetOrdersByCustomerIdQueryHandler(
            repository,
            new GetOrdersByCustomerIdQueryValidator());

        var result = await handler.HandleAsync(new GetOrdersByCustomerIdQuery(order.CustomerId));

        var dto = Assert.Single(result);
        Assert.Equal(order.Id, dto.Id);
        Assert.Equal(800m, dto.Total);
        Assert.Single(dto.Items);
    }

    [Fact]
    public async Task GetOrderById_PedidoInexistente_LancaOrderNotFoundException()
    {
        var repository = new InMemoryOrderRepository();
        var handler = new GetOrderByIdQueryHandler(
            repository,
            new GetOrderByIdQueryValidator());

        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => handler.HandleAsync(new GetOrderByIdQuery(Guid.NewGuid())));
    }

    private static (InMemoryOrderRepository Repository, InMemoryUnitOfWork UnitOfWork, Order Order)
        CreateOrder()
    {
        var repository = new InMemoryOrderRepository();
        var unitOfWork = new InMemoryUnitOfWork();
        var order = Order.Create(Guid.NewGuid());
        repository.Orders.Add(order);
        return (repository, unitOfWork, order);
    }

    private sealed class InMemoryOrderRepository : IOrderRepository
    {
        public List<Order> Orders { get; } = [];

        public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(Orders.SingleOrDefault(order => order.Id == orderId));
        }

        public Task<IReadOnlyList<Order>> GetByCustomerIdAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            IReadOnlyList<Order> orders = Orders
                .Where(order => order.CustomerId == customerId)
                .ToArray();
            return Task.FromResult(orders);
        }

        public Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Orders.Add(order);
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCount { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SaveChangesCount++;
            return Task.FromResult(1);
        }
    }
}
