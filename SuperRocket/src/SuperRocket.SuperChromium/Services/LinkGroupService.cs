using System;
using FirstFloor.ModernUI.Presentation;
using SuperRocket.Core.Interfaces;
using SuperRocket.SuperChromium.Views;

namespace SuperRocket.SuperChromium.Services
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
                DisplayName = "Super Chromium"
            };

            linkGroup.Links.Add(new Link
            {
                DisplayName = "Super Chromium",
                Source = new Uri($"/SuperRocket.SuperChromium;component/Views/{nameof(MainView)}.xaml", UriKind.Relative)
            });

            return linkGroup;
        }
    }
}
