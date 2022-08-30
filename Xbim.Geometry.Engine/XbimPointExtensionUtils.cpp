#include "XbimPointExtensionUtils.h"


namespace Xbim
{
    namespace Geometry
    {
        double XbimPointExtensionUtils::PointDistance(XbimPoint3D pt1, XbimPoint3D pt2)
        {
            return sqrt(pow((pt2.X - pt1.X), 2) + pow((pt2.Y - pt1.Y), 2) + pow((pt2.Y - pt1.Y), 2));
        }
    }
}