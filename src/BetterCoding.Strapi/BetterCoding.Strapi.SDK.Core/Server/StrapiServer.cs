using BetterCoding.Strapi.SDK.Core.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCoding.Strapi.SDK.Core.Server
{
    public class StrapiServerConfiguration
    {
        public string ServerURI { get; set; } = string.Empty;
        public string APIToken { get; set; } = string.Empty;

        /// <summary>
        /// key to map server
        /// </summary>
        public string Alias { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
    }

    public class StrapiServer
    {
        public StrapiServerConfiguration ServerConfiguration { get; }
        public StrapiServer(StrapiServerConfiguration serverConfiguration)
        {
            ServerConfiguration = serverConfiguration;
        }
        public StrapiUser ActiveUser { get; internal set; }
    }
}
