using BookStore.Application.Authors.Commands.CreateAuthor;
using BookStore.Application.Authors.Queries.GetAllAuthors;
using FluentAssertions;
using Xunit;

namespace BookStore.Tests.Authors;

public class AuthorTests
{
    [Fact]
    public async Task GetAllAuthorsQuery_ReturnsAllAuthors()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateWithSeededDb();
        var handler = new GetAllAuthorsQueryHandler(uow);

        // Act
        var result = await handler.Handle(new GetAllAuthorsQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().FullName.Should().Be("Robert Martin");
    }

    [Fact]
    public async Task CreateAuthorCommand_ReturnsSuccess_WhenValidData()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateWithSeededDb();
        var handler = new CreateAuthorCommandHandler(uow);
        var command = new CreateAuthorCommand(
            FirstName: "Martin",
            LastName: "Fowler",
            Bio: "Software engineer and author",
            Email: "martin@fowler.com"
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.FullName.Should().Be("Martin Fowler");
        result.Data.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateAuthorCommand_ReturnsFail_WhenEmailAlreadyExists()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateWithSeededDb();
        var handler = new CreateAuthorCommandHandler(uow);

        // Email "robert@test.com" is already seeded
        var command = new CreateAuthorCommand(
            FirstName: "Another",
            LastName: "Person",
            Bio: null,
            Email: "robert@test.com"
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("email"));
    }

    [Fact]
    public async Task CreateAuthorCommand_ReturnsSuccess_WhenEmailIsNull()
    {
        // Arrange
        var uow = UnitOfWorkFactory.CreateEmpty();
        var handler = new CreateAuthorCommandHandler(uow);
        var command = new CreateAuthorCommand("No", "Email", null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
    }
}
