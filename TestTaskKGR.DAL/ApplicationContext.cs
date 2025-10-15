using Microsoft.EntityFrameworkCore;

namespace TestTaskKGR.DAL;

public class ApplicationContext: DbContext
{
    public DbSet<Type> Types {get; set;}
    public DbSet<Role> Roles {get; set;}
    public DbSet<Violation> Violations {get; set;}

    public ApplicationContext(){}
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options){}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=testtaskkgr.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Type>(entity =>
        {
            entity.HasIndex(e => e.Name)
                .IsUnique();
        });
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.Name)
                .IsUnique();
        });
    }
}