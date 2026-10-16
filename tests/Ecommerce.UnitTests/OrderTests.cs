using Ecommerce.Domain.Common;
using Ecommerce.Domain.Orders;

namespace Ecommerce.UnitTests.Domain;

public class OrderTests
{
    [Fact]
    public void Create_ClienteValido_RetornaPedidoComStatusPendente()
    {
        var customerId = Guid.NewGuid();

        var order = Order.Create(customerId);

        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Empty(order.Items);
        Assert.Equal(0m, order.Total);
    }

    [Fact]
    public void Create_ClienteInvalido_LancaArgumentException()
    {
        var action = () => Order.Create(Guid.Empty);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void AddItem_PedidoPendenteEProdutoNovo_AdicionaItemEAtualizaTotal()
    {
        var order = Order.Create(Guid.NewGuid());

        order.AddItem(Guid.NewGuid(), "Teclado mecânico", 299.90m, 2);

        var item = Assert.Single(order.Items);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(599.80m, item.TotalPrice);
        Assert.Equal(599.80m, order.Total);
    }

    [Fact]
    public void AddItem_PedidoPendenteEProdutoExistente_AcumulaQuantidade()
    {
        var order = Order.Create(Guid.NewGuid());
        var productId = Guid.NewGuid();

        order.AddItem(productId, "Mouse gamer", 120.00m, 1);
        order.AddItem(productId, "Mouse gamer", 120.00m, 2);

        var item = Assert.Single(order.Items);
        Assert.Equal(3, item.Quantity);
        Assert.Equal(360.00m, order.Total);
    }

    [Fact]
    public void AddItem_PedidoNaoPendente_LancaInvalidOperationException()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), "Monitor", 900m, 1);
        order.Approve();

        var action = () => order.AddItem(Guid.NewGuid(), "Cabo HDMI", 50m, 1);

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void AddItem_QuantidadeInvalida_LancaDomainException()
    {
        var order = Order.Create(Guid.NewGuid());

        var action = () => order.AddItem(Guid.NewGuid(), "Webcam", 150m, 0);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Approve_PedidoPendenteComItens_AtualizaStatusParaApproved()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), "Notebook", 2500m, 1);

        order.Approve();

        Assert.Equal(OrderStatus.Approved, order.Status);
    }

    [Fact]
    public void Approve_PedidoSemItens_LancaInvalidOperationException()
    {
        var order = Order.Create(Guid.NewGuid());

        var action = () => order.Approve();

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Cancel_PedidoPendenteComMotivoValido_AtualizaStatusEInformacoesDeCancelamento()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), "Headset", 250m, 1);

        order.Cancel("Cliente desistiu da compra.");

        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.Equal("Cliente desistiu da compra.", order.ReasonCancelled);
        Assert.NotNull(order.CancelledAt);
    }

    [Fact]
    public void Cancel_SemMotivo_LancaInvalidOperationException()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), "SSD", 500m, 1);

        var action = () => order.Cancel(string.Empty);

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Cancel_MotivoComMaisDe200Caracteres_LancaInvalidOperationException()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), "Monitor", 700m, 1);
        var motive = new string('x', 201);

        var action = () => order.Cancel(motive);

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void RemoveItem_PedidoPendenteComItemExistente_RemoveItemDaLista()
    {
        var order = Order.Create(Guid.NewGuid());
        var productId = Guid.NewGuid();

        order.AddItem(productId, "Cadeira gamer", 850m, 1);
        order.RemoveItem(productId);

        Assert.Empty(order.Items);
    }

    [Fact]
    public void RemoveItem_ItemNaoExistente_LancaArgumentException()
    {
        var order = Order.Create(Guid.NewGuid());

        var action = () => order.RemoveItem(Guid.NewGuid());

        Assert.Throws<ArgumentException>(action);
    }
}
