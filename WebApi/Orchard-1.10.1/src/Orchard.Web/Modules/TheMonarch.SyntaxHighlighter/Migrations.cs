using Orchard.Data.Migration;

namespace TheMonarch.SyntaxHighlighter {
    public class Migrations : DataMigrationImpl {

        public int Create() {

            SchemaBuilder.CreateTable("SyntaxHighlighterSettingsPartRecord", 
                table => table
                    .ContentPartRecord()
                    .Column<string>("Theme")
                );

            return 1;
        }
    }
}