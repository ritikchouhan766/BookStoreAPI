using BookStore.Application.Common.Models;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Books.Queries.GetBookById;

public record GetBookByIdQuery(int Id) : IRequest<ApiResult<BookDto>>;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, ApiResult<BookDto>>
{
    private readonly IUnitOfWork _uow;

    public GetBookByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ApiResult<BookDto>> Handle(GetBookByIdQuery request, CancellationToken ct)
    {
        var book = await _uow.Books.GetByIdWithDetailsAsync(request.Id);
        if (book == null)
            return ApiResult<BookDto>.Fail($"Book with ID {request.Id} not found.");

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
            AuthorName = book.Author.FullName,
            PublisherName = book.Publisher.Name,
            AuthorId = book.AuthorId,
            PublisherId = book.PublisherId,
            CreatedAt = book.CreatedAt
        });
    }
}
