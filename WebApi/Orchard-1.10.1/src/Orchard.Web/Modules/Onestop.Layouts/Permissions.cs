using System.Collections.Generic;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Onestop.Layouts {
    [OrchardFeature("Onestop.Layouts")]
    public class Permissions : IPermissionProvider {
        public static readonly Permission ManageLayouts = new Permission { Description = "Manage Layouts", Name = "ManageLayouts" };
        public static readonly Permission ManageTemplates = new Permission { Description = "Manage Templates", Name = "ManageTemplates" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new[] {
                ManageLayouts,
                ManageTemplates
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] { ManageLayouts, ManageTemplates }
                }
            };
        }
    }
}