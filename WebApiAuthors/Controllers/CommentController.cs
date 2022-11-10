using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> userManager;

        public CommentController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
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
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<CommentDTO>> Post(int bookId, CommentCreationDTO commentCreationDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var user = await userManager.FindByEmailAsync(email);
            var userId = user.Id;
            var bookExists = await context.Books.AnyAsync(bookDB => bookDB.Id == bookId);
            if (!bookExists)
            {
                return NotFound();
            }

            var comment = mapper.Map<Comment>(commentCreationDTO);
            comment.BookId = bookId;
            comment.UserId = userId;
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
