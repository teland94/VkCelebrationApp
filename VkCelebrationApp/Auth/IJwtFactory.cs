using System.Security.Claims;
using System.Threading.Tasks;

namespace VkCelebrationApp.Auth
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string login, ClaimsIdentity identity);

        ClaimsIdentity GenerateClaimsIdentity(string login, string id);
    }
}
