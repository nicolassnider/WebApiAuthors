using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using WebApiAuthors.Controllers.Entities;
using WebApiAuthors.Filters;
using WebApiAuthors.Services;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/[controller]")]
    public class AuthorsController:ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IService service;
        private readonly ServiceTransient serviceTransient;
        private readonly ServiceScoped serviceScoped;
        private readonly ServiceSingleton serviceSingleton;
        private readonly ILogger<AuthorsController> logger;

        public AuthorsController(ApplicationDbContext context,IService service,ServiceTransient serviceTransient,
            ServiceScoped serviceScoped, ServiceSingleton serviceSingleton, ILogger<AuthorsController> logger)
        {
            this.context = context;
            this.service = service;
            this.serviceTransient = serviceTransient;
            this.serviceScoped = serviceScoped;
            this.serviceSingleton = serviceSingleton;
            this.logger = logger;
        }
        [HttpGet("GUID")]
        //[ResponseCache(Duration =10)]
        [ServiceFilter(typeof(MyActionFilter))]
        public ActionResult GetGUID()
        {
            
            return Ok(new
            {
                AuthorsCOntrollerTranstient = serviceTransient.guid,
                AuthorsCOntrollerScoped = serviceScoped.guid,
                AuthorsCOntrollerSingleton = serviceSingleton.guid,
                ServiceA_Transient = service.getTransient(),
                ServiceA_Scoped = service.getScoped(),
                ServiceA_Singleton = service.getSingleton()
            });
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
        [Authorize]
        public async Task<ActionResult<List<Author>>> Get()/* do not expose entities, use DTO*/
        {
            logger.LogInformation("logging info");
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
            var AuthorsCOntrollerTranstient = serviceTransient.guid;
            var AuthorsCOntrollerScoped = serviceScoped.guid;
            var AuthorsCOntrollerSingleton = serviceSingleton.guid;
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
