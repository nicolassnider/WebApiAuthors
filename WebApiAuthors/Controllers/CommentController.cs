using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Controllers.Entities;
using WebApiAuthors.DTOs;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/books/{bookId:int}/comments")]
    public class CommentController:ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CommentController(ApplicationDbContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<CommentDTO>>> GetComments(int bookId)
        {
            var comments = await context.Comments.Where(commentDB=>commentDB.BookId==bookId).ToListAsync();
            return mapper.Map<List<CommentDTO>>(comments);
        }

        [HttpGet("{id:int}",Name ="getCommentById")]
        public async Task<ActionResult<CommentDTO>> GetCommentByID(int id) { 
            var comment = await context.Comments.FirstOrDefaultAsync(commentDB=>commentDB.Id==id);
            if (comment==null)
            {
                return NotFound();
            }
            return mapper.Map<CommentDTO>(comment);
        }
        

        [HttpPost]
        public async Task<ActionResult<CommentDTO>> Post(int bookId, CommentCreationDTO commentCreationDTO)
        {
            var book = await context.Books.AnyAsync(bookDB => bookDB.Id == bookId);
            if (!book)
            {
                return NotFound();
            }

            var comment = mapper.Map<Comment>(commentCreationDTO);
            comment.BookId = bookId;
            context.Add(comment);
            await context.SaveChangesAsync();
            var commentDTO = mapper.Map<CommentDTO>(comment);
            return CreatedAtRoute("getCommentById", new { id = comment.Id, bookId = bookId }, commentDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Comment>> Put(int bookId, int id,CommentCreationDTO commentCreationDTO)
        {
            var bookExists = await context.Books.AnyAsync(bookDB => bookDB.Id == bookId);
            if (!bookExists)
            {
                return NotFound("Book not found");
            }
            var commentExists = await context.Comments.AnyAsync(commentDB => commentDB.Id == id);
            if (!commentExists)
            {
                return NotFound("comment not found");
            }
            var comment = mapper.Map<Comment>(commentCreationDTO);
            comment.Id = id;
            comment.BookId=bookId;
            context.Update(comment);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
