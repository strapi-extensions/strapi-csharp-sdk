# Log In

## Log in with local auth

```csharp
await StrapiClient.GetClient()
    .GetAuthBuilder()
    .Identifier("your-username")
    .Password("your-password")
    .LogInAsync();
```