using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

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

    }
}
