using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace MangaBank.UnitTests
{
    public class ProcessoUnitTests : AggregateRootIntegrationTest
    {
        public ProcessoUnitTests()
        {
        }

        [Fact]
        public void UnitTask()
        {
            var processo = new Processo("Processo_1");
            //processo.AdicionarEndereco();
            processo.Validar();

            processo.Id.Should().Be(0);
        }

        [Fact]
        public void UnitTask1()
        {
            var processo = new Processo("Processo_1");
            //processo.AdicionarEndereco();
            processo.Validar();

            ctx.Set<Processo>().Add(processo);
            ctx.SaveChanges();

            processo.Id.Should().Be(1);
        }

        [Fact]
        public async Task IncluirProcesso_Repository()
        {
            var processo = new Processo("Processo_1");
            var endereco = new Endereco("Avenida", "Brasil");
            processo.AdicionarEndereco(endereco);

            new ProcessoRepository(ctx).Incluir(processo);
            await ctx.CommitAsync();

            processo.Id.Should().Be(1);
            processo.FarmAreaCoordinates.Should().NotBeNull();
            processo.FarmAreaCoordinates.Id.Should().Be(1);
        }

        [Fact]
        public async Task AlterarEnderecoAposAdicionarProcesso_Repository()
        {
            var processo = new Processo("Processo_1");
            //var endereco = new Endereco("Avenida", "Brasil");
            //processo.AdicionarEndereco(endereco);

            new ProcessoRepository(ctx).Incluir(processo);
            await ctx.CommitAsync();

            processo.Id.Should().Be(1);
            processo.FarmAreaCoordinates.Should().NotBeNull();
            processo.FarmAreaCoordinates.Id.Should().Be(1);
        }
    }

    public interface IProcessoRepository
    {
        void Incluir(Processo processo);
    }

    public class ProcessoRepository : Repository<Processo>, IProcessoRepository
    {
        public ProcessoRepository(AggregateRootContext context)
            : base(context)
        {
        }

        public void Incluir(Processo processo)
        {
            DbSet.Add(processo);
        }
    }

    public class AggregateRootIntegrationTest
    {
        protected AggregateRootContext ctx;

        public AggregateRootIntegrationTest(AggregateRootContext ctx = null)
        {
            this.ctx = ctx ?? GetInMemoryDBContext();
        }

        protected AggregateRootContext GetInMemoryDBContext()
        {
            var serviceProvider = new ServiceCollection()
                //.AddDbContext<AggregateRootContext>()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<AggregateRootContext>();
            var options = builder
                    .UseInMemoryDatabase("test")
                    //.UseSqlServer()
                    .ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))
                    .UseInternalServiceProvider(serviceProvider)
                    .Options;

            AggregateRootContext dbContext = new AggregateRootContext(options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            //dbContext.Database.Migrate();
            return dbContext;
        }
    }

    public interface IAggregateRoot
    {
    }

    public abstract class Entities
    {
        public int Id { get; protected set; }

        public abstract void Validar();
    }

    public class Processo : Entities, IAggregateRoot
    {
        public Processo(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public Endereco FarmAreaCoordinates { get; set; }

        public override void Validar()
        {
            throw new NotImplementedException();
        }

        internal void AdicionarEndereco(Endereco endereco)
        {
            FarmAreaCoordinates = endereco;
        }
    }

    public class Endereco : Entities
    {
        public Endereco(string latitude, string longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public string Latitude { get; private set; }
        public string Longitude { get; private set; }
        public Processo FarmArea { get; private set; }
        //public int FarmAreaId { get; protected set; }

        public override void Validar()
        {
            throw new NotImplementedException();
        }
    }

    public interface IRepository<T> : IDisposable where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }

    public abstract class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, IAggregateRoot
    {
        protected readonly AggregateRootContext Db;
        protected readonly DbSet<TEntity> DbSet;
        protected readonly IQueryable<TEntity> DbSetReadOnly;

        public Repository(AggregateRootContext context)
        {
            Db = context;
            DbSet = context.Set<TEntity>();
            DbSetReadOnly = context.Set<TEntity>().AsNoTracking();
        }

        public IUnitOfWork UnitOfWork => Db;

        public void Dispose()
        {
            Db.Dispose();
        }
    }

    public interface IUnitOfWork
    {
        Task<bool> CommitAsync();
    }

    public class AggregateRootContext : DbContext, IUnitOfWork
    {
        public AggregateRootContext(DbContextOptions<AggregateRootContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AggregateRootContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public async Task<bool> CommitAsync()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }

    public class FarmAreaMappings : IEntityTypeConfiguration<Processo>
    {
        public void Configure(EntityTypeBuilder<Processo> builder)
        {
            builder.ToTable("TB_PROCESSO")
                    .HasKey(c => c.Id);

            builder.Property(p => p.Id)
                    .ValueGeneratedOnAdd();

            builder.Property(c => c.Name)
                .HasColumnType("nvarchar(255)")
                .IsRequired();

            //builder.HasMany(c => c.FarmAreaCoordinates)
            //    .WithOne(c => c.FarmArea)
            //    .HasForeignKey(c => c.FarmAreaId);

            builder.HasOne(c => c.FarmAreaCoordinates)
                //.WithOne(c => c.FarmArea)
                .HasForeignKey();
            //.HasForeignKey(c => c.FarmAreaId);
        }
    }

    public class FarmAreaCordinateMappings : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.ToTable("TB_ENDERECO")
                    .HasKey(c => c.Id);

            builder.Property(p => p.Id)
                    .ValueGeneratedOnAdd();

            builder.Property(c => c.Latitude)
                .HasColumnType("nvarchar(255)")
                .IsRequired();

            builder.Property(c => c.Longitude)
                .HasColumnType("nvarchar(255)")
                .IsRequired();

            //builder.HasOne(c => c.FarmArea)
            //    .WithMany(c => c.FarmAreaCoordinates)
            //    .HasForeignKey(c => c.FarmAreaId);
        }
    }
}