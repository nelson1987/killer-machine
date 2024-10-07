namespace Preacher.UnitTests;
/*
Primeiro, você escreve um teste que representa um requisito específico da funcionalidade que você está tentando implementar.
Em seguida, você faz o teste passar, escrevendo a quantidade mínima de código de produção com a qual você pode escapar.
Se necessário, você refatoria o código para eliminar duplicações ou outros problemas
*/


//Cliente - Dados podem mudar - Entidade
//Pedido - Dados podem mudar - Entidade
//Item de Pedido - Dados podem mudar - Entidade
//Endereco - Não pode ser mudado - Objeto de Valor
public class Usuario
{
    public Usuario(int id, string nome)
    {
        Id = id;
        Nome = nome;
        DataCriacao = DateTime.Now;
    }
    public int Id { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public string Nome { get; private set; }
    public void Validar()
    {
        if(string.IsNullOrEmpty(this.Nome))
            throw new Exception("Nome é obrigatório");
    }
}

public class UsuarioUnitTest
{
    [Fact]
    public void Usuario_ComDadosValidos_RetornaUsuario()
    {
        var usuario = new Usuario(0,"Teste Nome");
        usuario.Validar();
    }

    [Fact]
    public void Usuario_SemNome_RetornaException()
    {
        var usuario = new Usuario(0, "");
        usuario.Validar();
        //Testar Throws
    }
}

public class UsuarioRepository 
{
    public async Task IncluirAsync(Usuario usuario)
    {
        throw new NotImplementedException();
    }
}

public class UsuarioRepositoryUnitTest
{
    [Fact]
    public void IncluirAsync_ComDadosValidos_RetornaUsuarioPersistido()
    {
        var usuario = new Usuario(0,"Teste Nome");
        new UsuarioRepository().IncluirAsync(usuario);
        //usuarioId = 1
    }
}

public class Processo : IAggregateRoot, Entity
{
    //Teremos 1 Agregador Pessoa nossa entidade do sistema
    //Teremos 1 Objeto de Valor Endereco Entrega na entidade pessoa
    //Teremos 1 Entidade ContaCorrente
    //Teremos 1 DomainEvent PessoaCriada
    public string Nome {get; private set;}
    public Endereco Endereco {get; private set;}
}
public class Endereco : ValueObject
{
    public string Logradouro {get; private set;}
}
public class ContaCorrente : Entity
{
    public string Logradouro {get; private set;}
}
public interface IAggregateRoot
{

}
public class Entity
{

}
public class ValueObject
{

}
public class UnitTest
{
    [Fact]
    public void Test1()
    {
        var processo = new Processo();
        processo.Nome = "";
        Assert.Equal(1, 2);
    }
    [Fact]
    public void Test1()
    {
        var processo = new Processo();
        processo.Nome = "";
        processo.AdicionarEndereco()
    }
}
