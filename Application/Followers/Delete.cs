using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class Delete
    {
        public class Command : IRequest
        {
            public string Username { get; set; }
        }
        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var observe= await _context.Users.SingleOrDefaultAsync(x=>x.UserName==_userAccessor.CurrentUsername());
                var target = await _context.Users.SingleOrDefaultAsync(x=>x.UserName==request.Username);
                if (target==null)
                {
                    throw new RestException(HttpStatusCode.BadRequest,new {msg="Not Found User"});
                }

                var following = await _context.Followings.SingleOrDefaultAsync(x=>x.ObserverId==observe.Id && 
                    x.TargetId==target.Id);

                    if (following == null)
                    {
                        throw new RestException(HttpStatusCode.BadRequest,new {msg="You are not following this user"});
                    }
                    else if(following != null)
                    {
                        
                        _context.Followings.Remove(following);
                    }
                    var result= await _context.SaveChangesAsync()>0;
                    if (result)
                    {
                        return Unit.Value;
                    }
                    throw new Exception("Proplem Saving Changes");
            }
        }
         
    }
    
}