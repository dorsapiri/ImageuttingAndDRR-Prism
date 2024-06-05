using Kitware.VTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module1.Services
{
    public interface IContourDRRService
    {
        
        public void setOrganNumber(int organN);
        public void Update();
        public void setGantryAngle(int degree);
        public void setTableAngle(int degree);
        public void Show();
        public void Remove();
        public void initialProperties(vtkDICOMImageReader imageReader, string RTStructPath, RenderWindowControl renderWindowControl);


    }
}
