using BookStore.Application.Common.Models;
using BookStore.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookStore.Application.Books.Commands.UpdateBook;

public record UpdateBookCommand(
    int Id, string Title, string ISBN, decimal Price,
    int StockQuantity, DateTime PublishedDate,
    string? Description, string? Genre, int AuthorId, int PublisherId
) : IRequest<ApiResult<BookDto>>;

public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
    }
}

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, ApiResult<BookDto>>
{
    private readonly IUnitOfWork _uow;
    public UpdateBookCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ApiResult<BookDto>> Handle(UpdateBookCommand request, CancellationToken ct)
    {
        var book = await _uow.Books.GetByIdAsync(request.Id);
        if (book == null) return ApiResult<BookDto>.Fail($"Book with ID {request.Id} not found.");

        if (await _uow.Books.IsbnExistsAsync(request.ISBN, request.Id))
            return ApiResult<BookDto>.Fail($"ISBN '{request.ISBN}' is already used by another book.");

        book.Title = request.Title;
        book.ISBN = request.ISBN;
        book.Price = request.Price;
        book.StockQuantity = request.StockQuantity;
        book.PublishedDate = request.PublishedDate;
        book.Description = request.Description;
        book.Genre = request.Genre;
        book.AuthorId = request.AuthorId;
        book.PublisherId = request.PublisherId;
        book.UpdatedAt = DateTime.UtcNow;

        await _uow.Books.UpdateAsync(book);
        await _uow.SaveChangesAsync();

        return ApiResult<BookDto>.Ok(new BookDto
        {
            Id = book.Id, Title = book.Title, ISBN = book.ISBN,
            Price = book.Price, StockQuantity = book.StockQuantity,
            AuthorId = book.AuthorId, PublisherId = book.PublisherId
        }, "Book updated successfully.");
    }
}
