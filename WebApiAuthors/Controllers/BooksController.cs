using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Controllers.Entities;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController:ControllerBase
    {
        private readonly ApplicationDbContext context;

        public BooksController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Book>> Get(int id)
        {
            var book = await context.Books.Include(book=>book.Author).FirstOrDefaultAsync(book => book.Id == id);
            if (book==null)
            {
                return NotFound();
            }
            return Ok(book);
        }
        [HttpPost]
        public async Task<ActionResult<Book>> Post(Book book)
        {
            if (!await context.Authors.AnyAsync(author => author.Id == book.AuthorId))
            {
                return BadRequest("invalid author");
            }
            context.Add(book);
            await context.SaveChangesAsync();
            return Ok(book);
            
        }

    }
}
