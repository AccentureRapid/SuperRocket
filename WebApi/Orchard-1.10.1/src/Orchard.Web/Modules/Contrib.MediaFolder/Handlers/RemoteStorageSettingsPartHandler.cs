using Contrib.MediaFolder.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;

namespace Contrib.MediaFolder.Handlers {
    public class RemoteStorageSettingsPartHandler : ContentHandler {

        public RemoteStorageSettingsPartHandler(IRepository<RemoteStorageSettingsPartRecord> repository, IWorkContextAccessor workContextAccessor) {
            T = NullLocalizer.Instance;
            Filters.Add(new ActivatingFilter<RemoteStorageSettingsPart>("Site"));
            Filters.Add(StorageFilter.For(repository));
            Filters.Add(new TemplateFilterForRecord<RemoteStorageSettingsPartRecord>("RemoteStorageSettings", "Parts/MediaFolder.RemoteStorageSettings", "media"));

            //OnInitializing<RemoteStorageSettingsPart>(
            //    (context, part) => {
            //        part.Record.MediaLocation =
            //            workContextAccessor.GetContext().HttpContext.Server.MapPath("~/Media");
            //    });
        }

        public Localizer T { get; set; }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Media")));
        }
    }
}