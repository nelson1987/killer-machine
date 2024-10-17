using FluentAssertions;

namespace Preache.UnitTests;
public class EntidadeBase
{
    public EntidadeBase()
    {
        Ativo = true;
    }
    public int Id {get; set;}
    public bool Ativo {get; set;}
}
public class Usuario : EntidadeBase 
{ 
    public string Nome { get; private set; }
}
public class UsuarioUnitTest
{
    [Fact]
    public void InstanciarUsuario_ComDadosValidos_RetornaAtivoTrue()
    {
        var usuario = new Usuario();
        usuario.Ativo.Should().BeTrue();
    }
}