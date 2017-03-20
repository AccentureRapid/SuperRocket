using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using System.Collections.Generic;
namespace js.Ias.Settings
{
    public class InfiniteAjaxScrollingTypePartSettings
    {
        private string _container;
        private bool _useHistory;
        private string _item;
        private string _pagination;
        private string _nextAnchor;
        private string _loader;
        private string _onPageChange;
        private string _beforePageChange;
        private string _onLoadItems;
        private string _onRenderComplete;

        public string OnRenderComplete
        {
            get
            {
                return this._onRenderComplete;
            }
            set
            {
                this._onRenderComplete = value;
            }
        }

        public string OnLoadItems
        {
            get
            {
                return this._onLoadItems;
            }
            set
            {
                this._onLoadItems = value;
            }
        }

        public string BeforePageChange
        {
            get
            {
                return this._beforePageChange;
            }
            set
            {
                this._beforePageChange = value;
            }
        }

        public string OnPageChange
        {
            get
            {
                return this._onPageChange;
            }
            set
            {
                this._onPageChange = value;
            }
        }

        public string Loader
        {
            get
            {
                return string.IsNullOrWhiteSpace(_loader) ? "~/Modules/js.Ias/Styles/images/loader.gif" : _loader;
            }
            set
            {
                this._loader = value;
            }
        }

        public string NextAnchor
        {
            get
            {
                return string.IsNullOrWhiteSpace(_nextAnchor) ? ".zone-content .pager .last a" : _nextAnchor;
            }
            set
            {
                this._nextAnchor = value;
            }
        }

        public string Pagination
        {
            get
            {
                return string.IsNullOrWhiteSpace(_pagination) ? ".zone-content .pager" : _pagination;
            }
            set
            {
                this._pagination = value;
            }
        }

        public string Item
        {
            get
            {
                return string.IsNullOrWhiteSpace(_item) ? ".content-item" : _item;
            }
            set
            {
                this._item = value;
            }
        }

        public string Container
        {
            get
            {
                return string.IsNullOrWhiteSpace(_container) ? ".content-items" : _container;
            }
            set
            {
                this._container = value;
            }
        }

        public bool UseHistory
        {
            get
            {
                return this._useHistory;
            }
            set
            {
                this._useHistory = value;
            }
        }

    }

    public class InfiniteAjaxScrollingTypePartSettingsHooks : ContentDefinitionEditorEventsBase
    {
        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition)
        {
            if (definition.PartDefinition.Name != "InfiniteAjaxScrollingPart")
                yield break;
            var model = definition.Settings.GetModel<InfiniteAjaxScrollingTypePartSettings>();

            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel)
        {
            if (builder.Name != "InfiniteAjaxScrollingPart")
                yield break;

            var model = new InfiniteAjaxScrollingTypePartSettings();
            updateModel.TryUpdateModel(model, "InfiniteAjaxScrollingTypePartSettings", null, null);
            builder.WithSetting("InfiniteAjaxScrollingTypePartSettings.UseHistory", model.UseHistory.ToString());
            builder.WithSetting("InfiniteAjaxScrollingTypePartSettings.Container", model.Container.ToString());
            builder.WithSetting("InfiniteAjaxScrollingTypePartSettings.Pagination", model.Pagination.ToString());
            builder.WithSetting("InfiniteAjaxScrollingTypePartSettings.Item", model.Item.ToString());
            builder.WithSetting("InfiniteAjaxScrollingTypePartSettings.NextAnchor", model.NextAnchor.ToString());
            builder.WithSetting("InfiniteAjaxScrollingTypePartSettings.Loader", model.Loader.ToString());
            builder.WithSetting("InfiniteAjaxScrollingTypePartSettings.OnLoadItems", string.IsNullOrWhiteSpace(model.OnLoadItems) ? string.Empty : model.OnLoadItems.ToString());
            builder.WithSetting("InfiniteAjaxScrollingTypePartSettings.BeforePageChange", string.IsNullOrWhiteSpace(model.BeforePageChange) ? string.Empty : model.BeforePageChange.ToString());
            builder.WithSetting("InfiniteAjaxScrollingTypePartSettings.OnPageChange", string.IsNullOrWhiteSpace(model.OnPageChange) ? string.Empty : model.OnPageChange.ToString());
            builder.WithSetting("InfiniteAjaxScrollingTypePartSettings.OnRenderComplete", string.IsNullOrWhiteSpace(model.OnRenderComplete) ? string.Empty : model.OnRenderComplete.ToString()); 
            yield return DefinitionTemplate(model);
        }

    }
}