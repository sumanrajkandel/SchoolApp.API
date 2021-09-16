
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // IdentityDbContext present
using Microsoft.EntityFrameworkCore; // added nuget package : present DbContext
using Microsoft.EntityFrameworkCore.Design;
using SchoolApp.API.Models;

namespace SchoolApp.API.Data;
public class AppDbContext : IdentityDbContext<Applicationuser> // to create default identity framework's related table and propeties etc
//:DbContext //DbContext class also knowns as translater file means it reads both sql and c# and translate to each other.
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) //understand base(options)
    {

    }

    public DbSet<RefreshToken> RefreshTokens{ get; set; }
    public DbSet<APPLogInfo> APPLogInfo { get; set; }

}


public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=school-api-db;Integrated Security=True;Pooling=False");

        return new AppDbContext(optionsBuilder.Options);
    }
}
