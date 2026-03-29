using BookStore.Application.Common.Models;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Books.Commands.DeleteBook;

public record DeleteBookCommand(int Id) : IRequest<ApiResult<bool>>;

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, ApiResult<bool>>
{
    private readonly IUnitOfWork _uow;
    public DeleteBookCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ApiResult<bool>> Handle(DeleteBookCommand request, CancellationToken ct)
    {
        var book = await _uow.Books.GetByIdAsync(request.Id);
        if (book == null) return ApiResult<bool>.Fail($"Book with ID {request.Id} not found.");

        await _uow.Books.DeleteAsync(book);
        await _uow.SaveChangesAsync();

        return ApiResult<bool>.Ok(true, "Book deleted successfully.");
    }
}
