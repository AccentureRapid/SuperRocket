using Orchard.UI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.CRM.Project
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            builder.Add().DefineScript("CRMProjectComponents").SetUrl("CRMProjectComponents.js").SetDependencies("reactjs", "reactjs_dom", "BaseComponents", "react-bootstrap");
            builder.Add().DefineScript("ProjectWidgets").SetUrl("ProjectWidgets.js").SetDependencies("jQueryUI", "CRMProjectComponents");
            builder.Add().DefineScript("Chosen").SetUrl("chosen/chosen.jquery.js").SetDependencies("jQuery");
            builder.Add().DefineScript("JsTree").SetUrl("JsTree.js").SetDependencies("jQuery");
        }
    }
}