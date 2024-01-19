using BetterCoding.Strapi.SDK.Core.Http;
using BetterCoding.Strapi.SDK.Core.Server;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BetterCoding.Strapi.SDK.Core.Tests
{
    public class UserTests
    {
        [Fact]
        public async Task LogIn_Test()
        {
            var serverConfiguration = new StrapiServerConfiguration()
            {
                APIToken = "aa1f50223e80dcf00912a3c827bbd6012a5065620fafe397c0ba3af464e617876590f5458ba72580709285c48790940c17127c498f3dbd67e7854393cbdff6433110cd44080af25ad9c0564e8483ad4351677998959d959e275e124f544e415c0eeb02430c138dcb891e694641cb3c21720f197403053ea5019728140172f4da",
                ServerURI = "http://localhost:1337",
                Alias = "development",
                IsDefault = true,
            };

            StrapiClient.AddServer(serverConfiguration);

            var client = StrapiClient.GetClient();
            Mock<IWebClient> webClient = new Mock<IWebClient>();
            webClient.Setup(s => s.ExecuteAsync(
                It.IsAny<HttpRequest>(),
                It.IsAny<IProgress<IDataTransferLevel>>(),
                It.IsAny<IProgress<IDataTransferLevel>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(new Tuple<HttpStatusCode, string>(HttpStatusCode.OK, "{\"jwt\":\"test-jwt\",\"user\":{\"id\":1,\"username\":\"tom\",\"email\":\"tom@gmail.com\",\"provider\":\"local\",\"confirmed\":true,\"blocked\":false,\"createdAt\":\"2024-01-16T12:54:22.878Z\",\"updatedAt\":\"2024-01-18T22:30:19.329Z\"}}"));

            client.Services.WebClient = webClient.Object;

            var user = await client
                 .GetAuthBuilder()
                 .Identifier("username")
                 .Password("password")
                 .KeepLoggedIn(true)
                 .LogInAsync();

            Assert.Equal("test-jwt", user.Jwt);
        }
    }
}
