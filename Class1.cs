using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace MangaBank.UnitTests
{
    //public class UnitTests
    //{
    //    public UnitTests()
    //    {
    //    }

    //    [Fact]
    //    public async Task TesteInicial_GetPessoas()
    //    {
    //    }

    //    [Fact]
    //    public async Task TesteInicial_PostPessoa()
    //    {
    //        // Arrange
    //        var pessoa = new Pessoa("Jão Silva");
    //        // Act
    //        ctx.Pessoas.AddAsync(pessoa);
    //        ctx.CommitAsync();
    //        // Assert
    //        pessoa.Id.Should().Be(1);
    //    }
    //}

    public class DatabaseIntegrationTest
    {
        protected Contexto ctx;

        public DatabaseIntegrationTest(Contexto ctx = null)
        {
            this.ctx = ctx ?? GetInMemoryDBContext();
        }

        protected Contexto GetInMemoryDBContext()
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<Contexto>()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<Contexto>();
            var options = builder
                    .UseSqlServer("Server=localhost,1433;Database=contexto-teste;User ID=sa;Password=sN#240787;TrustServerCertificate=True")
                    .ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))
                    .UseInternalServiceProvider(serviceProvider)
                    .Options;

            Contexto dbContext = new Contexto(options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();
            return dbContext;
        }
    }

    public class PostUserValidator : AbstractValidator<CriacaoPessoaCommand>
    {
        public PostUserValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .MaximumLength(20);
        }
    }

    public class PostUserValidatorUnitTests
    {
        private readonly PostUserValidator _validator;

        public PostUserValidatorUnitTests()
        {
            _validator = new PostUserValidator();
        }

        [Fact]
        public async Task Should_have_error_when_Name_is_null()
        {
            var model = new CriacaoPessoaCommand(null);
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(person => person.Nome);
        }

        [Fact]
        public void Should_not_have_error_when_name_is_specified()
        {
            var model = new CriacaoPessoaCommand("Jeremy");
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(person => person.Nome);
        }
    }

    public record CriacaoPessoaCommand(string Nome);

    public class CriacaoPessoaCommandUnitTests
    {
        [Fact]
        public async Task TesteInicial_PostPessoa_NoRepository()
        {
            // Act
            var response = new CriacaoPessoaCommand("João Silva");
            // Assert
            response.Nome.Should().Be("João Silva");
        }
    }

    public record CriacaoPessoaResponse(int Id, string Nome);

    public class CriacaoPessoaResponseUnitTests
    {
        [Fact]
        public async Task TesteInicial_PostPessoa_NoRepository()
        {
            // Act
            var response = new CriacaoPessoaResponse(0, "João Silva");
            // Assert
            response.Id.Should().Be(0);
            response.Nome.Should().Be("João Silva");
        }
    }

    public class CriacaoPessoaHandler
    {
        private readonly PessoaRepository _pessoaRepository;

        public CriacaoPessoaHandler(PessoaRepository pessoaRepository)
        {
            _pessoaRepository = pessoaRepository;
        }

        public async Task<CriacaoPessoaResponse> Handle(CriacaoPessoaCommand command)
        {
            var pessoa = new Pessoa(0, command.Nome);
            await _pessoaRepository.Post(pessoa);
            return new CriacaoPessoaResponse(pessoa.Id, pessoa.Nome);
        }
    }

    public class CriacaoPessoaHandlerUnitTests : DatabaseIntegrationTest
    {
        private readonly Fixture fixture;
        private readonly Mock<Contexto> _context;
        private readonly PessoaRepository _pessoaRepository;
        private readonly CriacaoPessoaHandler _CriacaoPessoaHandler;

        public CriacaoPessoaHandlerUnitTests()
        {
            fixture = new Fixture();
            _context = new Mock<Contexto>();
            _pessoaRepository = new PessoaRepository(_context.Object);
            _CriacaoPessoaHandler = new CriacaoPessoaHandler(_pessoaRepository);
        }

        [Fact]
        public async Task TesteInicial_PostPessoa_NoRepository()
        {
            // Arrange
            //var pessoa = fixture.Build<Pessoa>().Do(x=> );
            //_context.Setup(x => x.AddAsync(pessoa, It.IsAny<CancellationToken>()));
            // Act
            var command = new CriacaoPessoaCommand("João Silva");
            var response = await _CriacaoPessoaHandler.Handle(command);
            // Assert
            response.Id.Should().Be(1);
            _context.Verify(m => m.AddAsync(It.IsAny<Pessoa>(), It.IsAny<CancellationToken>()), Times.Once);
            _context.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    public class PessoaRepository
    {
        private readonly Contexto _contexto;

        public PessoaRepository(Contexto contexto)
        {
            _contexto = contexto;
        }

        public async Task Post(Pessoa pessoa)
        {
            await _contexto.Set<Pessoa>().AddAsync(pessoa);
            await _contexto.CommitAsync();
        }
    }

    public class PessoaRepositoryUnitTests
    {
        private readonly Fixture fixture;
        private readonly Mock<Contexto> _context;
        private readonly PessoaRepository _pessoaRepository;

        public PessoaRepositoryUnitTests()
        {
            fixture = new Fixture();
            _context = new Mock<Contexto>();
            _pessoaRepository = new PessoaRepository(_context.Object);
        }

        [Fact]
        public async Task TesteInicial_PostPessoa_NoRepository()
        {
            // Arrange
            var pessoa = new Pessoa(0, "João Silva");
            // Act
            await _pessoaRepository.Post(pessoa);
            // Assert
            pessoa.Id.Should().Be(1);
        }

        [Fact]
        public async Task TesteInicial_PostPessoaWithId_NoRepository()
        {
            // Arrange
            var pessoa = new Pessoa(1, "João Silva");
            // Act
            await _pessoaRepository.Post(pessoa);
            // Assert
            pessoa.Id.Should().Be(1);
        }
    }

    public class Contexto : DbContext
    {
        //private DbSet<Pessoa> Pessoas;

        public Contexto(DbContextOptions<Contexto> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pessoa>(e =>
            {
                e
                .ToTable("TB_PESSOA")
                .HasKey(k => k.Id);

                e
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            });
        }

        public async Task CommitAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class PessoaContextUnitTests : DatabaseIntegrationTest
    {
        public PessoaContextUnitTests()
        {
        }

        [Fact]
        public async Task TesteInicial_PostPessoa_NoRepository()
        {
            // Arrange
            var pessoa = new Pessoa(0, "João Silva");
            // Act
            await ctx.Set<Pessoa>().AddAsync(pessoa);
            await ctx.CommitAsync();
            // Assert
            pessoa.Id.Should().Be(1);
        }

        [Fact]
        public async Task TesteInicial_PostPessoaWithId_NoRepository()
        {
            // Arrange
            var pessoa = new Pessoa(1, "João Silva");
            // Act
            await ctx.Set<Pessoa>().AddAsync(pessoa);
            await ctx.CommitAsync();
            // Assert
            pessoa.Id.Should().Be(1);
        }
    }

    public class Pessoa : Entity
    {
        public Pessoa(int id, string nome) : base(id)
        {
            Nome = nome;
        }

        public string Nome { get; private set; }
    }

    public class PessoaUnitTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [InlineData("LUCIANO PEREIRA")]
        public void Theory_PostUser_Name(string nome)
        {
            var user = new Pessoa(0, nome);
            Assert.Null(user.Nome);
            Assert.Empty(user.Nome);
            Assert.True(user.Nome.Length > 20);
        }
    }

    public class Entity
    {
        public Entity(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
    }
}