using System;
using Orchard.ContentManagement;
using Orchard.Events;
using Dolph.SmartContentPicker.Fields;
using Orchard.Localization;

namespace Dolph.SmartContentPicker.Tokens
{
    public interface ITokenProvider : IEventHandler {
        void Describe(dynamic context);
        void Evaluate(dynamic context);
    }

    public class SmartContentPickerFieldTokens : ITokenProvider {
        private readonly IContentManager _contentManager;

        public SmartContentPickerFieldTokens(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public Localizer T { get; set; }

        public void Describe(dynamic context) {
            context.For("SmartContentPickerField", T("Smart Content Picker Field"), T("Tokens for Smart Content Picker Fields"))
                .Token("Content", T("Content Item"), T("The content item."))
                ;
        }

        public void Evaluate(dynamic context) {
            context.For<SmartContentPickerField>("ContentPickerField")
                .Token("Content", (Func<SmartContentPickerField, object>)(field => field.Ids[0]))
                .Chain("Content", "Content", (Func<SmartContentPickerField, object>)(field => _contentManager.Get(field.Ids[0])))
                ;
        }
    }
}