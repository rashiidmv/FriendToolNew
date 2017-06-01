using Friend.Infra;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using QueryWindow;
using System.Windows;
using System.Windows.Controls;

namespace FriendExtension
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }


        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<IShellViewModel, ShellViewModel>();
        }
        protected override IModuleCatalog CreateModuleCatalog()
        {
            ModuleCatalog m = new ModuleCatalog();
            m.AddModule(typeof(QueryWindowModule)); 
            return m;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
        }


        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            RegionAdapterMappings rm = base.ConfigureRegionAdapterMappings();
            rm.RegisterMapping(typeof(StackPanel), Container.Resolve<StackPanelRegionAdapter>());
            return rm;
        }

        public UserControl GetShell()
        {
            ((UserControl)Shell).Visibility = Visibility.Visible;
            return (UserControl)Shell;
        }
    }
}
