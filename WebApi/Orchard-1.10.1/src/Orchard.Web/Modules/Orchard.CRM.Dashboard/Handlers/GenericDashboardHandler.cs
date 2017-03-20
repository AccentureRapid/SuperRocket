using Orchard.ContentManagement.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.CRM.Core.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.CRM.Dashboard.Models;
using Orchard.Core.Containers.Models;

namespace Orchard.CRM.Dashboard.Handlers
{
    public class GenericDashboardHandler : ContentHandler
    {
        public GenericDashboardHandler(IContentManager contentManager)
        {
            OnPublished<GenericDashboardPart>((context, part) => {

                if (!part.CreatePortletsOnPublishing)
                {
                    return;
                }

                // set default portlets for DashboardWidget
                if (part.ContentItem.ContentType == Consts.GenericDashboardWidgetContentType && string.IsNullOrEmpty(part.PortletList))
                {
                    part.PortletList = string.Join(",", (new string[] { Consts.SmtpPortletContentType, Consts.IMAPTPortletContentType }));
                }

                // portlet list
                var portlets = string.IsNullOrEmpty(part.PortletList) ? new string[] { } : part.PortletList.Split(',');

                // current portlets
                var currentPortlets = contentManager.Query().Where<CommonPartRecord>(c => c.Container.Id == part.ContentItem.Id).List();

                // add new portlets
                int position = -1;
                foreach (var newPortlet in portlets.Where(c => !currentPortlets.Any(d => d.ContentType == c)))
                {
                    position++;
                    var newPortletContentItem = contentManager.Create(newPortlet, VersionOptions.Draft);
                    ContainablePart containablePart = newPortletContentItem.As<ContainablePart>();
                    if (containablePart == null)
                    {
                        continue;
                    }

                    // Position
                    containablePart.Position = position;

                    // Container
                    var newPortletCommon = newPortletContentItem.As<CommonPart>();
                    newPortletCommon.Container = part.ContentItem;

                    contentManager.Publish(newPortletContentItem);
                }

                // delete portlets
                foreach (var portlet in currentPortlets.Where(c => !portlets.Any(d => d == c.ContentType)))
                {
                    contentManager.Remove(portlet);
                }
            });
        }
    }
}