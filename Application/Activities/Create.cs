using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        public class Command : IRequest
        {
            [Required]
            public string Title { get; set; }
            [Required,MinLength(3),MaxLength(60)]
            public string Description { get; set; }
            [Required]
            public string Category { get; set; }
            [Required]
            [DataType(DataType.Date)]
            public DateTime Date { get; set; }
            [Required]
            public string City { get; set; }
            [Required]
            public string Venue { get; set; }
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
                var activity = new Activity
                {
                    Id=Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                    Category = request.Category,
                    Date = request.Date,
                    City = request.City,
                    Venue = request.Venue
                };
                _context.Activities.Add(activity);
                var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == _userAccessor.CurrentUsername());
                var attendee = new UserActivity
                {
                    User = user,
                    Activity = activity,
                    DateJoined = DateTime.Now,
                    IsHost = true
                };
                _context.UserActivities.Add(attendee);

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