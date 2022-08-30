using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Geometry;

namespace ThXbim.Geometry.Interface
{
    public interface XbimPointExtensionInterface
    {
        double PointDistance(XbimPoint3D pt1, XbimPoint3D pt2);
    }
}
