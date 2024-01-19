using BetterCoding.Strapi.SDK.Core.Services;
using BetterCoding.Strapi.SDK.Core.Services.Extensions;
using BetterCoding.Strapi.SDK.Core.Utilities;

namespace BetterCoding.Strapi.SDK.Core.User
{
    public class AuthBuilder
    {
        public IServiceHub Services { get; internal set; }
        public AuthBuilder(IServiceHub serviceHub = default)
        {
            Services = serviceHub;
        }

        protected string _provider = "local";
        protected string _identifier;
        protected string _password;
        protected bool _active = false;

        public virtual AuthBuilder Identifier(string identifier)
        {
            _identifier = identifier;
            return this;
        }

        public virtual AuthBuilder Provider(string provider)
        {
            _provider = provider;
            return this;
        }

        public virtual AuthBuilder Password(string password)
        {
            _password = password;
            return this;
        }
        public virtual AuthBuilder KeepLoggedIn(bool active = true) 
        {
            _active = active;
            return this;
        }

        public async Task<StrapiUser> LogInAsync()
        {
            var authData = new Dictionary<string, object>
            {
                { "identifier", _identifier },
                { "password", _password },
            };
            var serverState = await Services.StrapiREST.ExecuteAsync($"api/auth/{_provider}", "POST", authData).OnSuccess(task =>
            task.Result is Dictionary<string, object> item && item != null ? Services.LocalUserStateCoder.Decode(item, Services.DataDecoder, Services) : null);
            if (serverState == null) throw new EntryPointNotFoundException();
            var user = Services.GenerateObjectFromState<StrapiUser>(serverState, "user");
            if (_active) 
            {
                Services.Server.ActiveUser = user;
            }
            return user;
        }
    }
}
