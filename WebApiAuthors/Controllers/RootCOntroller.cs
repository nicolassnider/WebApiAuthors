using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAuthors.Controllers.Entities;
using WebApiAuthors.DTOs;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }
        [HttpGet(Name = "GetRoot")]
        [AllowAnonymous]
        
        public async Task <ActionResult<IEnumerable<HATEOASData>>> Get()
        {
            var hateoasData = new List<HATEOASData>();
            var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");
            hateoasData.Add(new HATEOASData(link: Url.Link("GetRoot", new { }), description: "self", method: "GET"));
            hateoasData.Add(new HATEOASData(link: Url.Link("getAuthors", new { }), description: "get Authors", method: "GET"));
            hateoasData.Add(new HATEOASData(link: Url.Link("getAuthorById", new { }), description: "get Author by id", method: "GET"));
            hateoasData.Add(new HATEOASData(link: Url.Link("getAuthorByName", new { }), description: "get Author by name", method: "GET"));
            //mostrará si son admin
            if (isAdmin.Succeeded)
            {
                hateoasData.Add(new HATEOASData(link: Url.Link("updateAuthor", new { }), description: "update Author", method: "GET"));
                hateoasData.Add(new HATEOASData(link: Url.Link("deleteAuthor", new { }), description: "self", method: "GET"));
            }
            return hateoasData;
        }
    }
}
