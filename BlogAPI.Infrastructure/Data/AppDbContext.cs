using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;

namespace BlogAPI.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<IdentityRole<Guid>>().HasData(
    new IdentityRole<Guid>
    {
        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        Name = "Admin",
        NormalizedName = "ADMIN",
        ConcurrencyStamp = "1"
    },
    new IdentityRole<Guid>
    {
        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
        Name = "Author",
        NormalizedName = "AUTHOR",
        ConcurrencyStamp = "2"
    },
    new IdentityRole<Guid>
    {
        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
        Name = "Reader",
        NormalizedName = "READER",
        ConcurrencyStamp = "3"
    }
);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}