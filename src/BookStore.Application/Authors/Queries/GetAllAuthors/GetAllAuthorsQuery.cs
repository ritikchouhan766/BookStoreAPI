using BookStore.Application.Common.Models;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookStore.Application.Authors.Queries.GetAllAuthors;

public record GetAllAuthorsQuery : IRequest<IEnumerable<AuthorDto>>;

public class GetAllAuthorsQueryHandler : IRequestHandler<GetAllAuthorsQuery, IEnumerable<AuthorDto>>
{
    private readonly IUnitOfWork _uow;
    public GetAllAuthorsQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<AuthorDto>> Handle(GetAllAuthorsQuery request, CancellationToken ct)
    {
        var authors = await _uow.Authors.GetAuthorsWithBooksAsync();
        return authors.Select(a => new AuthorDto
        {
            Id = a.Id, FirstName = a.FirstName, LastName = a.LastName,
            FullName = a.FullName, Bio = a.Bio, Email = a.Email,
            BookCount = a.Books.Count
        });
    }
}
