using Amba.HtmlBlocks.Elements;
using Orchard.Environment.Extensions;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Framework.Drivers;
using Orchard.Layouts.Helpers;
using Orchard.Layouts.Services;
using Orchard.Layouts.ViewModels;

namespace Amba.HtmlBlocks.Drivers {


    public class HtmlBlockElementDriver : ElementDriver<HtmlBlockElement> {
        private readonly IElementFilterProcessor _processor;
        public HtmlBlockElementDriver(IElementFilterProcessor processor)
        {
            _processor = processor;
        }

        protected override EditorResult OnBuildEditor(HtmlBlockElement element, ElementEditorContext context)
        {
            var viewModel = new Orchard.Layouts.ViewModels.MarkdownEditorViewModel
            {
                Text = element.Content
            };
            var editor = context.ShapeFactory.EditorTemplate(TemplateName: "Elements.HtmlBlockElement", Model: viewModel);

            if (context.Updater != null) {
                context.Updater.TryUpdateModel(viewModel, context.Prefix, null, null);
                element.Content = viewModel.Text;
            }
            
            return Editor(context, editor);
        }

        protected override void OnDisplaying(HtmlBlockElement element, ElementDisplayingContext context)
        {            
            context.ElementShape.HTML = element.Content;
        }
        
    }
}