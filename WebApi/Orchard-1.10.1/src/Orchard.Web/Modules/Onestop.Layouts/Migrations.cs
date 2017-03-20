using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;
using Orchard.Environment.Features;
using Orchard.Localization;
using System.Linq;
using System;

namespace Onestop.Layouts {
    [OrchardFeature("Onestop.Layouts")]
    public class Migrations : DataMigrationImpl {
        private readonly ShellSettings _shellSettings;
        public Localizer T;

        private string layoutTypeName;
        private string templateTypeName;
        private string layoutDisplayName;
        private string templateDisplayName;
        private readonly IFeatureManager _featureManager;
        
        private bool upgrade; //detect if we are upgrading an existing Onestop.Layouts

        public Migrations(ShellSettings shellSettings, IFeatureManager featureManager)
        {
            _shellSettings = shellSettings;
            _featureManager = featureManager;
            layoutTypeName = "Layout";
            templateTypeName = "Template";
            layoutDisplayName = "Layout";
            templateDisplayName = "Template";
            upgrade = true; //see if we are doing an upgrade or new installation

        }
        public int Create() {
                        
            //check to see if the content already exists

            if (_featureManager.GetEnabledFeatures().Any(x => x.Id == "Orchard.Layouts"))
                upgrade = false;


            if (upgrade == false)
            {
                templateTypeName = "OSTemplate";
                layoutTypeName = "OSLayout";
                templateDisplayName = "Onestop Template";
                layoutDisplayName = "Onestop Layout";

            }
            
            ContentDefinitionManager.AlterTypeDefinition(
                layoutTypeName, cfg => cfg
                    .DisplayedAs(layoutDisplayName)
                    .RemovePart("LayoutPart")
                    .WithPart("LayoutTemplatePart")
                    .WithPart("CommonPart")
                    .WithPart("IdentityPart")
                    .WithPart("TitlePart"));

            ContentDefinitionManager.AlterTypeDefinition(
                templateTypeName, cfg => cfg
                    .DisplayedAs(templateDisplayName)
                    .RemovePart("LayoutPart")
                    .WithPart("LayoutTemplatePart", builder => builder
                        .WithSetting("isTemplate", "true"))
                    .WithPart("CommonPart")
                    .WithPart("IdentityPart")
                    .WithPart("TitlePart"));

            ContentDefinitionManager.AlterPartDefinition("TemplatedItemPart", cfg => cfg
                .WithDescription("Enables your content type to use a layout template.")
                .Attachable());

            return 1;
        }

        public int UpdateFrom1() {
            ContentDefinitionManager.AlterTypeDefinition("Layout", type => type
                .WithPart("IdentityPart"));
            ContentDefinitionManager.AlterTypeDefinition("Template", type => type
                .WithPart("IdentityPart"));
                      
            return 2;
        }

        public int UpdateFrom2()
        {


            //rename the tables if you've already installed the community edition

            if (upgrade)
            {
                string sql = "";
                var tablePrefix = String.IsNullOrEmpty(_shellSettings.DataTablePrefix)
                                       ? ""
                                       : _shellSettings.DataTablePrefix + "_";

                sql = "update [dbo].[" + tablePrefix + "Orchard_Framework_ContentTypeRecord]  set name = 'OSLayout'  where id in (select top 1  id from [dbo].[" + tablePrefix + "Orchard_Framework_ContentTypeRecord]  where name = 'layout' order by id)";
                SchemaBuilder.ExecuteSql(sql);

                sql = "update [dbo].[" + tablePrefix + "Settings_ContentTypeDefinitionRecord] set name = 'OSLayout', DisplayName = 'Onestop Layout' where id in (select top 1  id from [dbo].[" + tablePrefix + "Settings_ContentTypeDefinitionRecord]  where name = 'layout' order by id)";

                SchemaBuilder.ExecuteSql(sql);

                //Services.Notifier.Information(T("Layouts have now been named OSLayouts"));

                sql = "update [dbo].[" + tablePrefix + "Orchard_Framework_ContentTypeRecord]  set name = 'OSTemplate' where id in (select top 1 id from [dbo].[" + tablePrefix + "Orchard_Framework_ContentTypeRecord]  where name = 'template' order by id)";
                SchemaBuilder.ExecuteSql(sql);

                sql = " update [dbo].[" + tablePrefix + "Settings_ContentTypeDefinitionRecord] set name = 'OSTemplate', DisplayName = 'Onestop Template' where id in (select top 1  id from [dbo].[" + tablePrefix + "Settings_ContentTypeDefinitionRecord]  where name = 'template' order by id)";
                SchemaBuilder.ExecuteSql(sql);
            }
            return 3;
        }

    }

   
}
