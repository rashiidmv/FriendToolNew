using Friend.Infra;
using Help.Navigation;
using Help.Views;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Help
{
    public class HelpModule : ModuleBase
    {
        public HelpModule(IUnityContainer uc, IRegionManager rm) : base(uc, rm)
        {
        }
        protected override void InitializeModules()
        {
            MyRegionManager.RegisterViewWithRegion(RegionNames.ToolbarRegion, typeof(Open));
            MyRegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(Main));
            //MyRegionManager.AddToRegion(RegionNames.ContentRegion,MyUnityContainer.Resolve<Main>());
            //MyRegionManager.RequestNavigate(RegionNames.ContentRegion, typeof(Main).FullName);

        }

        protected override void RegisterTypes()
        {
            MyUnityContainer.RegisterType<IMainViewModel, MainViewModel>();
            MyUnityContainer.RegisterType<object, Main>(typeof(Main).FullName);
            MyUnityContainer.RegisterForNavigation<Main>();
        }
    }
}
