using System;
using System.Windows.Controls;

namespace SuperRocket.SuperChromium.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            //TODO here will load all the modules information and then initialize the memu for modules
            var link = new FirstFloor.ModernUI.Presentation.Link();
            link.DisplayName = "IDS";
            link.Source = GetUri(typeof(Chromium));
            this.chromiumTab.Links.Add(link);
        }

        private Uri GetUri(Type viewType)
        {
            return new Uri($"/SuperRocket.SuperChromium;component/Views/{viewType.Name}.xaml", UriKind.Relative);
        }
    }
}
