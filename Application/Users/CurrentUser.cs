using System.Linq;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users
{
    public class CurrentUser
    {
        public class Query : IRequest<User> { }
        public class Handler : IRequestHandler<Query, User>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IUserAccessor _userAccessor;
            private readonly IJwtJenerator _jwtJenerator;

            public Handler(UserManager<AppUser> userManager,IUserAccessor userAccessor,IJwtJenerator jwtJenerator)
            {
                _userManager = userManager;
                _userAccessor = userAccessor;
                _jwtJenerator = jwtJenerator;
            }
            public async Task<User> Handle(Query request, CancellationToken cancellationToken)
            {
                var user =await _userManager.FindByNameAsync(_userAccessor.CurrentUsername());
                return new User
                {
                    Username = user.UserName,
                    DisplayName = user.DisplayName,
                    Email=user.Email,
                    Token=_jwtJenerator.CreateToken(user),
                    Image = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url
                };
            }
        }
    }
}
