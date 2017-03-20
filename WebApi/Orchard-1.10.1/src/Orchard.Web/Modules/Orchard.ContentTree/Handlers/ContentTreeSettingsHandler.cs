using Orchard.ContentManagement.Handlers;
using Orchard.ContentTree.Models;
using Orchard.Data;

namespace Spa.Performance.Handlers
{
    public class ContentTreeSettingsHandler : ContentHandler
    {
        public ContentTreeSettingsHandler(IRepository<ContentTreeSettingsRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
            Filters.Add(new ActivatingFilter<ContentTreeSettingsPart>("ContentTreeSettings"));
        }
    }
}