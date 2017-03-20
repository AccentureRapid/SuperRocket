using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.UI.Resources;
using Orchard.Environment.Extensions;


namespace Datwendo.Localization
{
    [OrchardFeature("Datwendo.Localization")]
    public class LocResourceManifest : IResourceManifestProvider 
    {
        public void BuildManifests(ResourceManifestBuilder builder) 
        {
            var manifest = builder.Add();
            manifest.DefineScript("PickerSelector").SetDependencies("jQuery").SetUrl("PickerSelector.js");
            manifest.DefineScript("AutorouteComplete").SetDependencies("jQuery").SetUrl("AutorouteComplete.js");
            manifest.DefineStyle("DtwLocalization").SetUrl("datwendo-localization-base.css");
            manifest.DefineStyle("DtwLocalizationAdmin").SetUrl("datwendo-localization-admin.css");
        }
    }
}