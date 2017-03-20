using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Environment.Configuration;
using Orchard.Environment.ShellBuilders.Models;
using Orchard.FileSystems.AppData;
using Orchard.Localization;
using Orchard.Logging;
using RaisingStudio.ModuleGenerator.Services;
using RaisingStudio.ModuleGenerator.TemplateModels;
using RaisingStudio.ModuleGenerator.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Orchard.Mvc.Extensions;
using RaisingStudio.ModuleGenerator.Core;
using RaisingStudio.ModuleGenerator.Interfaces;

namespace RaisingStudio.ModuleGenerator.Controllers
{
    public class AdminController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        public IOrchardServices Services { get { return _orchardServices; } }

        private readonly IContentManager _contentManager;
        private readonly IModuleGeneratorService _moduleGeneratorService;
        private readonly ShellSettings _shellSettings;
        private readonly ShellBlueprint _shellBlueprint;
        private readonly IAppDataFolder _appDataFolder;
        private readonly IZipFileService _zipFileService;

        public AdminController(
            IOrchardServices orchardServices,
            IContentManager contentManager,
            IModuleGeneratorService moduleGeneratorService,
            IZipFileService zipFileService,
            ShellSettings shellSettings,
            ShellBlueprint shellBlueprint,
            IAppDataFolder appDataFolder)
        {
            _orchardServices = orchardServices;
            _contentManager = contentManager;
            _moduleGeneratorService = moduleGeneratorService;
            _zipFileService = zipFileService;
            _shellSettings = shellSettings;
            _shellBlueprint = shellBlueprint;
            _appDataFolder = appDataFolder;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        private IEnumerable<Property> GetContentTypeDefinition(string contentTypeName)
        {
            List<Property> properties = new List<Property>();
            var contentTypeDefinitions = _contentManager.GetContentTypeDefinitions().Where(c => c.Name == contentTypeName);
            foreach (var contentTypeDefinition in contentTypeDefinitions)
            {
                if (contentTypeDefinition.Parts != null)
                {
                    var part = contentTypeDefinition.Parts.SingleOrDefault(p => p.PartDefinition.Name == contentTypeName);
                    if (part != null)
                    {
                        var fields = part.PartDefinition.Fields;
                        if (fields != null)
                        {
                            foreach (var field in fields)
                            {
                                properties.Add(new Property
                                    {
                                        PropertyName = field.Name,
                                        DisplayName = field.DisplayName,
                                        PropertyType = GetPropertyType(field),
                                        Required = GetRequired(field)
                                    });
                            }
                        }
                    }
                }
            }
            return properties;
        }

        private bool GetRequired(ContentPartFieldDefinition field)
        {
            switch (field.FieldDefinition.Name)
            {
                case "BooleanField":
                    {
                        return field.Settings.ContainsKey("BooleanFieldSettings.Optional") ? !Convert.ToBoolean(field.Settings["BooleanFieldSettings.Optional"]) : true;
                    }
                case "DateTimeField":
                    {
                        return field.Settings.ContainsKey("DateTimeFieldSettings.Required") ? Convert.ToBoolean(field.Settings["DateTimeFieldSettings.Required"]) : false;
                    }
                case "NumericField":
                    {
                        return field.Settings.ContainsKey("NumericFieldSettings.Required") ? Convert.ToBoolean(field.Settings["NumericFieldSettings.Required"]) : false;
                    }
                case "InputField":
                    {
                        return field.Settings.ContainsKey("InputFieldSettings.Required") ? Convert.ToBoolean(field.Settings["InputFieldSettings.Required"]) : false;
                    }
                default:
                    { 
                        return false;
                    }
            }
        }

        private string GetPropertyType(ContentPartFieldDefinition field)
        {
            switch (field.FieldDefinition.Name)
            {
                case "BooleanField":
                    {
                        return "bool";
                    }
                case "DateTimeField":
                    {
                        return "DateTime";
                    }
                case "NumericField":
                    {
                        return "decimal";
                    }
                default:
                    { 
                        return "string";
                    }
            }
        }

        public ActionResult Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.GenerateModule, T("Not allowed to generate module")))
                return new HttpUnauthorizedResult();

            var model = new GeneratorViewModel
            {
                ContentTypes = (from c in _contentManager.GetContentTypeDefinitions()
                               select new ContentType
                               {
                                   Name = c.Name,
                                   DisplayName = c.DisplayName
                               }).ToList(),
                ReferenceTemplates = GetTemplates(),
                ModuleTemplatesPathName = ModuleGeneratorService.ModuleTemplatesPathName
            };
            return View(model);
        }

        private IEnumerable<string> GetTemplates()
        {
            string path = Path.Combine("Sites", _shellSettings.Name, ModuleGeneratorService.ModuleTemplatesPathName);
            if(!_appDataFolder.DirectoryExists(path))
            {
                _appDataFolder.CreateDirectory(path);
            }
            return (from d in _appDataFolder.ListDirectories(path)
                   select Path.GetFileName(d)).ToList();
        }

        public ActionResult Generate(GeneratorViewModel model)
        {
            if (!Services.Authorizer.Authorize(Permissions.GenerateModule, T("Not allowed to generate module")))
                return new HttpUnauthorizedResult();

            Module module = new Module
            {
                ModuleName = model.ModuleName,
                Guid = Guid.NewGuid().ToString(),
                Entities = (from c in model.ContentTypes
                           where c.Selected
                           select new Entity
                           {
                               TypeName = c.Name,
                               DisplayName = c.Name,                               
                               Properties = GetContentTypeDefinition(c.Name)
                           }).ToList()
            };
            bool result = _moduleGeneratorService.GenerateModule(model.TemplateName, module);
            ViewBag.Success = result;
            return View(module);
        }


        protected JsonResult Jsonp(object data, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        {
            if (this.IsIE() && this.AcceptText())
            {
                return Json(data, "text/html", behavior);
            }
            else
            {
                return Json(data, behavior);
            }
        }

        private ActionResult SuccessResult()
        {
            string p = Request.Unvalidated()["p"];
            if (p == "json")
            {
                return Jsonp("OK");
            }
            else
            {
                string returnUrl = Request.Unvalidated()["returnUrl"];
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return this.RedirectLocal(returnUrl);
                }
                else
                {
                    return Content("OK");
                }
            }
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (!Services.Authorizer.Authorize(Permissions.InstallTemplate, T("Not allowed to install template")))
                return new HttpUnauthorizedResult();

            if (file != null)
            {
                try
                {
                    string path = Path.Combine("Sites", _shellSettings.Name, ModuleGeneratorService.ModuleTemplatesPathName);
                    if (!_appDataFolder.DirectoryExists(path))
                    {
                        _appDataFolder.CreateDirectory(path);
                    }
                    string fileName = Path.GetFileName(file.FileName);
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        string fullPath = _appDataFolder.MapPath(Path.Combine(path, fileName));
                        file.SaveAs(fullPath);
                        if (_zipFileService.IsZipFile(fullPath))
                        {
                            _zipFileService.ExtractToDirectory(fullPath, _appDataFolder.MapPath(Path.Combine(path, Path.GetFileNameWithoutExtension(fileName))));
                            return SuccessResult();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Upload file failed.");
                }
            }
            return new EmptyResult();
        }
    }
}