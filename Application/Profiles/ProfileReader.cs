using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Errors;
using Application.Interfaces;

namespace Application.Profiles
{
    public class ProfileReader : IProfileReader
    {
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;

        public ProfileReader(DataContext context,IUserAccessor userAccessor)
        {
            _context = context;
             _userAccessor = userAccessor;
        }
        public async Task<Profile> ReadProfile(string username)
        {
            //user who went to show his profile
            var user = await _context.Users.SingleOrDefaultAsync(x=> x.UserName==username);

            if (user==null)
            {
                throw new RestException(HttpStatusCode.NotFound,new{msg="Not Found User"});
            }
            var currentUser= await _context.Users.SingleOrDefaultAsync(x=> 
                x.UserName == _userAccessor.CurrentUsername());
            
            var profile= new Profile{
                 DisplayName = user.DisplayName,
                    Username = user.UserName,
                    Image = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                    Photos = user.Photos,
                    Bio = user.Bio,
                    FollowerCount= user.Followers.Count,
                    FollowingsCount=user.Followings.Count
            };

            if (currentUser.Followings.Any(x=>x.TargetId==user.Id))
            {
                profile.IsFollowed=true;
            }
            return profile;
        }
    }
}