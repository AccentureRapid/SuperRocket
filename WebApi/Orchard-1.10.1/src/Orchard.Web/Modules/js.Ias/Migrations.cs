using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using js.Ias.Models;
using Orchard.Core.Contents.Extensions;


namespace js.Ias
{
    public class Migrations : DataMigrationImpl
    {
        public Migrations()
        {
        }

        public int Create()
        {
                SchemaBuilder.CreateTable("InfiniteAjaxScrollingPartRecord",
                    table => table
                        .ContentPartRecord()
                    );
            

            ContentDefinitionManager.AlterPartDefinition(typeof(InfiniteAjaxScrollingPart).Name,
                                 cfg => cfg.Attachable());

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable("InfiniteAjaxScrollingPartRecord", table => table
                .AddColumn<bool>("UseHistory"));
            SchemaBuilder.AlterTable("InfiniteAjaxScrollingPartRecord", table => table
                .AddColumn<string>("Container"));
            SchemaBuilder.AlterTable("InfiniteAjaxScrollingPartRecord", table => table
                .AddColumn<string>("Item"));
            SchemaBuilder.AlterTable("InfiniteAjaxScrollingPartRecord", table => table
                .AddColumn<string>("Pagination"));
            SchemaBuilder.AlterTable("InfiniteAjaxScrollingPartRecord", table => table
                .AddColumn<string>("NextAnchor"));
            SchemaBuilder.AlterTable("InfiniteAjaxScrollingPartRecord", table => table
                .AddColumn<string>("Loader"));
            SchemaBuilder.AlterTable("InfiniteAjaxScrollingPartRecord", table => table
                .AddColumn<string>("onPageChange", column => column.Unlimited()));
            SchemaBuilder.AlterTable("InfiniteAjaxScrollingPartRecord", table => table
                .AddColumn<string>("beforePageChange", column => column.Unlimited()));
            SchemaBuilder.AlterTable("InfiniteAjaxScrollingPartRecord", table => table
                .AddColumn<string>("onLoadItems", column => column.Unlimited()));
            SchemaBuilder.AlterTable("InfiniteAjaxScrollingPartRecord", table => table
                .AddColumn<string>("onRenderComplete", column => column.Unlimited()));           

            SchemaBuilder.CreateTable("InfiniteAjaxScrollingPartSettingsRecord", table => table
                .ContentPartRecord()
                .Column<bool>("UseHistory", column => column.NotNull().WithDefault(false))
                .Column<string>("Container", column => column.NotNull().WithDefault(".content-items"))
                .Column<string>("Item", column => column.NotNull().WithDefault(".content-item"))
                .Column<string>("Pagination", column => column.NotNull().WithDefault(".zone-content .pager"))
                .Column<string>("NextAnchor", column => column.NotNull().WithDefault(".zone-content .pager .last a"))
                .Column<string>("Loader", column => column.NotNull().WithDefault("~/Modules/js.Ias/styles/images/loader.gif"))
                .Column<string>("onPageChange", column => column.Unlimited())
                .Column<string>("beforePageChange", column => column.Unlimited())
                .Column<string>("onLoadItems", column => column.Unlimited())
                .Column<string>("onRenderComplete", column => column.Unlimited())
            );
            return 2;
        }
    }
}