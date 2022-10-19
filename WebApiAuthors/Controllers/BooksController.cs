using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApiAuthors.Controllers.Entities;
using WebApiAuthors.DTOs;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController:ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public BooksController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [HttpGet("{id:int}",Name ="getBookById")]
        public async Task<ActionResult<BookDTOWithAuthors>> Get(int id)
        {
            var book = await context.Books
                .Include(bookBD => bookBD.Comments)
                .Include(bookBD => bookBD.AuthorsBooks)
                .ThenInclude(authorsBooksBD => authorsBooksBD.Author)
                .FirstOrDefaultAsync(book => book.Id == id);
            if (book==null)
            {
                return NotFound();
            }

            book.AuthorsBooks.OrderBy(book => book.Order).ToList();

            return mapper.Map<BookDTOWithAuthors>(book);
        }
        [HttpPost]
        public async Task<ActionResult<Book>> Post(BookCreationDTO bookCreationDTO)
        {
            if (bookCreationDTO.AuthorIds==null) return BadRequest("AuthorIds is required");
            var authorIds = await context.Authors.Where(authorBD => bookCreationDTO.AuthorIds.Contains(authorBD.Id)).Select(x=>x.Id).ToListAsync();
            if (bookCreationDTO.AuthorIds.Count()!=authorIds.Count()) return BadRequest("invalid authors");
            var book = mapper.Map<Book>(bookCreationDTO);
            AuthorsOrderAssign(book);
            
            context.Add(book);
            await context.SaveChangesAsync();
            var bookDTO = mapper.Map<BookDTO>(book);
            return CreatedAtRoute("getBookById", new { id = book.Id }, bookDTO);
            
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, BookCreationDTO bookCreationDTO)
        {
            var bookDB = await context.Books
                .Include(x => x.AuthorsBooks)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (bookDB==null)
            {
                return NotFound("Book not found");
            }
            
            bookDB = mapper.Map(bookCreationDTO, bookDB); //keep instance and replace
            AuthorsOrderAssign(bookDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<BookPatchDTO> patchDocument)
        {
            if (patchDocument==null)
            {
                return BadRequest();
            }
            var bookDB = await context.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (bookDB==null)
            {
                return NotFound();

            }
            var bookDTO = mapper.Map<BookPatchDTO>(bookDB);
            patchDocument.ApplyTo(bookDTO, ModelState);
            var isValid = TryValidateModel(bookDTO);
            if (!isValid) return BadRequest(ModelState);
            mapper.Map(bookDTO, bookDB);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Book>> Delete(int id)
        {
            var bookExists = await context.Books.AnyAsync(book => book.Id == id);
            if (!bookExists)
            {
                return NotFound();
            }
            context.Remove(bookExists);
            await context.SaveChangesAsync();
            return NoContent();


        }

        private void AuthorsOrderAssign(Book book)
        {
            if (book.AuthorsBooks != null)
            {
                for (int i = 0; i < book.AuthorsBooks.Count(); i++)
                {
                    book.AuthorsBooks[i].Order = i;
                }
            }
        }


    }
}
