using Ecommerce.Domain.Common;

namespace Ecommerce.UnitTests.Domain;

public class MoneyTests
{
    [Theory]
    [InlineData("BRL", "BRL")]
    [InlineData("usd", "USD")]
    [InlineData(" EUR ", "EUR")]
    public void Constructor_CodigoDeMoedaValido_NormalizaCodigo(string currency, string expectedCurrency)
    {
        var money = new Money(25.50m, currency);

        Assert.Equal(25.50m, money.Amount);
        Assert.Equal(expectedCurrency, money.Currency);
    }

    [Fact]
    public void Constructor_ValorNegativo_LancaDomainException()
    {
        var action = () => new Money(-0.01m, "BRL");

        Assert.Throws<DomainException>(action);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("BR")]
    [InlineData("BRL1")]
    [InlineData("R$")]
    public void Constructor_CodigoDeMoedaInvalido_LancaDomainException(string? currency)
    {
        var action = () => new Money(10m, currency!);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Constructor_ValorZero_AceitaValor()
    {
        var money = new Money(0m, "BRL");

        Assert.Equal(0m, money.Amount);
        Assert.Equal("BRL", money.Currency);
    }

    [Fact]
    public void Money_ValoresIguais_SaoIguais()
    {
        var first = new Money(10m, "BRL");
        var second = new Money(10m, "brl");

        Assert.Equal(first, second);
    }
}
