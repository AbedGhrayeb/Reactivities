using System.Collections.Generic;
using Domain;
using Newtonsoft.Json;

namespace Application.Profiles
{
    public class Profile
    {
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Image { get; set; }
        public string Bio { get; set; }   
        public int FollowingsCount { get; set; }
        public int FollowerCount { get; set; }
        [JsonProperty("following")]
        public bool IsFollowed { get; set; }
        public ICollection<Photo> Photos { get; set; }
    }
}