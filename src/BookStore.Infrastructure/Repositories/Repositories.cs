using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories;

// ── Generic Base ──────────────────────────────────────────────────────────────
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _db;
    protected readonly DbSet<T> _set;

    public Repository(AppDbContext db) { _db = db; _set = db.Set<T>(); }

    public async Task<T?> GetByIdAsync(int id) => await _set.FindAsync(id);
    public async Task<IEnumerable<T>> GetAllAsync() => await _set.ToListAsync();
    public async Task<T> AddAsync(T entity) { await _set.AddAsync(entity); return entity; }
    public Task UpdateAsync(T entity) { _set.Update(entity); return Task.CompletedTask; }
    public Task DeleteAsync(T entity) { _set.Remove(entity); return Task.CompletedTask; }
    public async Task<bool> ExistsAsync(int id) => await _set.AnyAsync(e => e.Id == id);
}

// ── Book Repository ───────────────────────────────────────────────────────────
public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<Book>> GetAllWithDetailsAsync() =>
        await _db.Books.Include(b => b.Author).Include(b => b.Publisher)
            .OrderByDescending(b => b.CreatedAt).ToListAsync();

    public async Task<Book?> GetByIdWithDetailsAsync(int id) =>
        await _db.Books.Include(b => b.Author).Include(b => b.Publisher)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<IEnumerable<Book>> GetByAuthorIdAsync(int authorId) =>
        await _db.Books.Include(b => b.Publisher)
            .Where(b => b.AuthorId == authorId).ToListAsync();

    public async Task<IEnumerable<Book>> GetByGenreAsync(string genre) =>
        await _db.Books.Include(b => b.Author).Include(b => b.Publisher)
            .Where(b => b.Genre != null && b.Genre.ToLower() == genre.ToLower()).ToListAsync();

    public async Task<bool> IsbnExistsAsync(string isbn, int? excludeId = null) =>
        await _db.Books.AnyAsync(b => b.ISBN == isbn && (excludeId == null || b.Id != excludeId));
}

// ── Author Repository ─────────────────────────────────────────────────────────
public class AuthorRepository : Repository<Author>, IAuthorRepository
{
    public AuthorRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<Author>> GetAuthorsWithBooksAsync() =>
        await _db.Authors.Include(a => a.Books).OrderBy(a => a.LastName).ToListAsync();

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null) =>
        await _db.Authors.AnyAsync(a => a.Email == email && (excludeId == null || a.Id != excludeId));
}

// ── Publisher Repository ──────────────────────────────────────────────────────
public class PublisherRepository : Repository<Publisher>, IPublisherRepository
{
    public PublisherRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<Publisher>> GetPublishersWithBooksAsync() =>
        await _db.Publishers.Include(p => p.Books).OrderBy(p => p.Name).ToListAsync();
}

// ── User Repository ───────────────────────────────────────────────────────────
public class UserRepository : Repository<AppUser>, IUserRepository
{
    public UserRepository(AppDbContext db) : base(db) { }

    public async Task<AppUser?> GetByEmailAsync(string email) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<bool> EmailExistsAsync(string email) =>
        await _db.Users.AnyAsync(u => u.Email == email);
}

// ── Unit of Work ──────────────────────────────────────────────────────────────
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public IBookRepository Books { get; }
    public IAuthorRepository Authors { get; }
    public IPublisherRepository Publishers { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        Books = new BookRepository(db);
        Authors = new AuthorRepository(db);
        Publishers = new PublisherRepository(db);
        Users = new UserRepository(db);
    }

    public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();
    public void Dispose() => _db.Dispose();
}
