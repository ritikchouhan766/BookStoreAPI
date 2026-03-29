using BookStore.Application.Books.Queries.GetAllBooks;
using BookStore.Application.Books.Queries.GetBookById;
using FluentAssertions;
using Xunit;

namespace BookStore.Tests.Books;

public class BookQueryTests
{
    // ── GetAllBooks ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllBooksQuery_ReturnsAllBooks_WhenNoneFiltered()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateWithSeededDb();
        var handler = new GetAllBooksQueryHandler(uow);

        // Act
        var result = await handler.Handle(new GetAllBooksQuery(null, 1, 10), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetAllBooksQuery_FiltersByGenre_ReturnsMatchingBooks()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateWithSeededDb();
        var handler = new GetAllBooksQueryHandler(uow);

        // Act — genre matches seeded book
        var result = await handler.Handle(new GetAllBooksQuery("Programming", 1, 10), CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().Genre.Should().Be("Programming");
    }

    [Fact]
    public async Task GetAllBooksQuery_FiltersByGenre_ReturnsEmpty_WhenNoMatch()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateWithSeededDb();
        var handler = new GetAllBooksQueryHandler(uow);

        // Act
        var result = await handler.Handle(new GetAllBooksQuery("Fantasy", 1, 10), CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetAllBooksQuery_Pagination_ReturnsCorrectPage()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateWithSeededDb();
        var handler = new GetAllBooksQueryHandler(uow);

        // Act — page 2 with pageSize 10 should be empty (only 1 book total)
        var result = await handler.Handle(new GetAllBooksQuery(null, 2, 10), CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeFalse();
    }

    // ── GetBookById ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetBookByIdQuery_ReturnsBook_WhenExists()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateWithSeededDb();
        var handler = new GetBookByIdQueryHandler(uow);

        // Act
        var result = await handler.Handle(new GetBookByIdQuery(1), CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be("Clean Code");
        result.Data.ISBN.Should().Be("978-0132350884");
        result.Data.AuthorName.Should().Be("Robert Martin");
    }

    [Fact]
    public async Task GetBookByIdQuery_ReturnsFail_WhenNotFound()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateWithSeededDb();
        var handler = new GetBookByIdQueryHandler(uow);

        // Act
        var result = await handler.Handle(new GetBookByIdQuery(999), CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors[0].Should().Contain("999");
    }
}
