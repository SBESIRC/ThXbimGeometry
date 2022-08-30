#pragma once
#include <math.h>

using namespace System::Collections::Generic;
using namespace ThXbim::Geometry::Interface;
using namespace Xbim::Common::Geometry;

namespace Xbim
{
    namespace Geometry
    {
        ref class XbimPointExtensionUtils : XbimPointExtensionInterface
        {
        public:
            // ͨ�� XbimPointExtensionInterface �̳�
            virtual double PointDistance(XbimPoint3D pt1, XbimPoint3D pt2);
        };
    }
}

