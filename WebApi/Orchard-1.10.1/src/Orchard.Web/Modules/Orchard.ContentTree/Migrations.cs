using System;
using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Orchard.ContentTree
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable("ContentTreeSettingsRecord", table =>
                table.ContentPartRecord().
                Column<string>("IncludedTypes", col => col.Unlimited())
                );

            ContentDefinitionManager.AlterPartDefinition("ContentTreeSettingsPart", part => part.Attachable());

            ContentDefinitionManager.AlterTypeDefinition("ContentTreeSettings", type => type.WithPart("ContentTreeSettingsPart"));

            return 2;
        }

        public int UpgradeFrom1() {
            ContentDefinitionManager.AlterPartDefinition("ContentTreeSettingsPart", part => part.Attachable());

            return 2;
        }
    }
}