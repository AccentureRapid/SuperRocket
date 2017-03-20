using Orchard.Data.Migration;

namespace Contrib.MediaFolder {
    public class Migrations : DataMigrationImpl {

        public int Create() {

            SchemaBuilder.CreateTable("RemoteStorageSettingsPartRecord", 
                table => table
                    .ContentPartRecord()
                    .Column<string>("MediaLocation", c => c.Unlimited())
                    .Column<string>("DirectRoute", c => c.Unlimited())
                    .Column<bool>("EnableDirectRoute")
                );

            return 1;
        }
    }
}