# Get entry by id

```csharp
var todo = await StrapiClient.GetClient()
    .GetFiltersBuilder()
    .EntryName("todo")
    .PluralApiId("todos")
    .GetAsync(1);
```