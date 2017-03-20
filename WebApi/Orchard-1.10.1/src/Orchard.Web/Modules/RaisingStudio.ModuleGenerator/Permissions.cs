using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace RaisingStudio.ModuleGenerator
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission GenerateModule = new Permission { Description = "Generate module", Name = "GenerateModule" };
        public static readonly Permission InstallTemplate = new Permission { Description = "Install template", Name = "InstallTemplate" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                GenerateModule,
                InstallTemplate
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] { GenerateModule, InstallTemplate }
                }
            };
        }
    }
}


