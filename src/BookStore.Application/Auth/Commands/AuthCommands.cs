using BookStore.Application.Common.Interfaces;
using BookStore.Application.Common.Models;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BookStore.Application.Auth.Commands;

// ── Register ──────────────────────────────────────────────────────────────────
public record RegisterCommand(string Username, string Email, string Password) : IRequest<ApiResult<AuthTokenDto>>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
    }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResult<AuthTokenDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtService _jwt;

    public RegisterCommandHandler(IUnitOfWork uow, IJwtService jwt)
    {
        _uow = uow;
        _jwt = jwt;
    }

    public async Task<ApiResult<AuthTokenDto>> Handle(RegisterCommand request, CancellationToken ct)
    {
        if (await _uow.Users.EmailExistsAsync(request.Email))
            return ApiResult<AuthTokenDto>.Fail("Email already registered.");

        var user = new AppUser
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "User"
        };

        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync();

        var token = _jwt.GenerateToken(user);
        return ApiResult<AuthTokenDto>.Ok(new AuthTokenDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        }, "Registration successful.");
    }
}

// ── Login ─────────────────────────────────────────────────────────────────────
public record LoginCommand(string Email, string Password) : IRequest<ApiResult<AuthTokenDto>>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResult<AuthTokenDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtService _jwt;

    public LoginCommandHandler(IUnitOfWork uow, IJwtService jwt)
    {
        _uow = uow;
        _jwt = jwt;
    }

    public async Task<ApiResult<AuthTokenDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _uow.Users.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return ApiResult<AuthTokenDto>.Fail("Invalid email or password.");

        var token = _jwt.GenerateToken(user);
        return ApiResult<AuthTokenDto>.Ok(new AuthTokenDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        }, "Login successful.");
    }
}
