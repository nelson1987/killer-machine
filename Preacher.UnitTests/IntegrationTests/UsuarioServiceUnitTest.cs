using Domain.Brokers;
using Domain.Contracts;
using Domain.Services;
using FluentAssertions;
using Infrastructure;
using Moq;

namespace Preacher.UnitTests.IntegrationTests;

public class UsuarioServiceIntegrationTests
{
    private readonly UsuarioService _sut;

    public UsuarioServiceIntegrationTests()
    {
        var contexto = new ContextoDbFactory();
        UsuarioRepository repository = new UsuarioRepository(contexto.CreateDbContext([""]));
        Mock<IMessageBroker> messageBroker = new Mock<IMessageBroker>();
        _sut = new UsuarioService(repository, messageBroker.Object);
    }

    [Fact]
    public async Task AdicionarAsync_CommandComDadosValidos_RetornaDadosDeResponse()
    {
        // Arrange
        var command = new CriacaoUsuarioCommand("Teste");
        //Act
        var response = await _sut.AdicionarAsync(command);
        //Assert
        response.Data.Should().NotBeNull();
        response.Data.Id.Should().NotBe(0);
        response.Data.Nome.Should().Be("Teste");
    }

    [Fact]
    public async Task AdicionarAsync_CommandComNomeVazio_RetornaDadosDeResponse()
    {
        // Arrange
        var command = new CriacaoUsuarioCommand(string.Empty);
        //Act
        var response = await _sut.AdicionarAsync(command);
        //Assert
        response.Error.Should().NotBeNull();
        response.Error.Code.Should().NotBeNull("Nome");
        response.Error.Description.Should().NotBeNull("O campo Nome não foi informado.");
    }
}