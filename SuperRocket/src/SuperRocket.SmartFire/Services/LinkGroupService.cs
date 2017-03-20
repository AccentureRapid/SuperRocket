using System;
using FirstFloor.ModernUI.Presentation;
using SuperRocket.SmartFire.Views;
using SuperRocket.Core.Interfaces;

namespace SuperRocket.SmartFire.Services
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
                DisplayName = "Smart Fire"
            };

            linkGroup.Links.Add(new Link
            {
                DisplayName = "Smart Fire",
                Source = new Uri($"/SuperRocket.SmartFire;component/Views/{nameof(MainView)}.xaml", UriKind.Relative)
            });

            return linkGroup;
        }
    }
}
