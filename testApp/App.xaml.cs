﻿using Module1.Services;
using Module1.ViewModels;
using OrganDRR_sample.Services;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using testApp.ViewModels;
using testApp.Views;

namespace testApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<MainWindowViewModel>();
            containerRegistry.RegisterSingleton<IDrrGeneratorService, DrrGeneratorService>();
            containerRegistry.RegisterSingleton<IContourDRRService, ContourDRRService>();
            containerRegistry.RegisterSingleton<IIsoCenterService, IsoCenterService>();
        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<Module1.Module1Module>();
        }
    }
}
