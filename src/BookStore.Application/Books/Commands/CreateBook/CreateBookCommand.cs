using BookStore.Application.Common.Models;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookStore.Application.Books.Commands.CreateBook;

public record CreateBookCommand(
    string Title,
    string ISBN,
    decimal Price,
    int StockQuantity,
    DateTime PublishedDate,
    string? Description,
    string? Genre,
    int AuthorId,
    int PublisherId
) : IRequest<ApiResult<BookDto>>;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ISBN).NotEmpty().Length(10, 17);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AuthorId).GreaterThan(0);
        RuleFor(x => x.PublisherId).GreaterThan(0);
        RuleFor(x => x.PublishedDate).LessThanOrEqualTo(DateTime.UtcNow);
    }
}

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, ApiResult<BookDto>>
{
    private readonly IUnitOfWork _uow;

    public CreateBookCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ApiResult<BookDto>> Handle(CreateBookCommand request, CancellationToken ct)
    {
        if (await _uow.Books.IsbnExistsAsync(request.ISBN))
            return ApiResult<BookDto>.Fail($"ISBN '{request.ISBN}' already exists.");

        if (!await _uow.Authors.ExistsAsync(request.AuthorId))
            return ApiResult<BookDto>.Fail($"Author with ID {request.AuthorId} not found.");

        if (!await _uow.Publishers.ExistsAsync(request.PublisherId))
            return ApiResult<BookDto>.Fail($"Publisher with ID {request.PublisherId} not found.");

        var book = new Book
        {
            Title = request.Title,
            ISBN = request.ISBN,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            PublishedDate = request.PublishedDate,
            Description = request.Description,
            Genre = request.Genre,
            AuthorId = request.AuthorId,
            PublisherId = request.PublisherId
        };

        await _uow.Books.AddAsync(book);
        await _uow.SaveChangesAsync();

        return ApiResult<BookDto>.Ok(new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            ISBN = book.ISBN,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            PublishedDate = book.PublishedDate,
            Description = book.Description,
            Genre = book.Genre,
            AuthorId = book.AuthorId,
            PublisherId = book.PublisherId,
            CreatedAt = book.CreatedAt
        }, "Book created successfully.");
    }
}
