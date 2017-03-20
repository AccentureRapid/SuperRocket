using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentTypes.Services;
using Orchard.Core.Contents.Extensions;
using Orchard.Core.Contents.Settings;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;
using Orchard.UI.Admin;

namespace Amba.AnonymousViewPermissionsEditor.Controllers
{
    [Admin]
    public class AdminController : Controller
    {
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IRoleService _roleService;

        public AdminController(IContentManager contentManager, IContentDefinitionService contentDefinitionService, IAuthorizationService authorizationService, IContentDefinitionManager contentDefinitionManager, IRoleService roleService)
        {
            _contentManager = contentManager;
            _contentDefinitionService = contentDefinitionService;
            _authorizationService = authorizationService;
            _contentDefinitionManager = contentDefinitionManager;
            _roleService = roleService;
        }

        // GET: Admin
        public ActionResult AnonymousViewEditor()
        {
            var types = _contentDefinitionService.GetTypes();
            var permissions = _roleService.GetInstalledPermissions();
            var simulation = UserSimulation.Create("Anonymous");
            var effectivePermissions = permissions
                .SelectMany(group => group.Value)
                .Where(permission => _authorizationService.TryCheckAccess(permission, simulation, null))
                .Select(permission => permission.Name)
                .Distinct()
                .ToList();

            var viewModel = new AnonymousViewEditorViewModel();
            foreach (var type in types)
            {
                viewModel.TypesSecuritySettings.Add(new TypeSecuritySettings()
                {
                    TypeName = type.Name,
                    IsSecurable = type.Settings.GetModel<ContentTypeSettings>().Securable,
                    Permission = "View_" + type.Name,
                    CanView = effectivePermissions.Contains("View_" + type.Name)
                });
            }
            viewModel.AllowViewAllContent = effectivePermissions.Contains("ViewContent");
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SaveAnonymousViewEditor()
        {
            var role = _roleService.GetRoleByName("Anonymous");
            var types = _contentDefinitionService.GetTypes();
            var permissions = _roleService.GetInstalledPermissions();
            var simulation = UserSimulation.Create("Anonymous");
            var effectivePermissions = permissions
                .SelectMany(group => group.Value)
                .Where(permission => _authorizationService.TryCheckAccess(permission, simulation, null))
                .Select(permission => permission.Name)
                .Distinct()
                .ToList();

            var viewContentChanged = false;
            if (Request.Form["AllowViewAllContent"] != "true" && effectivePermissions.Contains("ViewContent"))
            {
                effectivePermissions.Remove("ViewContent");
                viewContentChanged = true;
            }
            else if (Request.Form["AllowViewAllContent"] == "true" && !effectivePermissions.Contains("ViewContent"))
            {
                effectivePermissions.Add("ViewContent");
                viewContentChanged = true;
            }
            if (viewContentChanged)
            {
                _roleService.UpdateRole(role.Id, role.Name, effectivePermissions);
                return RedirectToAction("AnonymousViewEditor");
            }
            
            foreach (var type in types)
            {
                var permissionName = "View_" + type.Name;
                if (Request.Form[type.Name] != null)
                {
                    var isSecurable = Request.Form[type.Name] == "true";
                    var isSecurableOrigin = type.Settings.GetModel<ContentTypeSettings>().Securable;
                    if (isSecurable != isSecurableOrigin)
                    {
                        _contentDefinitionManager.AlterTypeDefinition(type.Name, x => x.Securable(isSecurable));
                        if (!isSecurable)
                        {
                            effectivePermissions.Remove(permissionName);
                            continue;
                        }
                    }
                }
                
                if (Request.Form[permissionName] != null)
                {
                    var canView = Request.Form[permissionName] == "true";
                    if (effectivePermissions.Contains(permissionName) && !canView)
                    {
                        effectivePermissions.Remove(permissionName);
                    }
                    else if (!effectivePermissions.Contains(permissionName) && canView)
                    {
                        effectivePermissions.Add(permissionName);
                    }                    
                }

                
                _roleService.UpdateRole(role.Id, role.Name, effectivePermissions);
            }
            foreach (string key in Request.Form.Keys) {                
                if (key.StartsWith("View_")) {
                    
                   //rolePermissions.Add(permissionName);
                }
            }
            return RedirectToAction("AnonymousViewEditor");
        }
    }

    public class AnonymousViewEditorViewModel
    {
        public bool AllowViewAllContent { get; set; }
        public AnonymousViewEditorViewModel()
        {
            TypesSecuritySettings = new List<TypeSecuritySettings>();
        }
        public List<TypeSecuritySettings> TypesSecuritySettings { get; set; }
    }

    public class TypeSecuritySettings
    {
        public string TypeName { get; set; }
        public bool IsSecurable { get; set; }
        public string Permission { get; set; }
        public bool CanView { get; set; }
    } 
}