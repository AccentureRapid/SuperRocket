using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using js.Ias.Models;
using js.Ias.Settings;

namespace js.Ias.Handlers
{
    public class InfiniteAjaxScrollingPartHandler : ContentHandler
    {
        public InfiniteAjaxScrollingPartHandler(IRepository<InfiniteAjaxScrollingPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
            OnInitializing<InfiniteAjaxScrollingPart>((context, part) =>
            {
                part.Container = ".content-items";
                part.Item = ".content-item";
                part.Loader = "~/Modules/js.Ias/Styles/images/loader.gif";
                part.NextAnchor = ".zone-content .pager .last a";
                part.Pagination = ".zone-content .pager";
            });
            OnLoaded<InfiniteAjaxScrollingPart>((ctx, part) =>
            {
                var settings = part.Settings.GetModel<InfiniteAjaxScrollingTypePartSettings>();

                try
                {
                    part.UseHistory = part.Record.UseHistory;
                    part.Container = string.IsNullOrWhiteSpace(part.Record.Container) ? settings.Container : part.Record.Container; // has default
                    part.Item = string.IsNullOrWhiteSpace(part.Record.Item) ? settings.Item : part.Record.Item; // has default
                    part.Loader = string.IsNullOrWhiteSpace(part.Record.Loader) ? settings.Loader : part.Record.Loader; // has default
                    part.NextAnchor = string.IsNullOrWhiteSpace(part.Record.NextAnchor) ? settings.NextAnchor : part.Record.NextAnchor; // has default
                    part.Pagination = string.IsNullOrWhiteSpace(part.Record.Pagination) ? settings.Pagination : part.Record.Pagination; // has default
                    part.BeforePageChange = part.Record.BeforePageChange;
                    part.OnLoadItems = part.Record.OnLoadItems;
                    part.OnPageChange = part.Record.OnPageChange;
                    part.OnRenderComplete = part.Record.OnRenderComplete;
                }
                catch { }
            });
        }
    }
}