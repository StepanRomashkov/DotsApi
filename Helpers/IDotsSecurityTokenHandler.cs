using DotsApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace DotsApi.Helpers
{
    public interface IDotsSecurityTokenHandler
    {
        string CreateToken(User user);
        SecurityToken ValidateToken(string token);
    }
}
