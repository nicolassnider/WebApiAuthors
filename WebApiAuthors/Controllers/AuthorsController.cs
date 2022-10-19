using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using WebApiAuthors.Controllers.Entities;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController:ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AuthorsController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [HttpPost]
        public async Task<ActionResult> Post(Author author)
        {
            var existsSameNameAuthor = await context.Authors.AnyAsync(x => x.Name == author.Name);
            if (existsSameNameAuthor)
            {
                return BadRequest("Author with same name already exists");
            }
            context.Add(author);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [HttpGet("list")]
        [HttpGet("/list")]
        public async Task<ActionResult<List<Author>>> Get()/* do not expose entities, use DTO*/
        {
            return await context.Authors.Include(author=>author.Books).ToListAsync();
        }
        [HttpGet("firstauthor")]
        public async Task<ActionResult<Author>> GetFirstAuthor()
        {
            return await context.Authors.Include(author => author.Books).FirstOrDefaultAsync();
        }
        [HttpGet("firstauthorheader")]
        public async Task<ActionResult<Author>> GetFirstAuthor([FromHeader]int value)
        {

            return await context.Authors.Include(author => author.Books).FirstOrDefaultAsync();
        }
        [HttpGet("firstauthorquerystring")]
        public async Task<ActionResult<Author>> GetFirstAuthor([FromQuery] int value,string value2)
        {

            return await context.Authors.Include(author => author.Books).FirstOrDefaultAsync();
        }
        [HttpGet("syncauthor")]
        public ActionResult<Author> ReturnSyncAuthor()
        {
            return new Author() { Id=9999,Name="Sync Author"};
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Author>> GetAuthorByID(int id)
        {
            var author = await context.Authors.FirstOrDefaultAsync(author => author.Id == id);
            if (author == null) return NotFound();
            return Ok(author);
        }
        [HttpGet("{name}")]
        public async Task<ActionResult<Author>> GetAuthorByName(string name)
        {
            var author = await context.Authors.FirstOrDefaultAsync(author => author.Name == name);
            if (author == null) return NotFound();
            return Ok(author);
        }
        [HttpGet("{id:int}/{param2}")]
        public async Task<ActionResult<Author>> GetAuthorByIdPlusParam(int id,string param2)
        {
            var author = await context.Authors.FirstOrDefaultAsync(author => author.Id == id);
            if (author == null) return NotFound();
            return Ok(author);
        }
        [HttpGet("{id:int}/{optionalParam?}")]
        public async Task<ActionResult<Author>> GetAuthorByIdPlusOptionalParam(int id, string optionalParam)
        {
            var author = await context.Authors.FirstOrDefaultAsync(author => author.Id == id);
            if (author == null) return NotFound();
            return Ok(author);
        }
        [HttpGet("{id:int}/{defaultParam=default}")]
        public async Task<ActionResult<Author>> GetAuthorByIdPlusDefaultParam(int id, string defaultParam)
        {
            var author = await context.Authors.FirstOrDefaultAsync(author => author.Id == id);
            if (author == null) return NotFound();
            return Ok(author);
        }
        [HttpPut("{id:int}")]// api/authors/1
        public async Task<ActionResult<Author>> Put(Author author,int id)
        {
            if (author.Id!=id)
            {
                return BadRequest("Invalid author Id");
            }
            if (!await context.Authors.AnyAsync(author => author.Id == id))
            {
                return NotFound();
            }
            context.Update(author);
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Author>> Delete(int id)
        {
            var author = await context.Authors.AnyAsync(author => author.Id == id);
            if (!author)
            {
                return NotFound();
            }
            context.Remove(author);
            await context.SaveChangesAsync();
            return Ok();
           

        }
    }
}
