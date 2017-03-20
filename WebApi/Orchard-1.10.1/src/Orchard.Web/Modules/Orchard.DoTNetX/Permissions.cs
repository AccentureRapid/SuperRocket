using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Orchard.DoTNetX
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageBlogPost = new Permission { Description = "Manage BlogPost", Name = "ManageBlogPost" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new Permission[] {
                  ManageBlogPost,
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new Permission[] {
                          ManageBlogPost,
                    }
                },
                new PermissionStereotype {
                    Name = "Editor",
                    Permissions = new Permission[] {
                          ManageBlogPost,
                    }
                },
                new PermissionStereotype {
                    Name = "Moderator",
                },
                new PermissionStereotype {
                    Name = "Author",
                    Permissions = new Permission[] {
                          ManageBlogPost,
                    }
                },
                new PermissionStereotype {
                    Name = "Contributor",
                    Permissions = new Permission[] {
                          ManageBlogPost,
                    }
                },
            };
        }
    }
}

