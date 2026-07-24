using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RealEstateApp.Infrastructure.Persistence.Contexts;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql("Host=aws-1-us-west-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.rmfvuyafgwutazgbbihz;Password=BXermrRlqfhLILAn");
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
