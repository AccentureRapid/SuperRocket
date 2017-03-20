﻿using System;
using FirstFloor.ModernUI.Presentation;
using SuperRocket.ModuleOne.Views;
using SuperRocket.Core.Interfaces;

namespace SuperRocket.ModuleOne.Services
{
    /// <summary>
    /// Creates a LinkGroup
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// This is the entry point for the option menu.
    /// </remarks>
    public class LinkGroupService : ILinkGroupService
    {
        public LinkGroup GetLinkGroup()
        {
            LinkGroup linkGroup = new LinkGroup
            {
                DisplayName = "Module One"
            };

            linkGroup.Links.Add(new Link
            {
                DisplayName = "Module One",
                Source = new Uri($"/SuperRocket.ModuleOne;component/Views/{nameof(MainView)}.xaml", UriKind.Relative)
            });

            return linkGroup;
        }
    }
}
