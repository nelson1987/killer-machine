using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MangaBank.UnitTests
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Projeto>(e =>
            {
                e
                .ToTable("TB_PROJETO")
                .HasKey(k => k.Id);

                e
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

                e
                .Property(p => p.Id)
                .HasColumnName("IDT_PROJETO");

                e
                .Property(p => p.Nome)
                .HasColumnName("NOM_PROJETO")
                .IsRequired();
            });
        }

        public async Task CommitAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class DatabaseIntegrationTest
    {
        public Contexto GetInMemoryDBContext()
        {
            var options = new DbContextOptionsBuilder<Contexto>()
                    .UseSqlServer("Server=localhost,1433;Database=contexto-teste;User ID=sa;Password=sN#240787;TrustServerCertificate=True")
                    .ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))
                    .Options;
            return new Contexto(options);
        }
    }

    public class Projeto
    {
        public Projeto(int id, string nome)
        {
            if (id < 0) throw new ArgumentNullException("id");
            if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentNullException("nome");
            Id = id;
            Nome = nome;
        }

        //CRUD - Projeto
        public int Id { get; set; }

        public string Nome { get; set; }

        internal void AlterarNome(string nome)
        {
            Nome = nome;
        }
    }

    public class ProjetoUnitTests
    {
        [Fact]
        public async Task CriarProjeto()
        {
            var projeto = new Projeto(0, "PBI 1");
            projeto.Id.Should().Be(0);
            projeto.Nome.Should().Be("PBI 1");
        }

        [Fact]
        public async Task CriarProjeto_ComIdMenorQueZero_RetornaExcecao()
        {
            var projeto = new Projeto(-10, "PBI 1");
        }

        [Fact]
        public async Task CriarProjeto_ComNomeVazio_RetornaExcecao()
        {
            var projeto = new Projeto(0, " ");
        }

        [Fact]
        public async Task CriarProjeto_ComNomeNulo_RetornaExcecao()
        {
            var projeto = new Projeto(0, null);
        }

        [Fact]
        public async Task AlterarNomeProjeto()
        {
            var projeto = new Projeto(0, "PBI 1");
            projeto.AlterarNome("PBI 2");
            projeto.Nome.Should().Be("PBI 2");
        }
    }

    [Collection("Integration Tests")]
    public class ProjetoDbSetUnitTests : TestBase
    {
        public ProjetoDbSetUnitTests(DatabaseIntegrationTest server) : base(server)
        {
        }

        [Fact]
        public async Task AddAsync_DadoInexistente_RetornaDadoPersistido()
        {
            var projeto = new Projeto(0, "PBI 1");

            await _ctx.AddAsync(projeto);
            await _ctx.SaveChangesAsync();

            projeto.Id.Should().Be(1);
            projeto.Nome.Should().Be("PBI 1");
        }

        [Fact]
        public async Task AddAsync_SaveChangeExecutado2Vezes_RetornaExcecao()
        {
            var projeto = new Projeto(0, "PBI 1");

            await _ctx.AddAsync(projeto);
            await _ctx.SaveChangesAsync();

            await _ctx.AddAsync(projeto);
            var saveChange = async () => await _ctx.SaveChangesAsync();

            await saveChange.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public async Task Update_ProjetoExistente_RetornaDadosAlterados()
        {
            var projeto = new Projeto(0, "PBI 1");

            await _ctx.AddAsync(projeto);
            await _ctx.SaveChangesAsync();

            projeto.AlterarNome("PBI 2");

            _ctx.Update(projeto);
            await _ctx.SaveChangesAsync();

            projeto.Id.Should().Be(1);
            projeto.Nome.Should().Be("PBI 2");
        }

        [Fact]
        public async Task AddAsync_ProjetoInexistente_RetornaExcecao()
        {
            var projeto = new Projeto(1, "PBI 1");
            await _ctx.AddAsync(projeto);

            var saveChange = async () => await _ctx.SaveChangesAsync();

            await saveChange.Should().ThrowAsync<DbUpdateException>();
        }
    }

    public class ProjetoRepository
    {
        private readonly Contexto _contexto;

        public ProjetoRepository(Contexto ctx)
        {
            _contexto = ctx;
        }

        public async Task AdicionarAsync(Projeto projeto)
        {
            await _contexto.AddAsync(projeto);
            await _contexto.SaveChangesAsync();
        }

        public async Task AlterarAsync(Projeto projeto)
        {
            _contexto.Update(projeto);
            await _contexto.SaveChangesAsync();
        }
    }

    [CollectionDefinition("Integration Tests")]
    public class IntegrationTestCollection : ICollectionFixture<DatabaseIntegrationTest>
    {
    }

    public abstract class TestBase : IAsyncLifetime
    {
        private readonly DatabaseIntegrationTest _server;
        protected Contexto _ctx;

        public TestBase(DatabaseIntegrationTest server)
        {
            _server = server;
            _ctx = server.GetInMemoryDBContext();
        }

        public async Task InitializeAsync()
        {
            await _ctx.Database.EnsureCreatedAsync();
            await _ctx.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await _ctx.Database.EnsureDeletedAsync();
        }
    }

    [Collection("Integration Tests")]
    public class ProjetoRepositoryUnitTests : TestBase
    {
        private readonly ProjetoRepository _sut;

        public ProjetoRepositoryUnitTests(DatabaseIntegrationTest server) : base(server)
        {
            _sut = new ProjetoRepository(_ctx);
        }

        [Fact]
        public async Task AdicionarAsync_DadoInexistente_RetornaDadoPersistido()
        {
            // Arrange
            var pessoa = new Projeto(0, "João Silva");
            // Act
            await _sut.AdicionarAsync(pessoa);
            // Assert
            pessoa.Id.Should().Be(1);
            pessoa.Nome.Should().Be("João Silva");
        }

        [Fact]
        public async Task AdicionarAsync_Executado2Vezes_RetornaExcecao()
        {
            // Arrange
            var pessoa = new Projeto(0, "João Silva");
            await _sut.AdicionarAsync(pessoa);
            // Act
            pessoa.AlterarNome("Paulo Moura");
            await _sut.AlterarAsync(pessoa);
            // Assert
            pessoa.Id.Should().Be(1);
            pessoa.Nome.Should().Be("Paulo Moura");
        }
    }

    public class CriarProjetoHandler
    {
        private readonly ProjetoRepository _projetoRepository;

        public CriarProjetoHandler(ProjetoRepository projetoRepository)
        {
            _projetoRepository = projetoRepository;
        }

        public async Task<Projeto> Handle(CriarProjetoCommand command)
        {
            var projeto = command.ToEntity();
            await _projetoRepository.AdicionarAsync(projeto);
            return projeto;
        }
    }

    [Collection("Integration Tests")]
    public class CriarProjetoHandlerUnitTests : TestBase
    {
        private readonly CriarProjetoHandler _sut;

        public CriarProjetoHandlerUnitTests(DatabaseIntegrationTest server) : base(server)
        {
            _sut = new CriarProjetoHandler(new ProjetoRepository(_ctx));
        }

        [Fact]
        public async Task Handle_QuandoCommandoCorreto_RetornaValorPersistido()
        {
            // Arrange
            CriarProjetoCommand nome = new CriarProjetoCommand("João Silva");
            // Act
            var result = await _sut.Handle(nome);
            // Assert
            result.Id.Should().Be(1);
            result.Nome.Should().Be("João Silva");
        }
    }

    public record CriarProjetoCommand(string Nome)
    {
        public Projeto ToEntity()
        {
            return new Projeto(0, Nome);
        }
    }

    public class CriarProjetoCommandUnitTests
    {
        private readonly CriarProjetoCommand _sut;

        public CriarProjetoCommandUnitTests()
        {
            _sut = new CriarProjetoCommand("Nome");
        }

        [Fact]
        public async Task Validate_QuandoNomeVazio_RetornaProblema()
        {
            var comando = _sut with { Nome = "" };

            bool resultado = true;
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task Validate_QuandoNomeNulo_RetornaProblema()
        {
            var comando = _sut with { Nome = null };

            bool resultado = true;
            resultado.Should().BeFalse();
        }
    }
}