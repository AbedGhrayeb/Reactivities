using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class List
    {
        public class Query:IRequest<List<Profile>>
        {
            public string Username { get; set; }
            public string Predicate { get; set; }
        }
        public class Handler : IRequestHandler<Query, List<Profile>>
        {
            private readonly DataContext _context;
            private readonly IProfileReader _profileReader;

            public Handler(DataContext context,IProfileReader profileReader)
            {
                _context = context;
                _profileReader = profileReader;
            }

            public async Task<List<Profile>> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = _context.Followings.AsQueryable();

                var userFollowings= new List<UserFollowing>();
                var Profiles= new List<Profile>();

                switch(request.Predicate)
                {
                    case "followings":
                    userFollowings = await queryable.Where(x=> x.Observer.UserName==request.Username)
                        .ToListAsync();

                    foreach (var item in userFollowings)
                    {
                        Profiles.Add(await _profileReader.ReadProfile(item.Target.UserName));
                    }  
                    break;

                    case "followers":
                        userFollowings = await queryable.Where(x=> x.Target.UserName == request.Username)
                            .ToListAsync();  
                        foreach (var item in userFollowings)
                        {
                            Profiles.Add(await _profileReader.ReadProfile(item.Observer.UserName));
                        }
                        break;
                }
                return Profiles;

            }
        }
    }
}