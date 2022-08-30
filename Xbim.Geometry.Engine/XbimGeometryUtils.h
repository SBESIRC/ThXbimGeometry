#pragma once
#include "XbimFace.h"
#include <BRepAlgoAPI_Fuse.hxx>
#include <BOPAlgo_Section.hxx>
#include <BOPDS_DS.hxx>
#include <GeomProjLib.hxx>
#include <Geom_Plane.hxx>

using namespace System::Collections::Generic;
using namespace ThXbim::Geometry::Interface;

namespace Xbim
{
	namespace Geometry
	{
		ref class XbimGeometryUtils :ThXbimGeometryUtilInterface
		{
		public:
			virtual bool FaceIntersects(IXbimFace^ face, IXbimFace^ otherFace);
			virtual IXbimFace^ FaceUnion(IXbimFace^ face, IXbimFace^ otherFace);
			virtual IXbimFace^ FaceUnion(List<IXbimFace^>^ faceSet);
			virtual void CurveProjectToSurface(XbimPoint3D sp, XbimPoint3D ep, IXbimFace^ face);
		};
	}
}

