﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement.Drivers;
using RM.QuickLogOn.Models;
using RM.QuickLogOn.Services;

namespace RM.QuickLogOn.Drivers
{
    public class QuickLogOnWidget : ContentPartDriver<QuickLogOnWidgetPart>
    {
        private readonly IQuickLogOnService _quickLogOnService;

        public QuickLogOnWidget(IQuickLogOnService quickLogOnService)
        {
            _quickLogOnService = quickLogOnService;
        }

        protected override DriverResult Display(QuickLogOnWidgetPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_QuickLogOnWidget", () => shapeHelper.Parts_QuickLogOnWidget(Providers: _quickLogOnService.GetProviders()));
        }
    }
}
