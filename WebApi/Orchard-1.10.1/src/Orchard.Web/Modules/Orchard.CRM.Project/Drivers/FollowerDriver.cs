using Orchard.ContentManagement.Drivers;
using Orchard.CRM.Project.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Orchard.CRM.Project.Drivers
{
    public class FollowerDriver : ContentPartDriver<FollowerPart>
    {
        private readonly IOrchardServices services;

        public FollowerDriver(IOrchardServices services)
        {
            this.services = services;
        }

        protected override DriverResult Display(FollowerPart part, string displayType, dynamic shapeHelper)
        {
            if (displayType != "Detail" || services.WorkContext.CurrentUser == null)
            {
                return null;
            }

            string currentUserId = services.WorkContext.CurrentUser.Id.ToString(CultureInfo.InvariantCulture);

            string followers = part.Followers;
            followers = followers ?? string.Empty;

            bool followed = followers.Contains(currentUserId);

            dynamic model = new ExpandoObject();
            model.ContentItemId = part.ContentItem.Id;
            model.Followed = followed;

            return ContentShape("Parts_Follow_Link",   () => shapeHelper.FollowLink(contentItem: part.ContentItem));
        }
    }
}