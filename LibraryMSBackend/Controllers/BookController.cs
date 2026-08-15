using MediatR;
using LibraryMSBackend.Applications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BooksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetBooks()
        {
            var result = await _mediator.Send(new GetBooksQuery());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBook([FromBody] AddBookCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("{bookId}/borrow")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> BorrowBook(Guid bookId, [FromBody] DateTime dueDate)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var command = new BorrowBookCommand { BookId = bookId, UserId = userId, DueDate = dueDate };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("return/{borrowRecordId}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> ReturnBook(Guid borrowRecordId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var command = new ReturnBookCommand { BorrowRecordId = borrowRecordId, UserId = userId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("my-history")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> GetMyHistory()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new GetMyBorrowHistoryQuery { UserId = userId });
            return Ok(result);
        }
    
    

    [HttpDelete("{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Delete(Guid id)
{
    try
    {
        await _mediator.Send(new DeleteBookCommand { Id = id });
        return NoContent();
    }
    catch (InvalidOperationException ex)
    {
        return Conflict(new { message = ex.Message });
    }
}




    
    
    
    }
}