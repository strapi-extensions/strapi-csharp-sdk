using BetterCoding.Strapi.SDK.Core.Entry;

namespace BetterCoding.Strapi.SDK.Core.User
{
    [StrapiEntryName("user")]
    public class StrapiUser : StrapiEntry
    {
        [StrapiFieldName("jwt")]
        public string Jwt
        {
            get => GetProperty<string>(null, nameof(Jwt));
            set => SetProperty(value, nameof(Jwt));
        }

        [StrapiFieldName("username")]
        public string Username
        {
            get => GetProperty<string>(null, nameof(Username));
            set => SetProperty(value, nameof(Username));
        }
    }
}
