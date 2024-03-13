using EvilDICOM.CV.RT.Meta;
using Kitware.VTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module1.ViewModels
{
    internal interface IRemoveTableService
    {
        vtkImageData Execute(vtkImageData imageData, StructureMeta structureMeta);
    }
}
