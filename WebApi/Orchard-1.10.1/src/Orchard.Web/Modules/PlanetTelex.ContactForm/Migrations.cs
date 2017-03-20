using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using PlanetTelex.ContactForm.Models;

namespace PlanetTelex.ContactForm {
    public class Migrations : DataMigrationImpl 
    {
        /// <summary>
        /// This executes whenever this module is activated.
        /// </summary>
        public int Create() 
        {
			// Creating table ContactFormRecord
			SchemaBuilder.CreateTable("ContactFormRecord", table => table
				.ContentPartRecord()
                .Column<string>("RecipientEmailAddress", column => column.WithLength(800))
                .Column<string>("StaticSubjectMessage", column => column.WithLength(2000))
				.Column<bool>("UseStaticSubject")
                .Column<bool>("DisplayNameField")
                .Column<bool>("RequireNameField")
			);

            ContentDefinitionManager.AlterPartDefinition(
                typeof(ContactFormPart).Name, cfg => cfg.Attachable());

            return 1;
        }

        public int UpdateFrom1()
        {
            ContentDefinitionManager.AlterTypeDefinition("ContactFormWidget", cfg => cfg
                .WithPart("ContactFormPart")
                .WithPart("WidgetPart")
                .WithPart("CommonPart")
                .WithSetting("Stereotype", "Widget"));

            return 2;
        }

        public int UpdateFrom2() 
        {
            SchemaBuilder.CreateTable("ContactFormSettingsRecord", table => table
               .ContentPartRecord()
               .Column<bool>("EnableSpamProtection")
               .Column<bool>("EnableSpamEmail")
               .Column<string>("SpamEmail")
              );

            return 3;
        }

        public int UpdateFrom3()
        {
            ContentDefinitionManager.AlterPartDefinition(typeof(ContactFormPart).Name, builder => builder
                .WithDescription("A simple contact form with spam protection."));

            return 4;
        }
    }
}