using Kitware.VTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module1.Services
{
    public class IsoCenterService : IIsoCenterService
    {
        private vtkSphereSource IsoCenterPoint;
        private double[] iso;
        private int slabNumberOfSlices;
        private vtkMatrix4x4 isoTransformMatrix;
        private vtkImageActor isoActor;
        private vtkImageData IsoCenterImage;

        public IsoCenterService() 
        {
            iso = new double[3];
            isoTransformMatrix = new vtkMatrix4x4();
        }
        public void Create()
        {
            IsoCenterPoint = new();
            IsoCenterPoint.SetRadius(10);
            IsoCenterPoint.SetCenter(0, 0, 0);
            IsoCenterPoint.Update();

            // Convert the polydata (sphere) to an image data
            vtkPolyDataToImageStencil polyDataToImageStencil = vtkPolyDataToImageStencil.New();
            polyDataToImageStencil.SetInputData(IsoCenterPoint.GetOutput());
            polyDataToImageStencil.Update();

            vtkImageData imageData = vtkImageData.New();
            imageData.SetSpacing(0.1, 0.1, 0.1);
            imageData.SetOrigin(-5, -5, -5);
            

            vtkImageStencil imageStencil = vtkImageStencil.New();
            imageStencil.SetInputData(imageData);
            imageStencil.SetStencilConnection(polyDataToImageStencil.GetOutputPort());
            imageStencil.ReverseStencilOff();
            imageStencil.SetBackgroundValue(0);
            imageStencil.Update();

            IsoCenterImage = imageStencil.GetOutput();
        }
        public void IsoCenterDRR() 
        {
            

            vtkImageReslice IsoReslice = new();
            IsoReslice.SetInputData(IsoCenterImage);
            IsoReslice.SetOutputDimensionality(2);
            IsoReslice.SlabTrapezoidIntegrationOn();
            IsoReslice.SetSlabSliceSpacingFraction(2);
            IsoReslice.InterpolateOn();
            IsoReslice.SetSlabModeToMean();
            IsoReslice.SetSlabNumberOfSlices(slabNumberOfSlices);
            IsoReslice.SetResliceAxes(isoTransformMatrix);
            IsoReslice.Update();

            //vtkImageMapper isoMapper = new();
            //isoMapper.SetInputData(IsoReslice.GetOutput());
            //isoMapper.Update();

            isoActor = new();
            isoActor.SetInputData(IsoReslice.GetOutput());
        }
        public void Update(double[] isocenter, int slabnumber,vtkMatrix4x4 transformMatrix)
        {
            iso = isocenter;
            slabNumberOfSlices = slabnumber;
            isoTransformMatrix = transformMatrix;
            IsoCenterDRR();
        }
        public void show(RenderWindowControl rwc)
        {
            rwc.RenderWindow.GetRenderers().GetFirstRenderer().AddActor(isoActor);
        }
    }
}
