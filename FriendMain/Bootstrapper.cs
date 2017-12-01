﻿using DataLists;
using Friend.Infra;
using Help;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using QueryWindow;
using System.Windows;
using System.Windows.Controls;

namespace FriendMain
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell() 
        {
            base.InitializeShell();

            Window mainWindow = (Window)Shell;
            mainWindow.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            App.Current.MainWindow = mainWindow;
            App.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterType<IShellViewModel, ShellViewModel>();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            ModuleCatalog m = new ModuleCatalog();
            m.AddModule(typeof(DataListModule));
            m.AddModule(typeof(QueryWindowModule));
            m.AddModule(typeof(HelpModule));
            return m;
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            RegionAdapterMappings rm = base.ConfigureRegionAdapterMappings();
            rm.RegisterMapping(typeof(StackPanel), Container.Resolve<StackPanelRegionAdapter>());
            return rm;
        }
    }
}
