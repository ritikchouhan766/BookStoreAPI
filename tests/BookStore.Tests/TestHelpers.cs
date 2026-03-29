using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Persistence;
using BookStore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BookStore.Tests;

/// <summary>
/// Creates a fresh in-memory AppDbContext for each test — no SQL Server needed.
/// </summary>
public static class TestDbFactory
{
    public static AppDbContext CreateInMemoryDb(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;

        var db = new AppDbContext(options);
        db.Database.EnsureCreated();
        return db;
    }

    /// <summary>Seeds a known set of Authors, Publishers, and Books for query tests.</summary>
    public static AppDbContext CreateSeededDb()
    {
        var db = CreateInMemoryDb();

        var author = new Author
        {
            Id = 1, FirstName = "Robert", LastName = "Martin",
            Email = "robert@test.com",
            CreatedAt = DateTime.UtcNow
        };
        var publisher = new Publisher
        {
            Id = 1, Name = "Prentice Hall",
            CreatedAt = DateTime.UtcNow
        };
        var book = new Book
        {
            Id = 1, Title = "Clean Code", ISBN = "978-0132350884",
            Price = 45.99m, StockQuantity = 50, Genre = "Programming",
            PublishedDate = new DateTime(2008, 8, 1, DateTimeKind.Utc),
            AuthorId = 1, PublisherId = 1,
            CreatedAt = DateTime.UtcNow
        };

        db.Authors.Add(author);
        db.Publishers.Add(publisher);
        db.Books.Add(book);
        db.SaveChanges();

        return db;
    }
}

/// <summary>
/// Builds a real UnitOfWork backed by an in-memory DB.
/// </summary>
public static class UnitOfWorkFactory
{
    public static IUnitOfWork CreateWithSeededDb()
    {
        var db = TestDbFactory.CreateSeededDb();
        return new UnitOfWork(db);
    }

    public static IUnitOfWork CreateEmpty()
    {
        var db = TestDbFactory.CreateInMemoryDb();
        return new UnitOfWork(db);
    }
}
