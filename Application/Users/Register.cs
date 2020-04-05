using Application.Errors;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users
{
    public class Register
    {
        public class Command:IRequest<User>
        {
            [Required]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }
            [Required,MinLength(3)]
            public string Username { get; set; }
            [Required,MinLength(3)]
            public string DisplayName { get; set; }
            [Required,DataType(DataType.Password)]
            public string Password { get; set; }

        }
        public class Handler : IRequestHandler<Command, User>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IJwtJenerator _jwtJenerator;

            public Handler(UserManager<AppUser> userManager,IJwtJenerator jwtJenerator)
            {
                _userManager = userManager;
                _jwtJenerator = jwtJenerator;
            }
            public async Task<User> Handle(Command request, CancellationToken cancellationToken)
            {
                if (await _userManager.Users.AnyAsync(x=>x.Email==request.Email))
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { msg = "This Email Already Token" });
                }
                if (await _userManager.Users.AnyAsync(x=>x.UserName==request.Username))
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { msg = "This Username Already Token" });
                }

                var user = new AppUser
                {
                    UserName = request.Username,
                    Email = request.Email,
                    DisplayName = request.DisplayName
                };
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    return new User
                    {
                        Username = user.UserName,
                        DisplayName = user.DisplayName,
                        Email=user.Email,
                        Token = _jwtJenerator.CreateToken(user),
                        Image = null
                    };
                }
                throw new Exception("Proplem Saving Change");
            }
        }
    }
}
