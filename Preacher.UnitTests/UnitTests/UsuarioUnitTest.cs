using Domain.Contracts;
using Domain.Entities;
using FluentAssertions;

namespace Preacher.UnitTests.UnitTests;

public class UsuarioUnitTest
{
    [Fact]
    public void InstanciarUsuario_ComDadosValidos_RetornaAtivoTrue()
    {
        // Arrange
        var command = new CriacaoUsuarioCommand("Teste");
        // Act
        var usuario = new Usuario(command);
        //Assert
        usuario.Id.Should().Be(0);
        usuario.Ativo.Should().BeTrue();
        usuario.Nome.Should().Be("Teste");
    }

    [Fact]
    public void InstanciarUsuario_ComNomeInvalido_DisparaExcecao()
    {
        // Arrange
        var command = new CriacaoUsuarioCommand(string.Empty);
        // Act
        var usuario = () => new Usuario(command);
        //Assert
        usuario.Should().Throw<ArgumentException>();
    }
}