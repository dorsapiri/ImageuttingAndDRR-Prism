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
        void InitialGantry(vtkImageData imageData);
        void updateImgeData();
        void setGantryAngle(int degree);
        void setTableAngle(int degree);
        void show(RenderWindowControl renderWindowControl);
        void removeActor();
        void updateRenderer();
    }
}
