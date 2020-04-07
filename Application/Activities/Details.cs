using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities.Dtos;
using Application.Errors;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Details
    {
        public class Query:IRequest<ActivityDto>{
            public Guid Id { get; set; }
        }
        public class Handler : IRequestHandler<Query, ActivityDto>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context,IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<ActivityDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.Id);
                if(activity==null)
                {
                    throw new RestException(HttpStatusCode.NotFound,new {msg="Activity Not Found"});
                }
                var activityToReturen = _mapper.Map<ActivityDto>(activity);
                    return activityToReturen;        
                }
        }
    }
}