﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using RM.QuickLogOn.OAuth.Models;

namespace RM.QuickLogOn.OAuth.Handlers
{
    [OrchardFeature("RM.QuickLogOn.OAuth.LiveID")]
    public class LiveIDSettingsPartHandler : ContentHandler
    {
        public Localizer T { get; set; }

        public LiveIDSettingsPartHandler(IRepository<LiveIDSettingsPartRecord> repository)
        {
            Filters.Add(new ActivatingFilter<LiveIDSettingsPart>("Site"));
            Filters.Add(StorageFilter.For(repository));
            T = NullLocalizer.Instance;
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("QuickLogOn")) { Id = "QuickLogOn" });
        }
    }
}
