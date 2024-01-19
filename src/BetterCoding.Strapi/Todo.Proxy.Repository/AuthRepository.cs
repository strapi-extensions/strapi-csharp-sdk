using BetterCoding.Strapi.SDK.Core;
using Todo.Proxy.Contracts.API;

namespace Todo.Proxy.Repository
{
    public interface IAuthRepository 
    {
        Task LogInAsync(LogInRequest request);
    }

    public class AuthRepository: IAuthRepository
    {
        public async Task LogInAsync(LogInRequest request) 
        {
            await StrapiClient.GetClient()
                .GetAuthBuilder()
                .Identifier(request.Username)
                .Password(request.Password)
                .LogInAsync();
        }
    }
}
