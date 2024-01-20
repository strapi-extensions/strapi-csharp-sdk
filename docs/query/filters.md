# Filter entries

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