using System;
using FirstFloor.ModernUI.Presentation;
using SuperRocket.Core.Interfaces;
using SuperRocket.ModuleTwo.Views;

namespace SuperRocket.ModuleTwo.Services
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
                DisplayName = "Module Two"
            };

            linkGroup.Links.Add(new Link
            {
                DisplayName = "Module Two",
                Source = new Uri($"/SuperRocket.ModuleTwo;component/Views/{nameof(MainView)}.xaml", UriKind.Relative)
            });

            return linkGroup;
        }
    }
}
