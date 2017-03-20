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
    [OrchardFeature("Datwendo.Localization.CookieCultureSelector")]
    public class CookieMigrations : DataMigrationImpl
    {
        public int Create()
        {           
            SchemaBuilder.CreateTable("CookieCulturePickerPartRecord",
                                        table => table.ContentPartRecord()
                                            .Column<string>("Style", c => c.WithLength(255))
                                            .Column<bool>("ShowBrowser", c => c.WithDefault(true))
                                        );

            ContentDefinitionManager.AlterPartDefinition(
                "CookieCulturePickerPart",
                builder => builder.Attachable());

            ContentDefinitionManager.AlterTypeDefinition(
                "CookieCulturePicker",
                cfg => cfg
                           .WithPart("CommonPart")
                           .WithPart("IdentityPart")
                           .WithPart("WidgetPart")
                           .WithPart("CookieCulturePickerPart")
                           .WithSetting("Stereotype", "Widget")
                );
            return 1;
        }
    }
}
