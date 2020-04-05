using Domain;

namespace Application.Interfaces
{
    public interface IJwtJenerator
    {
         string CreateToken(AppUser user);

    }
}
