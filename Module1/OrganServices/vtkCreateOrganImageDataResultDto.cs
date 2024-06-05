using Kitware.VTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vtk_tester.Organ.OrganServices
{
    public class vtkCreateOrganImageDataResultDto
    {
        public vtkImageData ContourData { get; set; }
        public List<int> ListsliceContourZ { get; set; }
        public double[] structureColors { get; set; }
        public string contourName { get; set; }
    }
}
