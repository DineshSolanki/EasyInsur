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
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NzE0NTY3QDMyMzAyZTMyMmUzMFQzYWk3ejF3QU5FZmZtc2FkQ1VzL3FlYVc3T1ZoMENDQTd4R0FTeE9FNGM9");
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
