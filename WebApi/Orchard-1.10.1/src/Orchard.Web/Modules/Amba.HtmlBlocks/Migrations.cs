using Amba.HtmlBlocks.Models;
using Orchard.Data.Migration;

namespace Amba.HtmlBlocks
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof (HtmlBlockRecord).Name,
                table => table
                    .Column<int>("Id", x => x.PrimaryKey().Identity())
                    .Column<string>("BlockKey", x => x.WithLength(128).Unique().NotNull())
                    .Column<string>("HTML", x => x.Unlimited())
                    .Column<string>("HelpText", x => x.Unlimited())
                );
            return 1;
        }

    }
}