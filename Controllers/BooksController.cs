using Microsoft.AspNetCore.Mvc;
using BookCatalog.Models;
using BookCatalog.Services;
using BookCatalog.DTOs;
using BookCatalog.Validators;

namespace BookCatalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(BookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? author, [FromQuery] string? genre, [FromQuery] int? year, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var books = _bookService.FilterBooks(author, genre, year);
            var paged = books.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Genre = b.Genre,
                    PublishedYear = b.PublishedYear,
                    Price = b.Price
                });
            return Ok(paged);
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query parameter is required.");

            var books = _bookService.GetAllBooks()
                .Where(b => (b.Title != null && b.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                         || (b.GetType().GetProperty("Description") != null && b.GetType().GetProperty("Description")?.GetValue(b)?.ToString()?.Contains(query, StringComparison.OrdinalIgnoreCase) == true))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Genre = b.Genre,
                    PublishedYear = b.PublishedYear,
                    Price = b.Price
                });
            return Ok(books);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null) return NotFound();
            var dto = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                PublishedYear = book.PublishedYear,
                Price = book.Price
            };
            return Ok(dto);
        }

        [HttpPost]
        public IActionResult Add([FromBody] CreateBookDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var errors = BookValidator.ValidateCreate(dto);
            if (errors.Any()) return BadRequest(errors);
            var book = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                Genre = dto.Genre,
                PublishedYear = dto.PublishedYear,
                Price = dto.Price
            };
            _bookService.AddBook(book);
            _logger.LogInformation("Book added: {Title} by {Author}", book.Title, book.Author);
            var resultDto = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                PublishedYear = book.PublishedYear,
                Price = book.Price
            };
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, resultDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateBookDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var errors = BookValidator.ValidateUpdate(dto);
            if (errors.Any()) return BadRequest(errors);
            var updatedBook = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                Genre = dto.Genre,
                PublishedYear = dto.PublishedYear,
                Price = dto.Price
            };
            var success = _bookService.UpdateBook(id, updatedBook);
            if (!success) return NotFound();
            _logger.LogInformation("Book updated (ID: {Id}): {Title} by {Author}", id, updatedBook.Title, updatedBook.Author);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var success = _bookService.DeleteBook(id);
            if (!success) return NotFound();
            _logger.LogInformation("Book deleted (ID: {Id})", id);
            return NoContent();
        }
    }
}
