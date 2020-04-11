using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Application.Followers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Profiles;

namespace API.Controllers
{
    [Route("api/profile")]
    public class UserFollowingsController:BaseController
    {
        [HttpPost("{username}/follow")]
        public async Task<ActionResult<Unit>> Follow(string username)
        {
            return await Mediator.Send(new Add.Command{Username=username});
        }
        [HttpPost("{username}/unfollow")]
        public async Task<ActionResult<Unit>> Unfollow(string username)
        {
            return await Mediator.Send(new Delete.Command{Username=username});
        }

        [HttpGet("{username}/follow")]
        public async Task<List<Profile>> GetFollowings(string username,string predicate)
        {
            return await Mediator.Send(new List.Query{Username=username,Predicate=predicate});
        }
    }
}