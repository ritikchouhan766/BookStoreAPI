using BookStore.Application.Common.Models;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookStore.Application.Authors.Commands.CreateAuthor;

public record CreateAuthorCommand(
    string FirstName,
    string LastName,
    string? Bio,
    string? Email
) : IRequest<ApiResult<AuthorDto>>;

public class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
{
    public CreateAuthorCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}

public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, ApiResult<AuthorDto>>
{
    private readonly IUnitOfWork _uow;
    public CreateAuthorCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ApiResult<AuthorDto>> Handle(CreateAuthorCommand request, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(request.Email) &&
            await _uow.Authors.EmailExistsAsync(request.Email))
            return ApiResult<AuthorDto>.Fail("An author with this email already exists.");

        var author = new Author
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Bio = request.Bio,
            Email = request.Email
        };

        await _uow.Authors.AddAsync(author);
        await _uow.SaveChangesAsync();

        return ApiResult<AuthorDto>.Ok(new AuthorDto
        {
            Id = author.Id,
            FirstName = author.FirstName,
            LastName = author.LastName,
            FullName = author.FullName,
            Bio = author.Bio,
            Email = author.Email,
            BookCount = 0
        }, "Author created successfully.");
    }
}
