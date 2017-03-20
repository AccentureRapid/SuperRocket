using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using Onestop.Layouts.Helpers;
using Orchard.ContentManagement;

namespace Onestop.Layouts.Models {
    public class LayoutTemplatePart : ContentPart {
        private XDocument _descriptionDocument;

        public string LayoutDescription {
            get {
                var value = this.Retrieve(x => x.LayoutDescription, versioned: true);
                if (String.IsNullOrWhiteSpace(value)) {
                    value = "<layout></layout>";
                    LayoutDescription = value;
                }
                return value;
            }
            set {
                _descriptionDocument = null;
                this.Store(x => x.LayoutDescription, value, versioned: true);
            }
        }

        public void Update() {
            LayoutDescription = _descriptionDocument.ToString(SaveOptions.None);
        }

        public XDocument LayoutDescriptionDocument {
            get {
                if (_descriptionDocument == null) {
                    try {
                        _descriptionDocument = XDocument.Parse(LayoutDescription);
                    }
                    catch (XmlException) {
                        return XDocument.Parse("<layout></layout>");
                    }
                }
                return _descriptionDocument;
            }
        }

        public XElement RootElement {
            get {
                var rootElement = LayoutDescriptionDocument.Element("layout");
                Debug.Assert(rootElement != null, "Root element should be layout.");
                return rootElement;
            }
        }

        public int Width {
            get { return RootElement.AttrInt("width"); }
        }

        public int Height {
            get { return RootElement.AttrInt("height"); }
        }

        public string CssClass {
            get { return XmlHelpers.Attr(RootElement, "class"); }
        }

        public string PageCssClass {
            get { return XmlHelpers.Attr(RootElement, "pageclass"); }
        }

        public int? ParentLayoutId {
            get { return this.Retrieve(x => x.ParentLayoutId, versioned: true); }
            set { this.Store(x => x.ParentLayoutId, value, versioned: true); }
        }

        public string StylesheetPath { 
            get { return XmlHelpers.Attr(RootElement, "stylesheet"); }
        }
    }
}
