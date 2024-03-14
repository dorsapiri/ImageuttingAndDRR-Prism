using Kitware.VTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Module1.ViewModels
{
    public class DrrGeneratorService : IDrrGeneratorService
    {
        #region Properties
        private vtkTransform transformAxis;
        private vtkImageReslice reslice;
        public vtkImageData transformedImageData { get; set; }
        vtkImageActor actor;
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
        #endregion
        #region Methodes
        #region Constructor
        public DrrGeneratorService()
        {
            actor = new();
            reslice = new vtkImageReslice();
            transformAxis = new vtkTransform();
        }
        #endregion
        #region UpdateImageDate
        public void updateImgeData()
        {
            int slabNumberOfSlices = transformedImageData.GetDimensions()[0] * (int)transformedImageData.GetSpacing()[2];

            reslice.SetInputData(transformedImageData);
            reslice.SetOutputDimensionality(2);
            reslice.SlabTrapezoidIntegrationOn();
            reslice.SetSlabSliceSpacingFraction(2);
            reslice.InterpolateOn();
            reslice.SetSlabModeToMean();
            reslice.SetSlabNumberOfSlices(slabNumberOfSlices);
            reslice.SetResliceAxes(transformAxis.GetMatrix());
            reslice.Update();

            actor.SetInputData(setLookupTable(reslice.GetOutput()));
            actor.Update();
        }
        #endregion
        #region SetGantryAngle
        public void setGantryAngle(int degree)
        {
            
            transformAxis.RotateY(degree);
            reslice.SetResliceAxes(transformAxis.GetMatrix());
            reslice.Update();
        }
        #endregion
        #region SetTableAngle
        public void setTableAngle(int degree)
        {
            transformAxis = new vtkTransform();
            transformAxis.RotateZ(-degree);
            reslice.SetResliceAxes(transformAxis.GetMatrix());
            reslice.Update();
        }
        #endregion
        #region Show
        public void show(RenderWindowControl renderWindowC)
        {
            renderWindowControl = renderWindowC;
            renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().AddActor(actor);
            renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().Render();
            renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().ResetCamera();
            renderWindowControl.RenderWindow.Render();
        }
        #endregion
        #region RemoveActor
        public void removeActor()
        {
            renderWindowControl.RenderWindow.GetRenderers().GetFirstRenderer().RemoveActor(actor);
        }
        #endregion
        #region UpdateRenderer
        public void updateRenderer()
        {
            actor.SetInputData(setLookupTable(reslice.GetOutput()));
            actor.Update();
            renderWindowControl.RenderWindow.Render();
        }
        #endregion
        #region Initial Gantry
        public void InitialGantry(vtkImageData imageData)
        {
            //Rotate ImageData
            vtkTransform transform = new();
            transform.RotateX(-90);

            vtkImageReslice resliceInit = new();
            resliceInit.SetInputData(imageData);
            resliceInit.SetResliceTransform(transform);
            resliceInit.SetResliceAxesOrigin(0, 0, 0);
            resliceInit.Update();

            transformedImageData = resliceInit.GetOutput();

        }
        #endregion
        #region SetLookupTable
        private vtkImageData setLookupTable(vtkImageData imageData) 
        {
            double dWindow = 400;
            double dLevel = 0;
            double[] dRange = imageData.GetScalarRange();

            // Create a lookup table
            vtkLookupTable lookupTable = new vtkLookupTable();
            lookupTable.SetNumberOfTableValues(256); // Set the number of colors
            lookupTable.SetValueRange(0, 1);
            lookupTable.SetSaturationRange(0, 0);
            lookupTable.SetRange(dRange[0], dRange[1]);
            lookupTable.SetHueRange(0, 1);
            lookupTable.SetRampToLinear();
            lookupTable.Build(); 

            // Apply the lookup table to the image data
            vtkImageMapToColors mapColors = new vtkImageMapToColors();
            mapColors.SetLookupTable(lookupTable);
            mapColors.SetInputData(imageData);
            mapColors.Update();

            return mapColors.GetOutput();
        }
        #endregion
        #endregion
    }
}
