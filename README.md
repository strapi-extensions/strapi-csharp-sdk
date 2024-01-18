# Strapi CSharp SDK
![publish status on main](https://github.com/strapi-extensions/strapi-csharp-sdk/actions/workflows/nuget-publish.yml/badge.svg)
[![Nuget][nuget-svg]][nuget-link]
[![License][license-svg]][license-link]


## Installation

> dotnet add package BetterCoding.Strapi.SDK.Core 

## Usage


### Config servers

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
    Alias = "stage",
    IsDefault = false,
};

StrapiClient.AddServers(development, stage, production);
```


### Query


```csharp
var todos = await StrapiClient.GetClient()
    .GetQueryBuilder()
    .EntryName("todo")
    .PluralApiId("todos")
    .DeepEqualsTo("whoCreated", "id", userId)
    .FindAsync();

//model mapping
return todos.Select(t => new TodoEntry
{
    Id = t.Id,
    Due = t.Get<DateTime>("due"),
    Title = t.Get<string>("title"),
    Completed = t.Get<bool>("completed"),
}).ToList();
```

if you are using multiple-servers mode, you can switch server when calling GetClient by passing the alias of server:

```csharp
var todos = await StrapiClient.GetClient("stage")
    .GetQueryBuilder()
    .EntryName("todo")
    .PluralApiId("todos")
    .DeepEqualsTo("whoCreated", "id", userId)
    .FindAsync();
```

 [license-svg]: https://img.shields.io/badge/license-BSD-lightgrey.svg
 [license-link]: https://github.com/strapi-extensions/strapi-csharp-sdk/blob/main/LICENSE
 [nuget-svg]: https://img.shields.io/nuget/v/BetterCoding.Strapi.SDK.Core
 [nuget-link]: https://www.nuget.org/packages/BetterCoding.Strapi.SDK.Core
 [github-repo]: https://github.com/strapi-extensions/strapi-csharp-sdk 