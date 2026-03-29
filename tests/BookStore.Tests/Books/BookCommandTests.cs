using BookStore.Application.Books.Commands.CreateBook;
using BookStore.Application.Books.Commands.DeleteBook;
using BookStore.Application.Books.Commands.UpdateBook;
using FluentAssertions;
using Xunit;

namespace BookStore.Tests.Books;

public class BookCommandTests
{
    // ── CreateBook ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateBookCommand_ReturnsSuccess_WhenValidData()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateWithSeededDb();
        var handler = new CreateBookCommandHandler(uow);
        var command = new CreateBookCommand(
            Title: "The Pragmatic Programmer",
            ISBN: "978-0201616224",
            Price: 49.99m,
            StockQuantity: 30,
            PublishedDate: new DateTime(1999, 10, 1, 0, 0, 0, DateTimeKind.Utc),
            Description: "Your journey to mastery",
            Genre: "Programming",
            AuthorId: 1,
            PublisherId: 1
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be("The Pragmatic Programmer");
        result.Data.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateBookCommand_ReturnsFail_WhenIsbnAlreadyExists()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateWithSeededDb();
        var handler = new CreateBookCommandHandler(uow);

        // ISBN "978-0132350884" already seeded
        var command = new CreateBookCommand(
            Title: "Duplicate ISBN Book",
            ISBN: "978-0132350884",
            Price: 29.99m,
            StockQuantity: 10,
            PublishedDate: DateTime.UtcNow.AddYears(-1),
            Description: null,
            Genre: "Programming",
            AuthorId: 1,
            PublisherId: 1
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("ISBN"));
    }

    [Fact]
    public async Task CreateBookCommand_ReturnsFail_WhenAuthorNotFound()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateWithSeededDb();
        var handler = new CreateBookCommandHandler(uow);
        var command = new CreateBookCommand(
            Title: "Valid Book",
            ISBN: "978-1111111111",
            Price: 20m,
            StockQuantity: 5,
            PublishedDate: DateTime.UtcNow.AddYears(-1),
            Description: null,
            Genre: "Tech",
            AuthorId: 9999,   // Does not exist
            PublisherId: 1
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("Author"));
    }

    // ── UpdateBook ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateBookCommand_ReturnsSuccess_WhenBookExists()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateWithSeededDb();
        var handler = new UpdateBookCommandHandler(uow);
        var command = new UpdateBookCommand(
            Id: 1,
            Title: "Clean Code — Updated Edition",
            ISBN: "978-0132350884",
            Price: 55.00m,
            StockQuantity: 100,
            PublishedDate: new DateTime(2008, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            Description: "Updated description",
            Genre: "Programming",
            AuthorId: 1,
            PublisherId: 1
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Title.Should().Be("Clean Code — Updated Edition");
        result.Data.Price.Should().Be(55.00m);
    }

    [Fact]
    public async Task UpdateBookCommand_ReturnsFail_WhenBookNotFound()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateWithSeededDb();
        var handler = new UpdateBookCommandHandler(uow);
        var command = new UpdateBookCommand(
            Id: 9999, Title: "Ghost", ISBN: "000-0000000000",
            Price: 10m, StockQuantity: 1,
            PublishedDate: DateTime.UtcNow, Description: null,
            Genre: null, AuthorId: 1, PublisherId: 1
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("9999"));
    }

    // ── DeleteBook ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteBookCommand_ReturnsSuccess_WhenBookExists()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateWithSeededDb();
        var handler = new DeleteBookCommandHandler(uow);

        // Act
        var result = await handler.Handle(new DeleteBookCommand(1), CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteBookCommand_ReturnsFail_WhenBookNotFound()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateWithSeededDb();
        var handler = new DeleteBookCommandHandler(uow);

        // Act
        var result = await handler.Handle(new DeleteBookCommand(9999), CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }
}
