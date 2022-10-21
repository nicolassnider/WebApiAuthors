using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Controllers.Entities;
using WebApiAuthors.DTOs;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController:ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AuthorsController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }
        [HttpGet("surnameconfig")]
        public ActionResult<string> GetSurnameConfig()
        {
            var surnameconfig = configuration["surname"];
            if (surnameconfig==null)
            {
                return NotFound();
            }
            return Ok(new{ surname=surnameconfig });
        }

        [HttpPost]
        public async Task<ActionResult> Post(AuthorCreationDTO authorCreationDTO)
        {
            var existsSameNameAuthor = await context.Authors.AnyAsync(x => x.Name == authorCreationDTO.Name);
            if (existsSameNameAuthor)
            {
                return BadRequest("Author with same name already exists");
            }
            
            var author = mapper.Map<Author>(authorCreationDTO);
            context.Add(author);
            await context.SaveChangesAsync();
            var authorDTO = mapper.Map<AuthorDTO>(author);
            return CreatedAtRoute("getAuthorById", new {id=author.Id},authorDTO);
        }

        [HttpGet]
        
        public async Task<ActionResult<List<AuthorDTO>>> Get()
        {
            var authors= await context.Authors.ToListAsync();
            return mapper.Map<List<AuthorDTO>>(authors);
        }
        
        [HttpGet("{id:int}",Name ="getAuthorById")]
        public async Task<ActionResult<AuthorDTOWithBooks>> GetAuthorByID(int id)
        {
            var author = await context.Authors
                .Include(authorDB => authorDB.AuthorsBooks)
                .ThenInclude(authorsBooksDB => authorsBooksDB.Book)
                .FirstOrDefaultAsync(author => author.Id == id);
            if (author == null) return NotFound();
            return mapper.Map<AuthorDTOWithBooks>(author);
        }
        [HttpGet("{name}")]
        public async Task<ActionResult<List<AuthorDTO>>> GetAuthorsByName(string name)
        {
            var authors = await context.Authors.Where(authorBd => authorBd.Name.Contains(name)).ToListAsync();
            if (authors == null) return NotFound();
            return mapper.Map<List<AuthorDTO>>(authors);
        }
        
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Author>> Put(AuthorCreationDTO authorCreationDTO,int id)
        {
            
            if (!await context.Authors.AnyAsync(author => author.Id == id))
            {
                return NotFound();
            }
            var author = mapper.Map<Author>(authorCreationDTO);
            author.Id = id;
            context.Update(author);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Author>> Delete(int id)
        {
            var authorExists = await context.Authors.AnyAsync(author => author.Id == id);
            if (!authorExists)
            {
                return NotFound();
            }
            context.Remove(authorExists);
            await context.SaveChangesAsync();
            return NoContent();
           

        }
    }
}
