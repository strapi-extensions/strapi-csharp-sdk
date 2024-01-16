using BetterCoding.Strapi.SDK.Core.Entry;

namespace BetterCoding.Strapi.SDK.Core.Services
{
    public interface IEntryController
    {
        StrapiEntry Create(string entryName, IEntryState serverState);
    }

    public class EntryController : IEntryController
    {
        public StrapiEntry Create(string entryName, IEntryState serverState)
        {
            var entry = new StrapiEntry(entryName);
            entry.HandleFetchResult(serverState);
            return entry;
        }
    }
}
