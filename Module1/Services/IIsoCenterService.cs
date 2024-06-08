using Kitware.VTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module1.Services
{
    public interface IIsoCenterService
    {
        public void Create();
        public void IsoCenterDRR();
        public void Update(double[] isocenter, int slabnumber, vtkMatrix4x4 transformMatrix);
        public void show(RenderWindowControl rwc);
    }
}
