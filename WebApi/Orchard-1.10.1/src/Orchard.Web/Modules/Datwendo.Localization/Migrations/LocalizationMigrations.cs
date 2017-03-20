using System.Linq;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using Orchard.Autoroute.Settings;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement.MetaData.Models;

namespace Datwendo.Localization
{
    [OrchardFeature("Datwendo.Localization")]
    public class Migrations : DataMigrationImpl
    {
        const string LocalizationPartTypeName = "LocalizationPart";
        const string AutoroutePartTypeName = "AutoroutePart";
        const string LocalizedTitleName = "LocalizedTitle";
        static readonly string[] _contentTypesToUpdate = new[] { "Page", "BlogPost", "ProjectionPage", "ContentMenuItem", "MenuItem", "HtmlMenuItem", "NavigationQueryMenuItem", "ShapeMenuItem", "TaxonomyNavigationMenuItem" };

        public int Create()
        {                      

            foreach (var contentTypeName in _contentTypesToUpdate)
            {
                var typeDef = ContentDefinitionManager.GetTypeDefinition(contentTypeName);
                if (typeDef == null) continue;

                var localizationPart = typeDef.Parts.Where(p=>p.PartDefinition.Name == LocalizationPartTypeName).FirstOrDefault();
                if (localizationPart == null)
                {
                    ContentDefinitionManager.AlterTypeDefinition(contentTypeName, c=>c.WithPart(LocalizationPartTypeName));
                    typeDef = ContentDefinitionManager.GetTypeDefinition(contentTypeName);
                }

                var autoroutePart = typeDef.Parts.Where(p => p.PartDefinition.Name == AutoroutePartTypeName).FirstOrDefault();
                if (autoroutePart == null) continue;

                var autoroutePartSettings = LoadAutorouteSettings(autoroutePart.Settings);

                var routePattern = autoroutePartSettings.Patterns.Where(p => p.Name == LocalizedTitleName).FirstOrDefault();
                if (routePattern == null)
                {
                    var patterns = autoroutePartSettings.Patterns.ToList();
                    var baseRoute = patterns.FirstOrDefault();
                    routePattern = new RoutePattern
                    { 
                        Name = LocalizedTitleName,
                        Pattern = baseRoute != null ? string.Format("{{Content.Culture}}/{0}", baseRoute.Pattern) : "{Content.Culture}/{Content.Slug}",
                        Description = baseRoute != null ? string.Format("en-us/{0}", baseRoute.Description) : "en-us/my-content-item"
                    };
                    patterns.Add(routePattern);
                    autoroutePartSettings.Patterns = patterns;
                }
                var defaultIndex = autoroutePartSettings.Patterns.IndexOf(routePattern);
                autoroutePartSettings.DefaultPatternIndex = defaultIndex.ToString();

                SetAutorouteSettings(autoroutePartSettings, autoroutePart.Settings);

                ContentDefinitionManager.StoreTypeDefinition(typeDef);
            }           
            return 4;
        }

        public int UpdateFrom1()
        {

            // Define the DataSection content type and set it up to turn it into a menu item type
            ContentDefinitionManager.AlterTypeDefinition("CulturePickerLink", type => type
                .WithPart("CookieCulturePickerPart")     // Our custom part that will hold the Action, Controller, Area and RouteValues information
                .WithPart("MenuPart")           // Required so that the Navigation system can attach our custom menu items to a menu
                .WithPart("CommonPart")         // Required, contains common informatin such as the owner and creation date of our type. Many modules depend on this part being present
                .WithPart("IdentityPart")       // To support import / export, our type needs an identity since we won;t be providing one ourselves
                .DisplayedAs("CulturePicker Link")     // Specify the name to be displayed to the admin user

                // The value of the Description setting will be shown in the Navigation section where our custom menu item type will appear
                .WithSetting("Description", "Represents a Meny Item built on the Culture Picker.")

                // Required by the Navigation module
                .WithSetting("Stereotype", "MenuItem")

                // We don't want our menu items to be draftable
                .Draftable(false)

                // We don't want the user to be able to create new ActionLink items outside of the context of a menu
                .Creatable(false)
                );

            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable("CookieCulturePickerPartRecord",
                    table => table.AddColumn<bool>("ShowBrowser",c => c.WithDefault(true)));

            SchemaBuilder.CreateTable("MenuCulturePickerPartRecord",
                table => table.ContentPartRecord()
                    .Column<bool>("ShowBrowser", c => c.WithDefault(true))
                );

            ContentDefinitionManager.AlterPartDefinition(
                "MenuCulturePickerPart",
                builder => builder.Attachable());

            // Define the DataSection content type and set it up to turn it into a menu item type
            ContentDefinitionManager.AlterTypeDefinition("CulturePickerLink", type => type
                .WithPart("MenuCulturePickerPart")     // Our custom part that will hold the Action, Controller, Area and RouteValues information
                .WithPart("MenuPart")           // Required so that the Navigation system can attach our custom menu items to a menu
                .WithPart("CommonPart")         // Required, contains common informatin such as the owner and creation date of our type. Many modules depend on this part being present
                .WithPart("IdentityPart")       // To support import / export, our type needs an identity since we won;t be providing one ourselves
                .DisplayedAs("CulturePicker Link")     // Specify the name to be displayed to the admin user

                // The value of the Description setting will be shown in the Navigation section where our custom menu item type will appear
                .WithSetting("Description", "Represents a Meny Item built on the Culture Picker.")

                // Required by the Navigation module
                .WithSetting("Stereotype", "MenuItem")

                // We don't want our menu items to be draftable
                .Draftable(false)

                // We don't want the user to be able to create new ActionLink items outside of the context of a menu
                .Creatable(false)
                );

            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.DropTable("CookieCulturePickerPartRecord");
            SchemaBuilder.DropTable("MenuCulturePickerPartRecord");
            return 4;
        }

        private AutorouteSettings LoadAutorouteSettings(SettingsDictionary settings)
        {
            bool b;
            int i;
            string s;

            return new AutorouteSettings 
            {
                AllowCustomPattern = settings.TryGetValue("AutorouteSettings.AllowCustomPattern", out s) && bool.TryParse(s, out b) && b,
                AutomaticAdjustmentOnEdit = settings.TryGetValue("AutorouteSettings.AutomaticAdjustmentOnEdit", out s) && bool.TryParse(s, out b) && b,
                DefaultPatternIndex = settings.TryGetValue("AutorouteSettings.DefaultPatternIndex", out s) && int.TryParse(s, out i) ? i.ToString() : "-1",
                PatternDefinitions = settings.TryGetValue("AutorouteSettings.PatternDefinitions", out s) ? s : string.Empty,
                PerItemConfiguration = settings.TryGetValue("AutorouteSettings.PerItemConfiguration", out s) && bool.TryParse(s, out b) && b
            };
        }

        private void SetAutorouteSettings(AutorouteSettings autorouteSettings, SettingsDictionary settings)
        {
            settings["AutorouteSettings.AllowCustomPattern"] = autorouteSettings.AllowCustomPattern.ToString();
            settings["AutorouteSettings.AutomaticAdjustmentOnEdit"] = autorouteSettings.AutomaticAdjustmentOnEdit.ToString();
            settings["AutorouteSettings.DefaultPatternIndex"] = autorouteSettings.DefaultPatternIndex.ToString();
            settings["AutorouteSettings.PerItemConfiguration"] = autorouteSettings.PerItemConfiguration.ToString();
            settings["AutorouteSettings.PatternDefinitions"] = autorouteSettings.PatternDefinitions;
        }
    }
}
