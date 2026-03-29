using BookStore.Application.Common.Models;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookStore.Application.Publishers.Queries.GetAllPublishers;

public record GetAllPublishersQuery : IRequest<IEnumerable<PublisherDto>>;

public class GetAllPublishersQueryHandler : IRequestHandler<GetAllPublishersQuery, IEnumerable<PublisherDto>>
{
    private readonly IUnitOfWork _uow;
    public GetAllPublishersQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<PublisherDto>> Handle(GetAllPublishersQuery request, CancellationToken ct)
    {
        var publishers = await _uow.Publishers.GetPublishersWithBooksAsync();
        return publishers.Select(p => new PublisherDto
        {
            Id = p.Id,
            Name = p.Name,
            Address = p.Address,
            Website = p.Website,
            BookCount = p.Books.Count
        });
    }
}
