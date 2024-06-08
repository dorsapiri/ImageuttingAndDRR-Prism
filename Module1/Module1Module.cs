using Module1.Services;
using Module1.ViewModels;
using Module1.Views;
using OrganDRR_sample.Services;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Module1
{
    public class Module1Module : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IRemoveTableService, RemoveTableService>()
                             .RegisterSingleton<IDrrGeneratorService, DrrGeneratorService>()
                             .RegisterSingleton<IContourDRRService, ContourDRRService>()
                             .RegisterSingleton<IIsoCenterService, IsoCenterService>();
        }
    }
}