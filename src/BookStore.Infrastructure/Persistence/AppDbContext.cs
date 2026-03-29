using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // ── Seed data ────────────────────────────────────────────────────────
        modelBuilder.Entity<AppUser>().HasData(new AppUser
        {
            Id = 1,
            Username = "admin",
            Email = "admin@bookstore.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "Admin",
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, FirstName = "Robert", LastName = "Martin", Bio = "Author of Clean Code", Email = "robert@example.com", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Author { Id = 2, FirstName = "Martin", LastName = "Fowler", Bio = "Author of Refactoring", Email = "martin@example.com", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<Publisher>().HasData(
            new Publisher { Id = 1, Name = "Prentice Hall", Website = "https://www.pearson.com", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Publisher { Id = 2, Name = "Addison-Wesley", Website = "https://www.pearson.com", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book
            {
                Id = 1, Title = "Clean Code", ISBN = "978-0132350884",
                Price = 45.99m, StockQuantity = 50,
                PublishedDate = new DateTime(2008, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                Description = "A handbook of agile software craftsmanship",
                Genre = "Programming", AuthorId = 1, PublisherId = 1,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Book
            {
                Id = 2, Title = "Refactoring", ISBN = "978-0201485677",
                Price = 52.99m, StockQuantity = 35,
                PublishedDate = new DateTime(1999, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                Description = "Improving the design of existing code",
                Genre = "Programming", AuthorId = 2, PublisherId = 2,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
