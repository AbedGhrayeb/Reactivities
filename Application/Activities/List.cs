using System.Threading;
using System.Collections.Generic;
using Domain;
using MediatR;
using System.Threading.Tasks;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Application.Activities.Dtos;
using AutoMapper;

namespace Application.Activities
{
    public class List
    {
        public class Query:IRequest<List<ActivityDto>>{}
        public class Handler : IRequestHandler<Query, List<ActivityDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context,IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activities= await _context.Activities.ToListAsync();
                var activitiesToReturen = _mapper.Map <List<Activity>,List<ActivityDto>>(activities);
                return activitiesToReturen;
            }
        }
    }
}