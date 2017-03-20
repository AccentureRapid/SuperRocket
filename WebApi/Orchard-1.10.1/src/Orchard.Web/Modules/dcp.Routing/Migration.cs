using System;
using dcp.Routing.Models;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace dcp.Routing
{
    [OrchardFeature("dcp.Routing.Redirects")]
    public class RedirectsMigration : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable("RedirectRule", table => table
                .Column<int>("Id", c => c
                    .PrimaryKey()
                    .Identity())
                .Column<DateTime>("CreatedDateTime", c => c.NotNull().WithDefault("GetDate()"))
                .Column<string>("SourceUrl", c => c.NotNull().WithDefault(""))
                .Column<string>("DestinationUrl", c => c.NotNull().WithDefault(""))
                .Column<bool>("IsPermanent", c => c.NotNull().WithDefault(false))
                );

            return 1;
        }
    }
    
    public class Migrations : DataMigrationImpl
    {

        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ExtendedAliasRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<int>("AliasRecord_Id", column => column.NotNull())
                .Column<string>("RouteName", column => column
                    .NotNull()
                    .WithLength(50))
            );

            SchemaBuilder.CreateForeignKey("ExtendedAliasRecord_AliasRecord", "ExtendedAliasRecord", new[] { "AliasRecord_Id" }, "Orchard.Alias", "AliasRecord", new[] { "Id" });

            return 1;
        }

    }
}