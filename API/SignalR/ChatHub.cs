using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Application.Comments;

namespace API.SignalR
{
    public class ChatHub:Hub
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }   
        public async Task SentComment(Create.Command command)
        {
            string username= Context.User?.Claims?.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier)?.Value;
            command.Username=username;
           var commant= await _mediator.Send(command);
            await Clients.All.SendAsync("ReceviedComment",commant);
        }
    }
}