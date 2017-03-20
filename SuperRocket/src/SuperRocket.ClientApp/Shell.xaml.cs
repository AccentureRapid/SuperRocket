using System;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Presentation;
using SuperRocket.ClientApp.Constants;
using SuperRocket.ClientApp.Views;

namespace SuperRocket.ClientApp
{
    public partial class Shell : ModernWindow
    {
        public Shell()
        {
            InitializeComponent();
            AppearanceManager.Current.ThemeSource = new Uri(ThemesPath.SuperRocket, UriKind.Relative);
        }

        public void AddLinkGroups(LinkGroupCollection linkGroupCollection)
        {
            CreateMenuLinkGroup();

            foreach (LinkGroup linkGroup in linkGroupCollection)
            {
                this.MenuLinkGroups.Add(linkGroup);
            }
        }

        private void CreateMenuLinkGroup()
        {
            this.MenuLinkGroups.Clear();

            LinkGroup linkGroup = new LinkGroup
            {
                DisplayName = "Settings",
                GroupKey = "settings"
            };

            linkGroup.Links.Add(new Link
            {
                DisplayName = "Settings options",
                Source = GetUri(typeof(SettingsView))
            });

            this.MenuLinkGroups.Add(linkGroup);

            linkGroup = new LinkGroup
            {
                DisplayName = "Introduction"
            };

            linkGroup.Links.Add(new Link
            {
                DisplayName = "SuperRocket Framework Introduction",
                Source = GetUri(typeof(IntroductionView))
            });

            linkGroup.Links.Add(new Link
            {
                DisplayName = "Modern UI for WPF",
                Source = GetUri(typeof(MUIView))
            });

            linkGroup.Links.Add(new Link
            {
                DisplayName = "Data Grid",
                Source = GetUri(typeof(DataGrid))
            });

            linkGroup.Links.Add(new Link
            {
                DisplayName = "Lorem Ipsum",
                Source = GetUri(typeof(LoremIpsumView))
            });

            this.MenuLinkGroups.Add(linkGroup);
        }

        private Uri GetUri(Type viewType)
        {
            return new Uri($"/Views/{viewType.Name}.xaml", UriKind.Relative);
        }
    }
}