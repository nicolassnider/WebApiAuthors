using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using WebApiAuthors.DTOs;
using WebApiAuthors.Services;

namespace WebApiAuthors.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;
        public AccountsController(
            UserManager<IdentityUser> userManager, 
            IConfiguration configuration, 
            SignInManager<IdentityUser> signInManager,
            IDataProtectionProvider dataProtectionProvider,HashService hashService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector =dataProtectionProvider.CreateProtector("unique_secret_value");
        }
        [HttpGet("hash/{plainText}")]
        public ActionResult MakeHash(string plainText)
        {
            var result1 = hashService.Hash(plainText);
            var result2 = hashService.Hash(plainText);
            return Ok(new
            {
                Hash1 = result1.Hash,
                Salt1 = result1.Salt,
                Hash2 = result2.Hash,
                Salt2 = result2.Salt        
            });
        }
        
        [HttpGet("encrypt")]
        public ActionResult Encrypt()
        {
            var timeSpanDataProtector = dataProtector.ToTimeLimitedDataProtector();

            var plainText = "juán perez";
            var cipherText = dataProtector.Protect(plainText);
            var timeSpanCipherText = timeSpanDataProtector.Protect(plainText,lifetime:TimeSpan.FromSeconds(5));
            var decryptedText = dataProtector.Unprotect(cipherText);
            return Ok((plainText, timeSpanCipherText,cipherText, decryptedText));
        }
        [HttpPost("login")]
        public async Task<ActionResult<ResponseAuthentication>> Login(UserCredentials userCredentials)
        {
            var result = await signInManager.PasswordSignInAsync(
                userCredentials.Email, 
                userCredentials.Password, 
                isPersistent: false, 
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuildToken(userCredentials);

            }
            return BadRequest("Incorrect lgin");
        }


        [HttpPost("register")]
        public async Task<ActionResult<ResponseAuthentication>> Register(UserCredentials userCredentials)
        {
            var user = new IdentityUser { UserName = userCredentials.Email, Email = userCredentials.Email };
            var result = await userManager.CreateAsync(user, userCredentials.Password);

            if (result.Succeeded)
            {
                return await BuildToken(userCredentials);
            }
            return BadRequest(result.Errors);

            
        }
        [HttpGet("renewToken")]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        public async Task <ActionResult<ResponseAuthentication>> RenewToken()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var userCredentials = new UserCredentials() { Email = email };
            return await BuildToken(userCredentials);
        } 
        private async Task <ResponseAuthentication> BuildToken(UserCredentials userCredentials)
        {
            var claims = new List<Claim>
            {
                new Claim("email", userCredentials.Email),

            };

            var user = await userManager.FindByEmailAsync(userCredentials.Email);
            var claimsDb = await userManager.GetClaimsAsync(user);

            claims.AddRange(claimsDb);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwtKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(30); //TODO: verify expiration
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: credentials);
            return new ResponseAuthentication()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration

            };
        }
        [HttpPost("makeAdmin")]
        public async Task<ActionResult> MakeAdmin(EditAdminDTO editAdminDTO)
        {
            var user = await userManager.FindByEmailAsync(editAdminDTO.Email);
            await userManager.AddClaimAsync(user, new Claim("IsAdmin", "1"));
            return NoContent();
        }
        [HttpPost("removeAdmin")]
        public async Task<ActionResult> RemoveAdmin(EditAdminDTO editAdminDTO)
        {
            var user = await userManager.FindByEmailAsync(editAdminDTO.Email);
            await userManager.RemoveClaimAsync(user, new Claim("IsAdmin", "1"));
            return NoContent();
        }


    }
}

