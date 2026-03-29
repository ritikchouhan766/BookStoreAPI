namespace BookStore.Application.Common.Models;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime PublishedDate { get; set; }
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string PublisherName { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public int PublisherId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AuthorDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Email { get; set; }
    public int BookCount { get; set; }
}

public class PublisherDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Website { get; set; }
    public int BookCount { get; set; }
}

public class AuthTokenDto
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
