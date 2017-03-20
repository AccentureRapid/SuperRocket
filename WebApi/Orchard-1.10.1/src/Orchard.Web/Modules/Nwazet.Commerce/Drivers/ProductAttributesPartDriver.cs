﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributesPartDriver : ContentPartDriver<ProductAttributesPart>, IProductAttributesDriver {
        private readonly IProductAttributeService _attributeService;
        private readonly IEnumerable<IProductAttributeExtensionProvider> _attributeExtensions;

        public ProductAttributesPartDriver(
            IProductAttributeService attributeService,
            IEnumerable<IProductAttributeExtensionProvider> attributeExtensions) {

            _attributeService = attributeService;
            _attributeExtensions = attributeExtensions;

        }

        protected override string Prefix { get { return "NwazetCommerceAttribute"; } }

        protected override DriverResult Display(
            ProductAttributesPart part, string displayType, dynamic shapeHelper) {

            // The shape is acquired by the product driver so it can be included into the add to cart shape
            return null;
        }

        public dynamic GetAttributeDisplayShape(IContent product, dynamic shapeHelper) {
            var attributesPart = product.As<ProductAttributesPart>();
            var attributes = attributesPart == null
                ? null
                : _attributeService.GetAttributes(attributesPart.AttributeIds);
            return shapeHelper.Parts_ProductAttributes(
                ContentItem: product,
                ProductAttributes: attributes.OrderBy(a => a.SortOrder)
                    .Select(a => new ProductAttributePartDisplayViewModel {
                        Part = a,
                        // Return all possible attribute extensions input shapes
                        AttributeExtensionShapes = _attributeExtensions.Where(e => a.AttributeValues.Any(av => av.ExtensionProvider == e.Name))
                            .Select(e => e.BuildInputShape(a))
                    })
                );
        }

        public bool ValidateAttributes(IContent product, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues) {
            var attributesPart = product.As<ProductAttributesPart>();
            //if (attributeIdsToValues.Count == 1 &&
            //    attributeIdsToValues[0].)
            // If the part isn't there, there must be no attributes
            if (attributesPart == null) return attributeIdsToValues == null || !attributeIdsToValues.Any();
            // If the part is there, it must have as many attributes as were passed in
            if (attributesPart.AttributeIds.Count() != attributeIdsToValues.Count) return false;
            // The same attributes must be present
            if (!attributesPart.AttributeIds.All(attributeIdsToValues.ContainsKey)) return false;
            // Get the actual attributes in order to verify the values
            var attributes = _attributeService.GetAttributes(attributeIdsToValues.Keys);
            // The values that got passed in must exist
            return attributes.All(attribute => attribute.AttributeValues.Any(v => v.Text == attributeIdsToValues[attribute.Id].Value));
        }

        //GET
        protected override DriverResult Editor(ProductAttributesPart part, dynamic shapeHelper) {
            return ContentShape(
                "Parts_ProductAttributes_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/ProductAttributes",
                    Prefix: Prefix,
                    Model: new ProductAttributesPartEditViewModel {
                        Prefix = Prefix,
                        Part = part,
                        Attributes = _attributeService.Attributes
                            .OrderBy(p => p.As<TitlePart>().Title)
                            .ToList()
                    }));
        }

        //POST
        protected override DriverResult Editor(ProductAttributesPart part, IUpdateModel updater, dynamic shapeHelper) {
            var editViewModel = new ProductAttributesEditViewModel();
            if (updater.TryUpdateModel(editViewModel, Prefix, null, null)) {
                part.AttributeIds = editViewModel.AttributeIds;
            }
            return Editor(part, shapeHelper);
        }

        private class ProductAttributesEditViewModel {
            public int[] AttributeIds { get; set; }
        }

        protected override void Importing(ProductAttributesPart part, ImportContentContext context) {
            var values = context.Attribute(part.PartDefinition.Name, "Ids");
            if (!String.IsNullOrWhiteSpace(values)) {
                part.Record.Attributes = values;
            }
        }

        protected override void Exporting(ProductAttributesPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Ids", part.Record.Attributes);
        }
    }
}
