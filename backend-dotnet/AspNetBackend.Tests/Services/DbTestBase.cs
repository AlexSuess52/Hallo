using AspNetBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

// base test class that provides an in-memory database context for tests.
// implements IDisposable to clean up after tests.
public abstract class DbTestBase : IDisposable
{
    protected readonly AspNetPostgresDbContext _context;

    public DbTestBase()
    {   
        // configure DbContext to use a unique in-memory database for each test class instance.
        var options = new DbContextOptionsBuilder<AspNetPostgresDbContext>()
                    // use InMemory database with a unique name (Guid) to isolate tests
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    // ignore warnings about transactions being ignored since InMemory provider does not support transactions
                    .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;

        _context = new AspNetPostgresDbContext(options);
    }

    // dispose method to clean up the in-memory database after each test
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
