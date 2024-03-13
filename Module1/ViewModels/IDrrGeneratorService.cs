using EvilDICOM.CV.RT.Meta;
using Kitware.VTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module1.ViewModels
{
    public interface IDrrGeneratorService
    {
        vtkImageData InitialGantry(vtkImageData imageData);
        vtkImageData updateImgeData(int GantryAngel, int TableAngle);
        void setGantryAngle(int degree);
        void setTableAngle(int degree);
        void show(vtkImageData imageData, RenderWindowControl renderWindowControl);
        void removeActor(vtkActor actor);
        void updateRenderer();
        //vtkImageData transformedImageData { get; set; }
    }
}
