using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace OShop.Downloads.Migrations {
    public class DownloadsMigrations : DataMigrationImpl {
        public int Create() {
			SchemaBuilder.CreateTable("DownloadableProductPartRecord", table => table
				.ContentPartRecord()
				.Column<int>("MediaId", c => c.Nullable())
			);

            ContentDefinitionManager.AlterPartDefinition("DownloadableProductPart", part => part
                .Attachable()
                .WithDescription("Allows to attach a downloadable media to a product")
            );

            ContentDefinitionManager.AlterPartDefinition("OrderDownloadsPart", part => part
                .Attachable(false)
            );

            ContentDefinitionManager.AlterTypeDefinition("Order", type => type
                .WithPart("OrderDownloadsPart")
            );

            return 1;
        }
    }
}