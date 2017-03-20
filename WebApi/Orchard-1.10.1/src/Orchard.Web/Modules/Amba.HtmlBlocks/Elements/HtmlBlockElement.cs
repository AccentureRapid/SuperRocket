using Orchard.Layouts.Elements;
using Orchard.Localization;

namespace Amba.HtmlBlocks.Elements 
{
    public class HtmlBlockElement : ContentElement 
    {
        public override string ToolboxIcon
        {
            get { return "\uf0f6"; }
        }

        public override LocalizedString DisplayText
        {
            get { return T("Html Block"); }
        }
    }

}