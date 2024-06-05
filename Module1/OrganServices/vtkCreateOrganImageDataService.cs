using Emgu.CV;
using Emgu.CV.Structure;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Mat = Emgu.CV.Mat;
using System.Text;
using System.Threading.Tasks;
using EvilDICOM.CV.RT.Meta;
using Kitware.VTK;
//using TreatmentPlanningSystem.UI.VTK.Services.DTOs.Requests;
//using TreatmentPlanningSystem.UI.VTK.Services.DTOs;
using EvilDICOM.Core;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using vtk_tester.Organ.OrganServices;

namespace TreatmentPlanningSystem.UI.VTK.Services.OrganServices
{
    public class vtkCreateOrganImageDataService 
    {
        private readonly vtkRTStructReaderService _rtStructReaderService;
        public vtkCreateOrganImageDataService()
        {
             _rtStructReaderService = new();
        }

        public List<vtkCreateOrganImageDataResultDto> Execute(vtkCreateOrganImageDataRequestDto requestDto)
        {
            Dictionary<string , StructureMeta> Structs =  _rtStructReaderService.Execute(requestDto.RtStrctPath);

            List<vtkCreateOrganImageDataResultDto> Results = new();
            foreach (var Struct in Structs)
            {
                vtkCreateOrganImageDataResultDto resultDto = GetContourInfo(Struct.Value, requestDto);
                resultDto.ContourData = CreateData_Organ(GetContoursMat(Struct.Value, requestDto), resultDto.ListsliceContourZ, requestDto);

                Results.Add(resultDto);
            }

            return Results;
        }

         
        private List<byte[]> GetContoursMat(StructureMeta Contour, vtkCreateOrganImageDataRequestDto requestDto)
        {

            List<byte[]> PixelDatas = new List<byte[]>();
            foreach (SliceContourMeta? slice in Contour.SliceContours)
            {
                List<Point> points = new List<Point>();
                foreach (Point2f point in slice.ContourPoints)
                {
                    double x = point.X;
                    double y = point.Y;
                    points.Add(new Point(Math.Round((x - requestDto.ImagePositionPatient[0]) / requestDto.imageSpacing[0]), Math.Round((y - requestDto.ImagePositionPatient[1]) / requestDto.imageSpacing[1])));
                }

                var Mats = Mask(points, new System.Drawing.Size(requestDto.ImageDimensions[0], requestDto.ImageDimensions[1]));
                CvInvoke.Rotate(Mats, Mats, Emgu.CV.CvEnum.RotateFlags.Rotate180);
                CvInvoke.Flip(Mats, Mats, Emgu.CV.CvEnum.FlipType.Horizontal);


                //CvInvoke.Imwrite(@"C:\Users\FRPC\Desktop\aa\a.png", Mats.Last());

                byte[] PixelData = new byte[requestDto.ImageDimensions[0] * requestDto.ImageDimensions[1]];
                PixelData = Mats.ToImage<Gray, byte>().Bytes;
                PixelDatas.Add(PixelData);
            }

            return PixelDatas;
        }

        private Mat Mask(List<Point> StylusPoints, System.Drawing.Size MySize)
        {
            Image<Gray, byte> blackImg = new(MySize);
            System.Drawing.Point[] contours = new System.Drawing.Point[StylusPoints.Count];


            for (int c = 0; c < StylusPoints.Count; c++)
            {
                Point MyPoint = StylusPoints[c];
                contours[c] = new System.Drawing.Point(MyPoint.X, MyPoint.Y);
            }

            blackImg.Draw(contours, new Gray(100), -1);

            Image<Gray, byte> LineImg = new(MySize);
            VectorOfVectorOfPoint VVcontours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(blackImg, VVcontours, new Mat(), Emgu.CV.CvEnum.RetrType.Ccomp, ChainApproxMethod.ChainApproxNone);
            CvInvoke.DrawContours(LineImg.Mat, VVcontours, -1, new MCvScalar(100), 1);

            return blackImg.Mat + LineImg.Mat;
        }

        private vtkImageData CreateData_Organ(List<byte[]> PixelData, List<int> ListsliceContourZ, vtkCreateOrganImageDataRequestDto requestDto)
        {
            vtkImageData ContourData = new();

            ContourData.SetSpacing(requestDto.imageSpacing[0], requestDto.imageSpacing[1], requestDto.imageSpacing[2]);
            ContourData.SetDimensions(requestDto.ImageDimensions[0], requestDto.ImageDimensions[1], requestDto.NumberOfSlices);
            ContourData.SetOrigin(requestDto.Origin[0], requestDto.Origin[1], requestDto.Origin[2]);

            vtkShortArray scalarImageIntArray = new();
            scalarImageIntArray.SetNumberOfTuples(requestDto.ImageDimensions[0] * requestDto.ImageDimensions[1] * requestDto.NumberOfSlices);
            int indexOfPixel = 0;
            int sliceIndex = 0;

            for (int z = 0; z < ListsliceContourZ.Count; z++)
            {
                sliceIndex = ListsliceContourZ[z];
                if (sliceIndex >= 0)
                {
                    for (int y = requestDto.ImageDimensions[1] - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < requestDto.ImageDimensions[0]; x++)
                        {
                            indexOfPixel = (sliceIndex * requestDto.ImageDimensions[1] * requestDto.ImageDimensions[0]) + (y * requestDto.ImageDimensions[0]) + x;
                            if (PixelData[z][(y * requestDto.ImageDimensions[0]) + x] != 0)
                            {
                                scalarImageIntArray.SetTuple1(indexOfPixel, PixelData[z][(y * requestDto.ImageDimensions[0]) + x]);
                            }
                        }
                    }
                }
            }

            ContourData.GetPointData().SetScalars(scalarImageIntArray);

            scalarImageIntArray.Dispose();

            PixelData = null;

            return ContourData;
        }

        private vtkCreateOrganImageDataResultDto GetContourInfo(StructureMeta Contour, vtkCreateOrganImageDataRequestDto requestDto)
        {
            List<int> ListsliceContourZ = new List<int>();
            for (int j = 0; j < Contour.SliceContours.Count; j++)
            {
                ListsliceContourZ.Add((int)(Math.Abs(Contour.SliceContours[j].Z - (requestDto.ImagePositionPatient[2] + ((requestDto.ImageDimensions[2] - 1) * requestDto.imageSpacing[2]))) / requestDto.imageSpacing[2]));
            }

            return new()
            {
                contourName = Contour.StructureName,
                structureColors = new double[3] { Contour.Color[0], Contour.Color[1], Contour.Color[2] },
                ListsliceContourZ = ListsliceContourZ,
            };
        }

    }
}
