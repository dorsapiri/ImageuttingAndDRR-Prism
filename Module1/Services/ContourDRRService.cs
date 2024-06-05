using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Emgu.CV.Structure.MCvMatND;
using static Kitware.VTK.vtkFFT;
using TreatmentPlanningSystem.UI.VTK.Services.OrganServices;
using vtk_tester.Organ.OrganServices;
using Kitware.VTK;
using Module1.Services;

namespace OrganDRR_sample.Services
{
    public class ContourDRRService :IContourDRRService
    {
        #region Properties
        private int[] dimention;
        private double[] origin;
        private double[] spacing;
        private int[] extend;
        private double[] ImagePositionPatient = new double[3];
        private List<vtkCreateOrganImageDataResultDto> DrrImageData;
        private vtkImageData _imageData;
        private vtkActor actor;
        private RenderWindowControl _renderWindowControl;
        private vtkTransform transformPoly;
        private vtkImageReslice reslice;
        private int organNumber;
        private vtkTransformPolyDataFilter transformPolyDataFilter;
        private vtkPolyData contour;
        private vtkPolyDataMapper polyDataMapper;
        #endregion
        #region Method
        #region Constructor
        public ContourDRRService()
        {
            
            
        }
        #endregion
        public void initialProperties(vtkDICOMImageReader imageReader, string RTStructPath, RenderWindowControl renderWindowControl) 
        {
            transformPoly = new vtkTransform();
            _imageData = imageReader.GetOutput();
            dimention = _imageData.GetDimensions();
            origin = _imageData.GetOrigin();
            spacing = _imageData.GetSpacing();
            extend = _imageData.GetExtent();
            _renderWindowControl = renderWindowControl;
            int i = 0;
            foreach (var pos in imageReader.GetImagePositionPatient())
            {
                ImagePositionPatient[i] = pos;
                i++;
            }
            getOrganDRR(RTStructPath);
            Create();
        }
        #region Create
        private void Create()
        {
            int slicenumber = dimention[0] * dimention[1] / 10;
            reslice = vtkImageReslice.New();
            reslice.SetInputData(DrrImageData[organNumber].ContourData);
            reslice.SetOutputDimensionality(2);
            reslice.SlabTrapezoidIntegrationOn();
            reslice.SetSlabSliceSpacingFraction(2);
            reslice.InterpolateOn();
            reslice.SetSlabModeToMax();
            reslice.SetSlabNumberOfSlices(slicenumber);
            //Axis Rotation
            vtkTransform transformAxis = new();
            transformAxis.RotateX(-90);
            reslice.SetResliceAxes(transformAxis.GetMatrix());
            reslice.Update();

            vtkContourFilter contourFilter = new vtkContourFilter();
            contourFilter.SetInputConnection(reslice.GetOutputPort());
            contourFilter.SetValue(0, 200);
            contourFilter.GenerateValues(1, 50, 50);
            contourFilter.Update();

            contour = contourFilter.GetOutput();



            transformPolyDataFilter = new();
            transformPolyDataFilter.SetInputData(contour);
            transformPolyDataFilter.SetTransform(transformPoly);
            transformPolyDataFilter.Update();

            // Create a lookup table
            vtkColorTransferFunction colorTransferFunction = vtkColorTransferFunction.New();
            colorTransferFunction.AddRGBPoint(50, DrrImageData[organNumber].structureColors[0] / 255,
                DrrImageData[organNumber].structureColors[1] / 255,
                DrrImageData[organNumber].structureColors[2] / 255);

            polyDataMapper = new vtkPolyDataMapper();
            polyDataMapper.SetInputData(transformPolyDataFilter.GetOutput());
            polyDataMapper.SetLookupTable(colorTransferFunction);
            polyDataMapper.Update();

            actor = new();
            actor.SetMapper(polyDataMapper);
            
            
        }
        #endregion
        #region GetOrganDRR
        private void getOrganDRR(string RTStructPath)
        {
            
            vtkCreateOrganImageDataRequestDto dto = new vtkCreateOrganImageDataRequestDto();
            dto.RtStrctPath = RTStructPath;
            dto.Origin = origin;
            dto.ImagePositionPatient = ImagePositionPatient;
            dto.NumberOfSlices = dimention[2];
            dto.imageSpacing = spacing;
            dto.ImageDimensions = dimention;

            vtkCreateOrganImageDataService createOrganImageDataService = new vtkCreateOrganImageDataService();
            DrrImageData = createOrganImageDataService.Execute(dto);
        }
        #endregion
        public void setOrganNumber(int organN)
        {
            organNumber = organN;
            Create();
            Remove();
            Show();
        }
        #region makeCountour
        public void Update()
        {

            transformPolyDataFilter = new();
            transformPolyDataFilter.SetInputData(contour);
            transformPolyDataFilter.SetTransform(transformPoly);
            transformPolyDataFilter.Update();

            polyDataMapper.SetInputData(transformPolyDataFilter.GetOutput());
            polyDataMapper.Update();
            
            actor = new();
            actor.SetMapper(polyDataMapper);
        }
        #endregion
        #region SetGantryAngle
        public void setGantryAngle(int degree)
        {
            transformPoly = new();
            transformPoly.RotateY(degree);
            
        }

        #endregion

        #region SetTableAngle
        public void setTableAngle(int degree)
        {
            transformPoly.RotateZ(-degree);
        }

        #endregion
        #region Render
        public void Show()
        {
            _renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().AddActor(actor);
            _renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().ResetCamera();
            _renderWindowControl.RenderWindow.Render();
        }
        #endregion
        #region Remove
        public void Remove()
        {
            _renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().RemoveActor(actor);
            _renderWindowControl.RenderWindow.Render();
        }
        #endregion
        #endregion
    }
}
