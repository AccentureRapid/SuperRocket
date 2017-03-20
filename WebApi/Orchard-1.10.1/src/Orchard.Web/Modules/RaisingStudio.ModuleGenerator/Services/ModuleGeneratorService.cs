using Newtonsoft.Json;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Configuration;
using Orchard.Environment.ShellBuilders.Models;
using Orchard.FileSystems.AppData;
using Orchard.Logging;
using RaisingStudio.ModuleGenerator.Interfaces;
using RaisingStudio.ModuleGenerator.Models;
using RaisingStudio.ModuleGenerator.TemplateModels;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace RaisingStudio.ModuleGenerator.Services
{
    public class ModuleGeneratorService : IModuleGeneratorService
    {
        public const string ModuleTemplatesPathName = "ModuleTemplates";
        public const string TEMPLATECONFIG_FILENAME = "config.json";

        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;
        private readonly ShellSettings _shellSettings;
        private readonly ShellBlueprint _shellBlueprint;
        private readonly IAppDataFolder _appDataFolder;

        public ModuleGeneratorService(
            IOrchardServices orchardServices,
            IContentManager contentManager,
            ShellSettings shellSettings,
            ShellBlueprint shellBlueprint,
            IAppDataFolder appDataFolder)
        {
            _orchardServices = orchardServices;
            _contentManager = contentManager;
            _shellSettings = shellSettings;
            _shellBlueprint = shellBlueprint;
            _appDataFolder = appDataFolder;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }


        private void CreateSubDirectory(string modulePath, string folder)
        {
            Directory.CreateDirectory(Path.Combine(modulePath, folder));
        }

        private static void EnsureDirectory(string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
        }

        private void CopyFile(string templatePath, string modulePath, string fileName)
        {
            string filePath = Path.Combine(modulePath, fileName);
            EnsureDirectory(filePath);
            File.Copy(Path.Combine(templatePath, fileName), filePath, true);
        }

        private void CopyFile(string templatePath, string templateFileName, string modulePath, string fileName)
        {
            string filePath = Path.Combine(modulePath, fileName);
            EnsureDirectory(filePath);
            File.Copy(Path.Combine(templatePath, templateFileName), filePath, true);
        }

        private static string ReadTemplateFile(string templatePath, string fileName)
        {
            return File.ReadAllText(Path.Combine(templatePath, fileName));
        }

        private static void WriteModuleFile(string modulePath, string fileName, string content)
        {
            string filePath = Path.Combine(modulePath, fileName);
            EnsureDirectory(filePath);
            File.WriteAllText(filePath, content);
        }

        public static string GetParameterName(string typeName)
        {
            var parameterName = char.ToLower(typeName[0]) + typeName.Substring(1);
            return parameterName;
        }

        public static string GetIndentityName(string typeName)
        {
            return typeName.ToLower();
        }

        public bool GenerateModule(string templateName, TemplateModels.Module module)
        {
            if (!string.IsNullOrWhiteSpace(templateName))
            {
                string templatePath = _appDataFolder.MapPath(Path.Combine("Sites", _shellSettings.Name, ModuleTemplatesPathName, templateName));
                string templateConfigFileName = Path.Combine(templatePath, TEMPLATECONFIG_FILENAME);
                if (File.Exists(templateConfigFileName))
                {
                    string templateConfigContent = File.ReadAllText(templateConfigFileName);
                    if (!string.IsNullOrEmpty(templateConfigContent))
                    {
                        TemplateConfig templateConfig = JsonConvert.DeserializeObject<TemplateConfig>(templateConfigContent);
                        if (templateConfig != null)
                        {
                            string moduleName = module.ModuleName;
                            if (!string.IsNullOrWhiteSpace(moduleName))
                            {
                                if (module.Settings == null)
                                {
                                    module.Settings = new Dictionary<string, string>();
                                }
                                foreach (var templateSetting in templateConfig.Settings)
                                {
                                    if (!module.Settings.ContainsKey(templateSetting.Name))
                                    {
                                        module.Settings.Add(templateSetting.Name, templateSetting.DefaultValue);
                                    }
                                }
                                string modulePath = HostingEnvironment.MapPath("~/Modules/" + moduleName);

                                Directory.CreateDirectory(modulePath);

                                var model = new Model { Module = module };
                           
                                #region Folders
                                string[] folders = templateConfig.Folders;
                                if (folders != null && folders.Length > 0)
                                {
                                    foreach (var folder in folders)
                                    {
                                        CreateSubDirectory(modulePath, folder);
                                    }
                                }
                                #endregion

                                #region Files
                                string[] files = templateConfig.Files;
                                if (files != null && files.Length > 0)
                                {
                                    foreach (var file in files)
                                    {
                                        CopyFile(templatePath, modulePath, file);
                                    }
                                }
                                #endregion

                                #region Module
                                foreach (var moduleTemplateFile in templateConfig.Module)
                                {
                                    if (!moduleTemplateFile.IsStatic)
                                    {
                                        string moduleTemplateContent = ReadTemplateFile(templatePath, moduleTemplateFile.Template);
                                        string moduleOutputContent = Razor.Parse(moduleTemplateContent, model);
                                        WriteModuleFile(modulePath, string.Format(moduleTemplateFile.Output, moduleName), moduleOutputContent);
                                    }
                                    else
                                    {
                                        CopyFile(templatePath, moduleTemplateFile.Template, string.Format(moduleTemplateFile.Output, moduleName));
                                    }
                                }
                                #endregion

                                #region Entity
                                var entityTemplates = templateConfig.Entity.Select(templateFile =>
                                    {
                                        if (!templateFile.IsStatic)
                                        {
                                            templateFile.Content = ReadTemplateFile(templatePath, templateFile.Template);
                                        }
                                        return templateFile;
                                    }).ToList();
                                foreach (var entity in model.Module.Entities)
                                {
                                    entity.Module = module;
                                    entity.ParameterName = GetParameterName(entity.TypeName);
                                    entity.IndentityName = GetIndentityName(entity.TypeName);

                                    entityTemplates.ForEach(entityTemplate =>
                                        {
                                            if (!entityTemplate.IsStatic)
                                            {
                                                string entityOutputContent = Razor.Parse(entityTemplate.Content, entity);
                                                WriteModuleFile(modulePath, string.Format(entityTemplate.Output, moduleName, entity.TypeName), entityOutputContent);
                                            }
                                            else
                                            {
                                                CopyFile(templatePath, entityTemplate.Template, modulePath, string.Format(entityTemplate.Output, moduleName, entity.TypeName));
                                            }
                                        });
                                }
                                #endregion

                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}