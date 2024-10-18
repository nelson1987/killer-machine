using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Preacher.UnitTests.IntegrationTests;

public class ContextoDbFactory : IDesignTimeDbContextFactory<Contexto>
{
    public Contexto CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<Contexto>()
               .UseSqlServer("Server=localhost,1433;Database=contexto-teste;User ID=sa;Password=sN#240787;TrustServerCertificate=True")
               .ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));

        return new Contexto(optionsBuilder.Options);
    }
}