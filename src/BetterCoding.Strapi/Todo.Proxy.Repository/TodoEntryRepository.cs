using BetterCoding.Strapi.SDK.Core;
using Todo.Proxy.Contracts;

namespace Todo.Proxy.Repository
{
    public class TodoEntryRepository
    {
        public async Task<List<TodoEntry>> GetByWhoCreated(int userId)
        {
            var todos = await StrapiClient.GetClient()
                .GetQueryBuilder()
                .EntryName("todo")
                .PluralApiId("todos")
                .DeepEqualsTo("whoCreated", "id", userId)
                .FindAsync();

            return todos.Select(t => new TodoEntry
            {
                Id = t.Id,
                Due = t.Get<DateTime>("due"),
                Title = t.Get<string>("title"),
                Completed = t.Get<bool>("completed"),
            }).ToList();
        }
    }
}
