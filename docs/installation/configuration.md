## Config Strapi Servers - Server Side

> the following section shows how config your servers in your server side .NET project, something like ASP.NET Core Web API, it is not recommended to configure the api token directly on the client.


```csharp
var serverConfiguration = new StrapiServerConfiguration()
{
    APIToken = "your api token for this server",
    ServerURI = "http://localhost:1337",
    Alias = "development",
    IsDefault = true,
};

StrapiClient.AddServer(serverConfiguration);
```

if you want to use multiple servers you can config them like this

```csharp

var development = new StrapiServerConfiguration()
{
    APIToken = "your api token for this server",
    ServerURI = "http://localhost:1337",
    Alias = "development",
    IsDefault = true,
};

var stage = new StrapiServerConfiguration()
{
    APIToken = "your api token for this server",
    ServerURI = "http://your-stage-server.com",
    Alias = "stage",
    IsDefault = false,
};

var production = new StrapiServerConfiguration()
{
    APIToken = "your api token for this server",
    ServerURI = "http://your-production-server.com",
    Alias = "production",
    IsDefault = false,
};

StrapiClient.AddServers(development, stage, production);
```