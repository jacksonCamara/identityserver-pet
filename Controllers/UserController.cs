using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> UserManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }
        
        [HttpGet]
        [Route("claims")]
        [AllowAnonymous]
        public async Task<IActionResult> GetClaims(string user)
        {
            //string user = null;
            System.Console.WriteLine("entrou no get");
            System.Console.WriteLine(user);
            var idUser = await UserManager.FindByEmailAsync(user);
            var userClaims = await UserManager.GetClaimsAsync(idUser);
            var userUsuario = await UserManager.GetUserIdAsync(idUser);
            var roles = await UserManager.GetRolesAsync(idUser);
            return Ok(roles);
        }

        [HttpGet]
        //[Authorize(Roles = "PayingUser")]
        //[AllowAnonymous]
        public IActionResult GetResultAsync()
        {
            System.Console.WriteLine(JwtBearerDefaults.AuthenticationScheme);
            if (User.Identity.IsAuthenticated)
            {
                System.Console.WriteLine("esta autenticado");
            }
            else
            {
                System.Console.WriteLine("Nao esta autenticado");
            }
            return new JsonResult("{ola api user - classe usercontroller2}");
        }
        /**/
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUserAsync([FromBody]UserCreateViewModel userCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = userCreateViewModel.Email, FirstName = userCreateViewModel.FirstName, LastName = userCreateViewModel.LastName, Email = userCreateViewModel.Email };
                //O método Create retorna um objeto de resultado de identidade. Obtem um sucesso booleano e uma colecao de erros
                var result = await UserManager.CreateAsync(user, userCreateViewModel.Password);
                if (result.Succeeded)
                {
                    var idUser = await UserManager.FindByNameAsync(user.Email);
                    var claimResult = await UserManager.AddClaimAsync(idUser, new Claim("email-publico", user.Email));
                    if (claimResult.Succeeded)
                    {
                        return StatusCode(201);
                    }
                }
                AddErrors(result);
            }
            return BadRequest(ModelState);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

    }
}