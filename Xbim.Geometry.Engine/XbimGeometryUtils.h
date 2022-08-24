#pragma once
#include "XbimOccShape.h"
#include "XbimWire.h"
#include "XbimWireSet.h"
#include <TopoDS_Face.hxx>
#include <BRepBuilderAPI_FaceError.hxx>
#include <Geom_Surface.hxx>
#include <TColgp_SequenceOfPnt.hxx>
#include <TopTools_DataMapOfIntegerShape.hxx>
#include <vector>
#include "XbimFace.h"
#include <BRepAlgoAPI_Common.hxx>
#include <BRepAlgoAPI_Fuse.hxx>
#include <BOPAlgo_Section.hxx>
#include <BOPDS_DS.hxx>

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
		};
	}
}

