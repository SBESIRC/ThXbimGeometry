#include "XbimOccShape.h"
#include "XbimFaceSet.h"
#include "XbimShell.h"
#include "XbimSolid.h"
#include "XbimCompound.h"
#include "XbimPoint3DWithTolerance.h"
#include "XbimConvert.h"
#include <BRepCheck_Analyzer.hxx>
#include <BRepMesh_IncrementalMesh.hxx>
#include <Poly_Triangulation.hxx>
#include <TShort_Array1OfShortReal.hxx> 
#include <BRep_Tool.hxx>
#include <Poly.hxx>
#include <BRepBuilderAPI_Transform.hxx>
#include <BRepTools.hxx>
#include <Bnd_Box.hxx>
#include <BRepBndLib.hxx>
#include <gp_Quaternion.hxx>
#include "XbimWire.h"
#include <TopExp.hxx>
#include <Geom_TrimmedCurve.hxx>
#include <Geom_Line.hxx>
#include <BRepTools_WireExplorer.hxx>
#include <BRepBuilderAPI_GTransform.hxx>
#include <BRepBuilderAPI_Transform.hxx>
#include <Geom_Plane.hxx>

using namespace System::Threading;
using namespace System::Collections::Generic;


namespace Xbim
{
	namespace Geometry
	{
		XbimOccShape::XbimOccShape()
		{
		}



		void XbimOccShape::WriteTriangulation(TextWriter^ textWriter, double tolerance, double deflection, double angle)
		{

		}

		void XbimOccShape::WriteTriangulation(IXbimMeshReceiver^ meshReceiver, double tolerance, double deflection, double angle)
		{
		

		}






		void XbimOccShape::WriteIndex(BinaryWriter^ bw, UInt32 index, UInt32 maxInt)
		{
			if (maxInt <= 0xFF)
				bw->Write((unsigned char)index);
			else if (maxInt <= 0xFFFF)
				bw->Write((UInt16)index);
			else
				bw->Write(index);
		}

		void XbimOccShape::WriteTriangulation(BinaryWriter^ binaryWriter, double tolerance, double deflection, double angle)
		{

		}
	}
}
