using BookStore.Application.Common.Models;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Books.Queries.GetAllBooks;

public record GetAllBooksQuery(string? Genre, int Page = 1, int PageSize = 10) : IRequest<PagedResult<BookDto>>;

public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, PagedResult<BookDto>>
{
    private readonly IUnitOfWork _uow;

    public GetAllBooksQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PagedResult<BookDto>> Handle(GetAllBooksQuery request, CancellationToken ct)
    {
        var books = await _uow.Books.GetAllWithDetailsAsync();

        if (!string.IsNullOrWhiteSpace(request.Genre))
            books = books.Where(b => b.Genre != null &&
                b.Genre.Contains(request.Genre, StringComparison.OrdinalIgnoreCase));

        var total = books.Count();
        var paged = books
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                ISBN = b.ISBN,
                Price = b.Price,
                StockQuantity = b.StockQuantity,
                PublishedDate = b.PublishedDate,
                Description = b.Description,
                Genre = b.Genre,
                AuthorName = b.Author.FullName,
                PublisherName = b.Publisher.Name,
                AuthorId = b.AuthorId,
                PublisherId = b.PublisherId,
                CreatedAt = b.CreatedAt
            });

        return new PagedResult<BookDto>
        {
            Items = paged,
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
