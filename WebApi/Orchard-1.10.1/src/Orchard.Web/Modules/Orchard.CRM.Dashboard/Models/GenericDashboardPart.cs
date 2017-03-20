using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Core.Containers.Models;

namespace Orchard.CRM.Dashboard.Models
{
    public class GenericDashboardPart : ContentPart<ContainerPartRecord>
    {
        /// <summary>
        /// Comma-seperated list of the Portlet types for the given dashboard
        /// </summary>
        public string PortletList
        {
            get { return this.Retrieve(x => x.PortletList); }
            set { this.Store(x => x.PortletList, value); }
        }

        /// <summary>
        /// If true, then it will create portlets automatically from portletList above
        /// </summary>
        public bool CreatePortletsOnPublishing
        {
            get { return this.Retrieve(x => x.CreatePortletsOnPublishing); }
            set { this.Store(x => x.CreatePortletsOnPublishing, value); }
        }
    }
}