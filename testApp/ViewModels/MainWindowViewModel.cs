using Kitware.VTK;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Forms.Integration;
using System.Windows;
using testApp.Views;
using System.ComponentModel;
using Module1.ViewModels;
using Module1.Services;

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
        private IContourDRRService  _contourDRRService;

        public IContourDRRService contourDRRService
        {
            get { return _contourDRRService; }
            set { SetProperty(ref _contourDRRService, value); }
        }
        private IIsoCenterService _isoCenterService;

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
        public MainWindowViewModel(IRegionManager regionManager,IDrrGeneratorService drrGeneratorService,IContourDRRService contourDRRService, IIsoCenterService isoCenterService)
        {
            _regionManager = regionManager;
            actor = new();
            _drrGeneratorService = drrGeneratorService;
            _contourDRRService = contourDRRService;
            _isoCenterService = isoCenterService;

            regionManager.RegisterViewWithRegion("ContentRegionDrrRotation", typeof(DrrTransformView));
            regionManager.RegisterViewWithRegion("ContentRegionOrganDrr", typeof(OrgansCheckListView));
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
            //string directoryPath = @"C:\Users\k703528\Documents\Study_1 _ CompleteS";
            //string directoryPath = @"C:\Users\k703528\Desktop\DICOMs\A990519-BREAST_KHANSARI^FORUGH\Study_1";
            //string directoryPath = @"C:\Users\k703528\Desktop\DICOMs\AE990615-1SADEGH\Study_1";
            //string directoryPath = @"C:\Users\k703528\Desktop\DICOMs\AE990505-1TABAR\Study_1";
            //string directoryPath = @"C:\Users\k703528\Desktop\DICOMs\A990613-GBM\Study_1";
            //string directoryPath = @"C:\Users\k703528\Desktop\DICOMs\A990610-BREAST2\Study_1";
            //string directoryPath = @"C:\Users\k703528\Desktop\DICOMs\A990515_ABDOLAHI  AZAM\Study_1";
            string directoryPath = @"C:\Users\k703528\Desktop\DICOMs\AE990507-1VAZIRI\Study_1";
            string RTStruct = @"C:\Users\k703528\Desktop\DICOMs\AE990507-1VAZIRI\Study_1\VAZIRI^AAZAM__RTS.dcm";
            var reader = ReadDicomDirectory(directoryPath);
            var imageData = CreateImagedata(reader);
            _drrGeneratorService.InitialGantry(imageData);
            _drrGeneratorService.updateImgeData();
            _drrGeneratorService.show(renderWindowControl);
            _contourDRRService.initialProperties(reader,RTStruct,renderWindowControl);
            double[] iso = new double[3] { 0, 0, 0 };
            _isoCenterService.Create();
            _isoCenterService.Update(iso,_drrGeneratorService.getSlabNumber(),_drrGeneratorService.getMatrixTransform());
            _isoCenterService.show(renderWindowControl);
        }
        #endregion
        #region Read Dicoms
        private vtkDICOMImageReader ReadDicomDirectory(string path)
        {
            vtkDICOMImageReader reader = new vtkDICOMImageReader();
            reader.SetDirectoryName(path);
            reader.Update();
            return reader;
        }
        #endregion
        #region imageData
        private vtkImageData CreateImagedata(vtkDICOMImageReader reader)
        {
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
