using Orchard.CRM.Core.Models;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.CRM.Core.Drivers
{
    public class CRMCommentDriver : ContentPartDriver<CRMCommentPart>
    {
        protected override DriverResult Display(CRMCommentPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_CRMComment",
                () => shapeHelper.Parts_CRMComment(
                    Model: part
                    ));
        }
    }
}