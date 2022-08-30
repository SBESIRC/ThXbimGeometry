#include "XbimGeometryUtils.h"
#include "XbimFaceSet.h"
#include <GeomAPI_IntCS.hxx>

#include <BRepLib.hxx>
#include<GeomAdaptor_Curve.hxx>
#include <Geom_Line.hxx>

namespace Xbim
{
    namespace Geometry
    {
        bool XbimGeometryUtils::FaceIntersects(IXbimFace^ face, IXbimFace^ otherFace)
        {   
            XbimFace^ xFace = dynamic_cast<XbimFace^>(face);
            XbimFace^ xOtherFace = dynamic_cast<XbimFace^>(otherFace);

            BOPAlgo_Section build;
            build.AddArgument(xFace);
            build.AddArgument(xOtherFace);
            build.Perform(); 
            BOPDS_DS* ds = build.PDS();
            BOPDS_VectorOfInterfEE& ee = ds->InterfEE();
            return ee.Length() > 0;
        }

        IXbimFace^ XbimGeometryUtils::FaceUnion(IXbimFace^ face, IXbimFace^ otherFace)
        {
            XbimFace^ xFace = dynamic_cast<XbimFace^>(face);
            XbimFace^ xOtherFace = dynamic_cast<XbimFace^>(otherFace);

            BRepAlgoAPI_Fuse boolOp(xFace, xOtherFace);
            bool s = boolOp.HasErrors() == Standard_False;
            TopoDS_Shape ts = boolOp.Shape(); 
            TopAbs_ShapeEnum type = ts.ShapeType();
            TopoDS_Face tFace = TopoDS::Face(boolOp.Shape());
            XbimFace^ resFace = gcnew XbimFace(tFace);
            return resFace;
        }

        IXbimFace^ XbimGeometryUtils::FaceUnion(List<IXbimFace^>^ faceSet)
        {
            return FaceUnion(faceSet[0], faceSet[1]);
        }

        void XbimGeometryUtils::CurveProjectToSurface(XbimPoint3D sp, XbimPoint3D ep, IXbimFace^ face)
        {
            auto aPnt1 = gp_Pnt(sp.X, sp.Y, sp.Z);
            auto aPnt2 = gp_Pnt(ep.X, ep.Y, ep.Z);
            auto aEdge1 = BRepBuilderAPI_MakeEdge(aPnt1, aPnt2).Edge();
            Standard_Real f, l;
            auto aCurve = BRep_Tool::Curve(aEdge1, f, l);
            auto adaptorCurve = GeomAdaptor_Curve(aCurve, f, l);
            auto tempCurve = adaptorCurve.Curve();
            
            XbimFace^ xFace = dynamic_cast<XbimFace^>(face);
            Handle(Geom_Plane) aPlane = Handle(Geom_Plane)::DownCast(BRep_Tool::Surface(xFace));
            gp_Dir dir(0, 0, 1);
            Handle(Geom_Curve) resCurve = GeomProjLib::ProjectOnPlane(tempCurve, aPlane, dir, Standard_True);
            
            Handle(Geom_Curve) curve1 = GeomProjLib::Project(tempCurve, BRep_Tool::Surface(xFace));
            GeomAPI_IntCS Intersector(resCurve, BRep_Tool::Surface(xFace));
            auto nums = Intersector.NbPoints();

            //auto aPnt11 = gp_Pnt((sp.X + ep.X)/ 2, (sp.Y + ep.Y) / 2, (sp.Z + ep.Z) / 2);
            auto aPnt11 = gp_Pnt(sp.X, sp.Y, sp.Z);
            auto aPnt21 = gp_Pnt(aPnt11.X(), aPnt11.Y(), aPnt11.Z() + 5);
            auto aEdge11 = BRepBuilderAPI_MakeEdge(aPnt11, aPnt21).Edge();
            Standard_Real f1, l1;
            auto aCurve1 = BRep_Tool::Curve(aEdge11, f1, l1);
            auto adaptorCurve1 = GeomAdaptor_Curve(aCurve, f1, l1);
            auto tempCurve1 = adaptorCurve1.Curve();

            GeomAPI_IntCS Intersector1(tempCurve1, BRep_Tool::Surface(xFace));
            auto nums1 = Intersector1.NbPoints();
        }
    }
}