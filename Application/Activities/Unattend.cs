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
namespace Application.Activities
{
    public class Unattend
    {

        public class Command : IRequest
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context,IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.Id);
                if (activity==null)
                {
                    throw new RestException(HttpStatusCode.BadRequest,new {msg="Not Found Activity"});
                }
                var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == _userAccessor.CurrentUsername());
                var attendee =await _context.UserActivities.SingleOrDefaultAsync(x=>
                    x.ActivityId==activity.Id && x.UserId==user.Id);
                    if (attendee==null)
                    {
                        return Unit.Value;
                    }
                    if (attendee.IsHost)
                    {
                    throw new RestException(HttpStatusCode.BadRequest,new {msg="You can not remove your self as host"});
                    }
                _context.UserActivities.Remove(attendee);

                var success=await _context.SaveChangesAsync()>0;
               if (success)
               {
                    return Unit.Value;
               }
                    throw new Exception("Proplem Saving Changes");
                
            }
        }
    }

    }
