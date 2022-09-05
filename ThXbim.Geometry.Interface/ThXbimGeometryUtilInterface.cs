using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Geometry;

namespace ThXbim.Geometry.Interface
{
    public interface ThXbimGeometryUtilInterface
    {
        bool FaceIntersects(IXbimFace face, IXbimFace otherFace);
        IXbimFace FaceUnion(IXbimFace face, IXbimFace otherFace);
        IXbimFace FaceUnion(List<IXbimFace> faceSet);
        List<IXbimEdge> CurveProjectToSurface(XbimPoint3D sp, XbimPoint3D ep, List<IXbimFace> face);
    }
}
