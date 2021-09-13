
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using Prism.Ioc;
using Prism.Regions;
using HandyControlWpfCoreApp1.Views;
using Syncfusion.SfSkinManager;

namespace HandyControlWpfCoreApp1
{
    public partial class App
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDk4MTEyQDMxMzkyZTMyMmUzMEpxK3hOT3ZvblgxVy9UNnpSTlJTTXR2bUJGNUZEQ0MvMUFBMWFBU0tyc3c9");
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
            //containerRegistry.RegisterDialog<DialogControl, DialogControlViewModel>("MessageBox");
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Container.Resolve<IRegionManager>().RegisterViewWithRegion("ContentRegion", typeof(PaymentWindow));
        }
    }
}
