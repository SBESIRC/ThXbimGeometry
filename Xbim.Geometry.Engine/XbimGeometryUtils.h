#pragma once
#include "XbimFace.h"
#include <BRepAlgoAPI_Fuse.hxx>
#include <BOPAlgo_Section.hxx>	//两个section游什么区别
#include <BRepAlgoAPI_Section.hxx>
#include <BOPDS_DS.hxx>
#include <GeomProjLib.hxx>
#include <Geom_Plane.hxx>
#include <BRepBuilderAPI_MakePolygon.hxx>
#include <BRepBuilderAPI_MakeFace.hxx>
#include "XbimPointExtensionUtils.h"

using namespace System::Collections::Generic;
using namespace ThXbim::Geometry::Interface;

namespace Xbim
{
	namespace Geometry
	{
		ref class XbimGeometryUtils : ThXbimGeometryUtilInterface
		{
		public:
			virtual bool FaceIntersects(IXbimFace^ face, IXbimFace^ otherFace);
			virtual IXbimFace^ FaceUnion(IXbimFace^ face, IXbimFace^ otherFace);
			virtual IXbimFace^ FaceUnion(List<IXbimFace^>^ faceSet);
			virtual List<IXbimEdge^>^ CurveProjectToSurface(XbimPoint3D sp, XbimPoint3D ep, List<IXbimFace^>^ faces);
		private:
			List<IXbimEdge^>^ CreateCompleteBeam(List<IXbimEdge^>^ edges);
			IXbimEdge^ MergeBeamLines(List<IXbimEdge^>^ edges);
		};
	}
}

