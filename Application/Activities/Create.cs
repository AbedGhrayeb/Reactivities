using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
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

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = new Activity
                {
                    Title = request.Title,
                    Description = request.Description,
                    Category = request.Category,
                    Date = request.Date,
                    City = request.City,
                    Venue = request.Venue
                };
                _context.Activities.Add(activity);
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