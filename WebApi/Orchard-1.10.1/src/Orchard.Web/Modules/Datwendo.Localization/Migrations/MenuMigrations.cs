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
    [OrchardFeature("Datwendo.Localization.MenuCulturePicker")]
    public class MenuMigrations : DataMigrationImpl
    {       
        public int Create()
        {           
            SchemaBuilder.CreateTable("MenuCulturePickerPartRecord",
                            table => table.ContentPartRecord()
                                .Column<bool>("ShowBrowser", c => c.WithDefault(true))
                            );

            ContentDefinitionManager.AlterPartDefinition(
                "MenuCulturePickerPart",
                builder => builder.Attachable());
           

            // Define the CulturePickerLink content type and set it up to turn it into a menu item type
            ContentDefinitionManager.AlterTypeDefinition("CulturePickerLink", type => type
                .WithPart("MenuCulturePickerPart")     // Our custom part that will hold the Action, Controller, Area and RouteValues information
                .WithPart("MenuPart")           // Required so that the Navigation system can attach our custom menu items to a menu
                .WithPart("CommonPart")         // Required, contains common informatin such as the owner and creation date of our type. Many modules depend on this part being present
                .WithPart("IdentityPart")       // To support import / export, our type needs an identity since we won;t be providing one ourselves
                .DisplayedAs("CulturePicker Link")     // Specify the name to be displayed to the admin user

                // The value of the Description setting will be shown in the Navigation section where our custom menu item type will appear
                .WithSetting("Description", "Represents a Menu Item built on the Culture Picker.")

                // Required by the Navigation module
                .WithSetting("Stereotype", "MenuItem")

                // We don't want our menu items to be draftable
                .Draftable(false)

                // We don't want the user to be able to create new ActionLink items outside of the context of a menu
                .Creatable(false)
                );
            return 1;
        }
    }
}
