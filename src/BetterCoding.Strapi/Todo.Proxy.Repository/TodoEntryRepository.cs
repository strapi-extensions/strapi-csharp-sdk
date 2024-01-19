using BetterCoding.Strapi.SDK.Core;
using Todo.Proxy.Contracts;

namespace Todo.Proxy.Repository
{
    public interface ITodoEntryRepository
    {
        Task<List<TodoEntry>> GetByWhoCreated(int userId);
        Task<TodoEntry> GetById(int id);
    }

    public class TodoEntryRepository: ITodoEntryRepository
    {
        public async Task<List<TodoEntry>> GetByWhoCreated(int userId)
        {
            var todos = await StrapiClient.GetClient()
                .GetFiltersBuilder()
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

        public async Task<TodoEntry> GetById(int id)
        {
            var todo = await StrapiClient.GetClient()
                .GetFiltersBuilder()
                .EntryName("todo")
                .PluralApiId("todos")
                .GetAsync(1);

            return new TodoEntry
            {
                Id = todo.Id,
                Due = todo.Get<DateTime>("due"),
                Title = todo.Get<string>("title"),
                Completed = todo.Get<bool>("completed"),
            };
        }
    }
}
