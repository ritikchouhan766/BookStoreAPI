using BookStore.Application.Common.Models;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookStore.Application.Publishers.Commands.CreatePublisher;

public record CreatePublisherCommand(
    string Name,
    string? Address,
    string? Website
) : IRequest<ApiResult<PublisherDto>>;

public class CreatePublisherCommandValidator : AbstractValidator<CreatePublisherCommand>
{
    public CreatePublisherCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Website).Must(w => w == null || Uri.TryCreate(w, UriKind.Absolute, out _))
            .WithMessage("Website must be a valid URL.");
    }
}

public class CreatePublisherCommandHandler : IRequestHandler<CreatePublisherCommand, ApiResult<PublisherDto>>
{
    private readonly IUnitOfWork _uow;
    public CreatePublisherCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ApiResult<PublisherDto>> Handle(CreatePublisherCommand request, CancellationToken ct)
    {
        var publisher = new Publisher
        {
            Name = request.Name,
            Address = request.Address,
            Website = request.Website
        };

        await _uow.Publishers.AddAsync(publisher);
        await _uow.SaveChangesAsync();

        return ApiResult<PublisherDto>.Ok(new PublisherDto
        {
            Id = publisher.Id,
            Name = publisher.Name,
            Address = publisher.Address,
            Website = publisher.Website,
            BookCount = 0
        }, "Publisher created successfully.");
    }
}
