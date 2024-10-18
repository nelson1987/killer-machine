using Domain.Contracts;
using Domain.Entities;
using FluentAssertions;
using Infrastructure;

namespace Preacher.UnitTests.IntegrationTests;

public class UsuarioRepositoryUnitTest
{
    private readonly UsuarioRepository _sut;

    public UsuarioRepositoryUnitTest()
    {
        var fabrica = new ContextoDbFactory();
        _sut = new UsuarioRepository(fabrica.CreateDbContext([""]));
    }

    [Fact]
    public async Task AdicionarAsync_ComDadosValidos_RetornaDadosDoUsuario()
    {
        // Arrange
        var command = new CriacaoUsuarioCommand("Teste");
        var usuario = new Usuario(command);
        //Act
        await _sut.AdicionarAsync(usuario);
        //Assert
        usuario.Id.Should().NotBe(0);
        usuario.Ativo.Should().BeTrue();
        usuario.Nome.Should().Be("Teste");
    }

    [Fact]
    public async Task BuscarPorId_AposAdicionarUsuario_RetornaDadosIguaisDosUsuarios()
    {
        // Arrange
        var command = new CriacaoUsuarioCommand("Teste");
        var usuario = new Usuario(command);
        //Act
        await _sut.AdicionarAsync(usuario);
        var UsuarioPersistido = await _sut.BuscarPorId(usuario.Id);
        //Assert
        usuario.Should().BeEquivalentTo(UsuarioPersistido);
    }
}