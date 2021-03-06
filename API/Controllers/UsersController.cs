using System.Threading.Tasks;
using Application.Users;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UsersController:BaseController
    {
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<User>> Login(Login.Query query)
        {
            return await Mediator.Send(query);
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(Register.Command command)
        {
            return await Mediator.Send(command);
        }
        [HttpGet("")]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            return await Mediator.Send(new CurrentUser.Query());
        }
        [HttpPut("")]
        public async Task<ActionResult<User>> Edit(Edit.Command command)
        {
            return await Mediator.Send(command);
        }
    }
}