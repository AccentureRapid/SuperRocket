﻿using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using OShop.Models;
using OShop.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OShop.Handlers {
    [OrchardFeature("OShop.VAT")]
    public class OrderVatPartHandler : ContentHandler {
        public OrderVatPartHandler() {
            OnActivated<OrderVatPart>((context, part) => {
                part._vatAmounts.Loader(() => GetVatAmounts(part));
            });
        }

        private List<TaxAmount> GetVatAmounts(OrderVatPart part) {
            List<TaxAmount> vatAmounts = new List<TaxAmount>();
            OrderPart order = part.As<OrderPart>();
            if (order != null) {
                foreach (var detail in order.Details) {
                    var vat = detail.GetProperty<Tax>("VAT");
                    if (vat != null) {
                        vatAmounts.AddTax(vat, detail.Total);
                    }
                }
            }
            return vatAmounts;
        }
    }
}