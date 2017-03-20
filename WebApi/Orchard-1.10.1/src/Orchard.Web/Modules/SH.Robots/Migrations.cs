using Orchard.Data.Migration;

namespace SH.Robots {
	public class Migrations : DataMigrationImpl {
		public int Create() {
			SchemaBuilder.CreateTable("RobotsFileRecord",
				table => table
					.Column<int>("Id", col => col.PrimaryKey().Identity())
					.Column<string>("FileContent", col => col.Nullable().Unlimited().WithDefault(@"User-agent: *
Allow: /"))
				);
			return 1;
		}
	}
}