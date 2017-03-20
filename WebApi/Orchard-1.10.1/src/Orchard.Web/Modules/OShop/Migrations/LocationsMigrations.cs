﻿using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace OShop.Migrations {
    [OrchardFeature("OShop.Locations")]
    public class LocationsMigrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable("LocationsCountryRecord", table => table
                 .Column<int>("Id", c => c.PrimaryKey().Identity())
                 .Column<bool>("Enabled")
                 .Column<string>("Name")
                 .Column<string>("IsoCode", c => c.WithLength(8))
                 .Column<string>("AddressFormat", c => c.WithLength(1000))
                 .Column<int>("ShippingZoneRecord_Id"));

            SchemaBuilder.CreateTable("LocationsStateRecord", table => table
                 .Column<int>("Id", c => c.PrimaryKey().Identity())
                 .Column<bool>("Enabled")
                 .Column<string>("Name")
                 .Column<string>("IsoCode", c => c.WithLength(8))
                 .Column<int>("LocationsCountryRecord_Id")
                 .Column<int>("ShippingZoneRecord_Id"));

            return 1;
        }
    }
}