using System;
using System.Text;
//using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using TheMonarch.SyntaxHighlighter.Models;

namespace TheMonarch.SyntaxHighlighter.Handlers {
    //[UsedImplicitly]
    public class SyntaxHighlighterSettingsPartHandler : ContentHandler {
        private readonly IEncryptionService _encryptionService;

        public SyntaxHighlighterSettingsPartHandler(IRepository<SyntaxHighlighterSettingsPartRecord> repository, IEncryptionService encryptionService) {
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;

            _encryptionService = encryptionService;
            Filters.Add(new ActivatingFilter<SyntaxHighlighterSettingsPart>("Site"));
            Filters.Add(StorageFilter.For(repository));

        }

        public new ILogger Logger { get; set; }


        public Localizer T { get; set; }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Syntax-Highlighter")));
        }
    }
}