using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Comments.Dtos;
using Application.Errors;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class Create
    {
        public class Command:IRequest<CommentDto>
        {
            public Guid ActivityId { get; set; }
            public string Username { get; set; }
            [Required]
            public string Body { get; set; }
        }
        public class Handler : IRequestHandler<Command, CommentDto>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context,IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<CommentDto> Handle(Command request, CancellationToken cancellationToken)
            {
                var user=await _context.Users.SingleOrDefaultAsync(x=>x.UserName==request.Username);
                
                if (user==null)
                {
                    throw new RestException(HttpStatusCode.BadRequest,new {msg="Not Found User"});
                }
                  var activity =await _context.Activities.SingleOrDefaultAsync(x=>x.Id==request.ActivityId);
                
                if (activity==null)
                {
                    throw new RestException(HttpStatusCode.BadRequest,new {msg="Not Found User"});
                }
                var comment=new Comment
                {
                    Id=Guid.NewGuid(), Author=user,Activity=activity,CreatAt=DateTime.Now,  Body=request.Body  
                };
                _context.Comments.Add(comment);
                var result=await _context.SaveChangesAsync()>0;
                if (result)
                {
                    var CommentToReturen=_mapper.Map<CommentDto>(comment);
                    return CommentToReturen;
                }
                throw new Exception("Proplem Saving Changes");
            }
        }
    }
}