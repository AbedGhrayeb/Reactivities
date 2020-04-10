using System.Linq;
using Application.Errors;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users
{
    public class Edit
    {
        public class Command : IRequest<User> 
        {
            public string DisplayName { get; set; }
            public string Email { get; set; }
        }
        public class Handler : IRequestHandler<Command, User>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly UserManager<AppUser> _userManager;
            private readonly IJwtJenerator _jwtJenerator;

            public Handler(IUserAccessor userAccessor,UserManager<AppUser> userManager,IJwtJenerator jwtJenerator)
            {
                _userAccessor = userAccessor;
                _userManager = userManager;
                _jwtJenerator = jwtJenerator;
            }
            public async Task<User> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(_userAccessor.CurrentUsername());
                user.DisplayName = request.DisplayName ?? user.DisplayName;

                    if ((await _userManager.Users.AnyAsync(x=>x.Email==request.Email)
                        && request.Email!= user.Email))
                    {
                    throw new RestException(HttpStatusCode.BadRequest, new { msg = "This Email Already Taken" });
                    }
                user.Email = request.Email;
                var result= await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return new User
                    {
                        DisplayName = user.DisplayName,
                        Username = user.UserName,
                        Email = user.Email,
                        Token = _jwtJenerator.CreateToken(user),
                        Image = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url
                    };
                }
                throw new Exception("Proplem Saving Changes");
            }
        }
    }
}
