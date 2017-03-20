using Orchard.Data.Migration;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using System.Data;
using Orchard.Indexing;
using Orchard.Environment.Extensions;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;

namespace Orchard.SuperRocket.Migrations
{
    public class Migrations : DataMigrationImpl
    {
        private readonly ITaxonomyService _taxonomyService;
        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;
        private readonly string[] ADGroup = { "Global", "CIO", "AccentureLeadership" };
        public Migrations(
            ITaxonomyService taxonomyService,
            IOrchardServices orchardServices,
            IContentManager contentManager)
        {
            _taxonomyService = taxonomyService;
            _orchardServices = orchardServices;
            _contentManager = contentManager;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        public int Create()
        {
            //CreateTaxonomy("ADGroup", ADGroup);

            #region Html packaged module
            {

                ContentDefinitionManager.AlterPartDefinition("HtmlModulePart", builder => builder.Attachable());
                ContentDefinitionManager.AlterTypeDefinition("HtmlModule", cfg => cfg
                    .WithPart("CommonPart")
                    .WithPart("HtmlModulePart")
                    .Creatable()
                    .Listable()
                    .Indexed()
                    );

                ContentDefinitionManager.AlterPartDefinition("HtmlModulePart", partBuilder => partBuilder
                    .WithField("Title", fieldBuilder => fieldBuilder.OfType("TextField")
                        .WithDisplayName("Module Title")
                        .WithSetting("TextFieldSettings.Required", "true")
                        .WithSetting("TextFieldSettings.Flavor", "Wide"))
                    .WithField("Author", fieldBuilder => fieldBuilder.OfType("TextField")
                        .WithDisplayName("Module Author")
                        .WithSetting("TextFieldSettings.Required", "false")
                        .WithSetting("TextFieldSettings.Flavor", "Wide"))
                   .WithField("Description", fieldBuilder => fieldBuilder.OfType("TextField")
                        .WithDisplayName("Module Description")
                        .WithSetting("TextFieldSettings.Required", "false")
                        .WithSetting("TextFieldSettings.Flavor", "Wide"))
                   .WithField("Url", fieldBuilder => fieldBuilder.OfType("TextField")
                        .WithDisplayName("Introduction Url")
                        .WithSetting("TextFieldSettings.Required", "false")
                        .WithSetting("TextFieldSettings.Flavor", "Wide"))
                        );

                ContentDefinitionManager.AlterPartDefinition("HtmlModulePart",
                    builder => builder
                        .WithField("HtmlModuleFile",
                            fieldBuilder => fieldBuilder
                                .OfType("MediaLibraryPickerField")
                                .WithDisplayName("Module File")));

                ContentDefinitionManager.AlterPartDefinition("HtmlModulePart",
                     builder => builder
                         .WithField("HtmlModuleIco",
                             fieldBuilder => fieldBuilder
                                 .OfType("MediaLibraryPickerField")
                                 .WithDisplayName("Module ICO")));
            }



            #endregion

            return 1;
        }
        private void CreateTaxonomy(string taxonomyName, string[] terms)
        {
            var taxonomy = _taxonomyService.GetTaxonomyByName(taxonomyName);

            if (taxonomy == null)
            {
                try
                {
                    taxonomy = _orchardServices.ContentManager.New("Taxonomy").As<TaxonomyPart>();
                    taxonomy.Name = taxonomyName;
                    taxonomy.ContentItem.As<TitlePart>().Title = taxonomyName;
                    _taxonomyService.CreateTermContentType(taxonomy);
                    _orchardServices.ContentManager.Create(taxonomy);
                    _orchardServices.ContentManager.Publish(taxonomy.ContentItem);

                    foreach (var term in terms)
                    {
                        CreateTerm(taxonomy, term);
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Error("Error occurs when create terms with the migration :" + ex.Message);
                }
            }
        }

        private void CreateTerm(TaxonomyPart tax, string termName)
        {
            var term = _taxonomyService.NewTerm(tax);
            term.Name = termName;
            term.Selectable = true;
            _contentManager.Create(term, VersionOptions.Published);
        }

    }
}