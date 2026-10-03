using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using EasyInsur.Views;
using Prism.Ioc;
using Prism.Regions;

namespace EasyInsur
{
    public partial class App
    {
        public App()
        {
            var syncfusionKey = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY");
            if (!string.IsNullOrEmpty(syncfusionKey))
            {
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionKey);
            }
            RepoDb.SqLiteBootstrap.Initialize();
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                        CultureInfo.CurrentCulture.IetfLanguageTag)));
            
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<PaymentWindow>();
            containerRegistry.RegisterForNavigation<PersonDetails>();
            containerRegistry.RegisterForNavigation<ViewPaymentWindow>();
            containerRegistry.RegisterForNavigation<ViewPeopleWindow>();
            containerRegistry.RegisterForNavigation<Dashboard>();
            //containerRegistry.RegisterDialog<DialogControl, DialogControlViewModel>("MessageBox");
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Container.Resolve<IRegionManager>().RegisterViewWithRegion("ContentRegion", typeof(Dashboard));
        }
    }
}
