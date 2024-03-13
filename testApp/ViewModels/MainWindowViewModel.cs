using Kitware.VTK;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Forms.Integration;
using System.Windows;
using testApp.Views;
using System.ComponentModel;
using Module1.ViewModels;

namespace testApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "DRR Window";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private IRegionManager _regionManager;
        vtkImageActor actor;
        private IDrrGeneratorService _drrGeneratorService;

        public IDrrGeneratorService drrGeneratorService
        {
            get { return _drrGeneratorService; }
            set { SetProperty(ref _drrGeneratorService, value); }
        }

        #region RenderWindowControl Property
        private RenderWindowControl _renderWindowControl;
        public RenderWindowControl renderWindowControl
        {
            get { return _renderWindowControl; }
            set
            {
                _renderWindowControl = value;
            }
        }
        #endregion
        public MainWindowViewModel(IRegionManager regionManager,IDrrGeneratorService drrGeneratorService)
        {
            _regionManager = regionManager;
            actor = new();
            _drrGeneratorService = drrGeneratorService;
            
            regionManager.RegisterViewWithRegion("ContentRegionDrrRotation", typeof(DrrTransformView));
        }
        #region WindowsFormHost Loaded
        public void WindowsFormsHost_Loaded(object sender, RoutedEventArgs e)
        {
            if (renderWindowControl == null)
            {
                WindowsFormsHost? windowsFormHost = sender as WindowsFormsHost;
                renderWindowControl = new();
                windowsFormHost.Child = renderWindowControl;
            }
            renderWindowControl.RenderWindow.Render();
            renderWindowControl.RenderWindow.GetInteractor().SetInteractorStyle(vtkInteractorStyleImage.New());
            string directoryPath = @"C:\Users\k703528\Documents\Study_1 _ CompleteS";
            var imageData = ReadDicomDirectory(directoryPath);
            
            var initgant = _drrGeneratorService.InitialGantry(imageData);
            var Drr =_drrGeneratorService.updateImgeData(0,0);
            _drrGeneratorService.show(Drr, renderWindowControl);
            
        }
        #endregion
        #region Read Dicoms
        private vtkImageData ReadDicomDirectory(string path)
        {
            vtkDICOMImageReader reader = new vtkDICOMImageReader();
            reader.SetDirectoryName(path);
            reader.Update();
            vtkImageData imageData = reader.GetOutput();
            var center = imageData.GetCenter();
            imageData.SetOrigin(-center[0], -center[1], -center[2]);//Move Images center into (0,0,0)
            return imageData;
        }
        #endregion
        #region Render Rotated DRR
        public void RenderDicom(vtkImageData imageData)
        {
            actor.SetInputData(imageData);
            actor.Update();

            renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().AddActor(actor);
            renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().Render();
            renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().ResetCamera();
            renderWindowControl.RenderWindow.Render();

        }
        #endregion

    }
}
