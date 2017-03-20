using System.Collections.ObjectModel;
using System.Linq;
using Prism.Mvvm;
using SuperRocket.Core.Model;
using SuperRocket.Core.Services;
using SuperRocket.SuperChromium.Services;
using CefSharp.Wpf;
using System.Windows.Input;
using FirstFloor.ModernUI.Presentation;
using System;
using Prism.Commands;
using CefSharp;
using SuperRocket.SuperChromium.ResourceHandler;

namespace SuperRocket.SuperChromium.ViewModels
{
    public class ChromiumViewModel : BindableBase
    {
        private string address;
        public string Address
        {
            get { return address; }
            set { SetProperty(ref address, value); }
        }


        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private IWpfWebBrowser webBrowser;
        public IWpfWebBrowser WebBrowser
        {
            get { return webBrowser; }
            set { SetProperty(ref webBrowser, value); }
        }

        public ICommand ShowDevToolsCommand { get; private set; }

        public ChromiumViewModel(
            ICustomerService service,
            IBrowserManager manager)
        {
            manager.CefInitialize();
            //TODO get the enabled module's default home page to set the address.
            Address = "local://Resource/Modules/Example/Default.html";
            Title = "This is a module for Super Rocket";

            ShowDevToolsCommand = new DelegateCommand(this.ShowDevTools);
        }


        private void ShowDevTools()
        {
            WebBrowser.GetBrowser().GetHost().ShowDevTools();
            Keyboard.ClearFocus();
        }
    }
}
