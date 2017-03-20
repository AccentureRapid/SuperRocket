﻿using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using OShop.Models;
using OShop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OShop.Drivers {
    [OrchardFeature("OShop.Products")]
    public class OrderProductsPartDriver : ContentPartDriver<OrderProductsPart> {
        private const string TemplateName = "Parts/Order.Products";

        protected override string Prefix { get { return "OrderProducts"; } }

        public OrderProductsPartDriver() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override DriverResult Display(OrderProductsPart part, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_Order_Products", () => shapeHelper.Parts_Order_Products(
                    ContentPart: part)),
                ContentShape("Parts_Order_Products_SubTotal", () => shapeHelper.Parts_Order_SubTotal(
                    Label: T("Products total"),
                    SubTotal: part.ProductDetails.Sum(d => d.Total)))
            );
        }

        protected override DriverResult Editor(OrderProductsPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Order_Products_Edit", () => shapeHelper.EditorTemplate(
                    TemplateName: TemplateName,
                    Model: part,
                    Prefix: Prefix)
                );
        }

        protected override DriverResult Editor(OrderProductsPart part, Orchard.ContentManagement.IUpdateModel updater, dynamic shapeHelper) {


            return Editor(part, shapeHelper);
        }
    }
}