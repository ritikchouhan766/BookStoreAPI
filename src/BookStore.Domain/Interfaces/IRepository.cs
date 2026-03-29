using BookStore.Domain.Entities;

namespace BookStore.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(int id);
}

public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> GetAllWithDetailsAsync();
    Task<Book?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Book>> GetByAuthorIdAsync(int authorId);
    Task<IEnumerable<Book>> GetByGenreAsync(string genre);
    Task<bool> IsbnExistsAsync(string isbn, int? excludeId = null);
}

public interface IAuthorRepository : IRepository<Author>
{
    Task<IEnumerable<Author>> GetAuthorsWithBooksAsync();
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
}

public interface IPublisherRepository : IRepository<Publisher>
{
    Task<IEnumerable<Publisher>> GetPublishersWithBooksAsync();
}

public interface IUserRepository : IRepository<AppUser>
{
    Task<AppUser?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
}

public interface IUnitOfWork : IDisposable
{
    IBookRepository Books { get; }
    IAuthorRepository Authors { get; }
    IPublisherRepository Publishers { get; }
    IUserRepository Users { get; }
    Task<int> SaveChangesAsync();
}
