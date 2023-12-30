using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nvisia.Profile.Service.WriteStore.Context;

namespace Nvisia.Profile.Service.UnitTests.Utils;

public static class TestUtils
{
    public static ApplicationDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        
        return new ApplicationDbContext(options);
    }
}