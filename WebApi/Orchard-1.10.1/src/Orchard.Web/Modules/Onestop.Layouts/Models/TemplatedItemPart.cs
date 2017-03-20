using System;
using System.Diagnostics;
using System.Xml.Linq;
using Orchard.ContentManagement;

namespace Onestop.Layouts.Models {
    public class TemplatedItemPart : ContentPart {
        private XDocument _dataDocument;

        public int? LayoutId {
            get { return this.Retrieve(x => x.LayoutId, versioned: true); }
            set { this.Store(x => x.LayoutId, value, versioned: true); }
        }

        public string Data {
            get { return this.Retrieve(x => x.Data, versioned: true); }
            set { this.Store(x => x.Data, value, versioned: true); }
        }

        internal XDocument DataDocument {
            get {
                if (_dataDocument == null) {
                    if (String.IsNullOrWhiteSpace(Data)) {
                        Data = "<data></data>";
                    }
                    _dataDocument = XDocument.Parse(Data);
                }
                return _dataDocument;
            }
        }

        internal XElement RootElement {
            get {
                var rootElement = DataDocument.Element("data");
                Debug.Assert(rootElement != null, "Root element should be data.");
                return rootElement;
            }
        }
    }
}
