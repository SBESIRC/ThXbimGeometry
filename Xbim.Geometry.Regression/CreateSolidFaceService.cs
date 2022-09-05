using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Geometry;
using Xbim.Geometry.Engine.Interop;
using Xbim.Ifc;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.TopologyResource;
using XbimRegression.CreateIFCService;

namespace XbimRegression
{
    public class CreateSolidFaceService
    {
        public void CreateFaces(IfcStore model, IfcBuildingElement element, IfcBuildingElement roof)
        {
            var allPolyLoop = new List<IfcPolyLoop>();
            foreach (var rep in element.Representation.Representations)
            {
                foreach (var item in rep.Items)
                {
                    foreach (var face in ((IfcManifoldSolidBrep)item).Outer.CfsFaces)
                    {
                        foreach (var bound in face.Bounds)
                        {
                            allPolyLoop.Add((IfcPolyLoop)bound.Bound);
                        }
                    }
                }
            }

            var allRoofPolyLoop = new List<IfcPolyLoop>();
            foreach (var rep in roof.Representation.Representations)
            {
                foreach (var item in rep.Items)
                {
                    foreach (var face in ((IfcManifoldSolidBrep)item).Outer.CfsFaces)
                    {
                        foreach (var bound in face.Bounds)
                        {
                            allRoofPolyLoop.Add((IfcPolyLoop)bound.Bound);
                        }
                    }
                }
            }

            FilterFace filterFace = new FilterFace();
            var faceLst = CreateOCCTElementFace(allPolyLoop);
            var beamPts = filterFace.FilterBeamFace(faceLst);     //获取梁面点

            var roofFaceElm = CreateOCCTElementFace(allRoofPolyLoop);
            var roofFaces = filterFace.FilterRoofFace(roofFaceElm);     //获取屋面面

            FollowTheSlopeService followTheSlopeService = new FollowTheSlopeService(beamPts, roofFaces);
            var beam = followTheSlopeService.FollowSlope();

            CreateBeamService.CreateIFCBeamInIfc(model, beam);
        }

        private List<List<IXbimFace>> CreateOCCTElementFace(List<IfcPolyLoop> ifcPolyLoops)
        {
            var xbimfaceLst = CreateOCCTTriangleFace(ifcPolyLoops);
            var faceLst = ClassifyTriangle(xbimfaceLst);

            return faceLst;
        }

        /// <summary>
        /// 将ifc的polyloop转化为Xbimface
        /// </summary>
        /// <param name="ifcPolyLoops"></param>
        /// <returns></returns>
        private List<IXbimFace> CreateOCCTTriangleFace(List<IfcPolyLoop> ifcPolyLoops)
        {
            var allFaces = new List<IXbimFace>();
            foreach (var loop in ifcPolyLoops)
            {
                XbimGeometryEngine xbimGeometryEngine = new XbimGeometryEngine();
                var face = xbimGeometryEngine.CreateFace(loop, null); 
                allFaces.Add(face);
            }

            return allFaces;
        }

        private List<List<IXbimFace>> ClassifyTriangle(List<IXbimFace> faces)
        {
            Dictionary<XbimVector3D, List<IXbimFace>> faceGroup = new Dictionary<XbimVector3D, List<IXbimFace>>();
            foreach (var face in faces)
            {
                var dir = face.Normal;
                var dirKeys = faceGroup.Keys.Where(x => x.IsEqual(dir, 0.001) || x.IsEqual(dir.Negated(), 0.001)).ToList();
                if (dirKeys.Count > 0)
                {
                    faceGroup[dirKeys.First()].Add(face);
                }
                else
                {
                    faceGroup.Add(dir, new List<IXbimFace>() { face });
                }
            }

            List<List<IXbimFace>> lineStrGroup = new List<List<IXbimFace>>();
            ThXbimGeometryUtils thXbimGeometryUtils = new ThXbimGeometryUtils();
            foreach (var group in faceGroup)
            {
                var groupLineStrings = group.Value.ToList();
                List<IXbimFace> faceLst = new List<IXbimFace>();
                while (groupLineStrings.Count > 0)
                {
                    if (faceLst.Count <= 0)
                    {
                        var firLinestring = groupLineStrings.First();
                        groupLineStrings.Remove(firLinestring);
                        faceLst.Add(firLinestring);
                    }

                    var closeLines = groupLineStrings.Where(x => faceLst.Any(y => thXbimGeometryUtils.FaceIntersects(x, y))).ToList();
                    if (closeLines.Count <= 0)
                    {
                        lineStrGroup.Add(faceLst);
                        faceLst = new List<IXbimFace>();
                        continue;
                    }
                    groupLineStrings = groupLineStrings.Except(closeLines).ToList();
                    faceLst.AddRange(closeLines);
                }
                lineStrGroup.Add(faceLst);
            }

            return lineStrGroup;
        }
    }
}
