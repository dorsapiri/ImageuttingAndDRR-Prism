using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vtk_tester.Organ.OrganServices
{
    public class vtkCreateOrganImageDataRequestDto
    {
        public string RtStrctPath { get; set; }
        public int NumberOfSlices { get; set; }
        public double[] imageSpacing { get; set; }
        public double[] Origin { get; set; }
        public int[] ImageDimensions { get; set; }
        public double[] ImagePositionPatient { get; set; }
    }
}
