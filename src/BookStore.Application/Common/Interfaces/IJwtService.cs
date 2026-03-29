using BookStore.Domain.Entities;

namespace BookStore.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(AppUser user);
}
