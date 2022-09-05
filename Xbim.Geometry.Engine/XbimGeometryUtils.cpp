#include "XbimGeometryUtils.h"
#include "XbimFaceSet.h"
#include <GeomAPI_IntCS.hxx>

#include <BRepLib.hxx>
#include<GeomAdaptor_Curve.hxx>
#include <Geom_Line.hxx>
#include <GeomAPI_ExtremaCurveCurve.hxx>
#include <BRep_Builder.hxx>

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

        List<IXbimEdge^>^ XbimGeometryUtils::CurveProjectToSurface(XbimPoint3D sp, XbimPoint3D ep, List<IXbimFace^>^ faces)
        {
            List<IXbimEdge^>^ resEdges = gcnew List<IXbimEdge^>();
            for each (IXbimFace ^ face in faces)
            {
                double maxZValue = DBL_MIN;
                XbimFace^ xFace = dynamic_cast<XbimFace^>(face);
                for each (XbimPoint3D pt in  xFace->OuterBound->Points)
                {
                    if (maxZValue < pt.Z)
                    {
                        maxZValue = pt.Z;
                    }
                }
                maxZValue = maxZValue + 100;

                auto pt1 = gp_Pnt(sp.X, sp.Y, sp.Z);
                auto pt2 = gp_Pnt(ep.X, ep.Y, ep.Z);
                auto pt3 = gp_Pnt(ep.X, ep.Y, ep.Z + maxZValue);
                auto pt4 = gp_Pnt(sp.X, sp.Y, sp.Z + maxZValue);
                //auto beamEdge = BRepBuilderAPI_MakeEdge(pt1, pt2).Edge();
                BRepBuilderAPI_MakePolygon maker(pt1, pt2, pt3, pt4, Standard_True);
                BRepBuilderAPI_MakeFace faceMaker(maker.Wire());
                BRepAlgoAPI_Section section(xFace, faceMaker.Face());
                for (TopExp_Explorer explorer(section.Shape(), TopAbs_EDGE); explorer.More(); explorer.Next())
                {
                    TopoDS_Shape shape = explorer.Value();
                    TopoDS_Edge edge = TopoDS::Edge(shape);
                    XbimEdge^ resEdge = gcnew XbimEdge(edge);
                    resEdges->Add(resEdge);
                }
            }

            return CreateCompleteBeam(resEdges);
        }

        List<IXbimEdge^>^ XbimGeometryUtils::CreateCompleteBeam(List<IXbimEdge^>^ edges)
        {
            List<List<IXbimEdge^>^>^ groupEdges = gcnew List<List<IXbimEdge^>^>();
            List<IXbimEdge^>^ completeEdges = gcnew List<IXbimEdge^>();
            while (edges->Count > 0)
            {
                if (completeEdges->Count == 0)
                {
                    auto firEdgeLst = edges[0];
                    edges->Remove(firEdgeLst);
                    completeEdges->Add(firEdgeLst);
                }
                auto completeEdge = MergeBeamLines(completeEdges);
                auto fSp = completeEdge->EdgeStart->VertexGeometry;
                auto fEp = completeEdge->EdgeEnd->VertexGeometry;
                auto fDir = (fEp - fSp).Normalized();
                bool change = false;
                for each (auto e in edges)
                {
                    auto eSp = e->EdgeStart->VertexGeometry;
                    auto eEp = e->EdgeEnd->VertexGeometry;
                    auto eDir = (eEp - eSp).Normalized();
                    if (((abs(eSp.X - fSp.X) < 0.01 && abs(eSp.Y - fSp.Y) < 0.01 && abs(eSp.Z - fSp.Z) < 0.01) ||
                        (abs(eSp.X - fEp.X) < 0.01 && abs(eSp.Y - fEp.Y) < 0.01 && abs(eSp.Z - fEp.Z) < 0.01) ||
                        (abs(eEp.X - fSp.X) < 0.01 && abs(eEp.Y - fSp.Y) < 0.01 && abs(eEp.Z - fSp.Z) < 0.01) ||
                        (abs(eEp.X - fEp.X) < 0.01 && abs(eEp.Y - fEp.Y) < 0.01 && abs(eEp.Z - fEp.Z) < 0.01)) &&
                        fDir.IsParallel(eDir, 0.0001))
                    {
                        completeEdges->Add(e);
                        change = true;
                    }
                }
                for each (auto e in completeEdges)
                {
                    edges->Remove(e);
                }
                if (!change)
                {
                    //auto tempEdges = gcnew List<IXbimEdge^>(completeEdges);
                    groupEdges->Add(completeEdges);
                    completeEdges = gcnew List<IXbimEdge^>();
                }
            }
            groupEdges->Add(completeEdges);
            List<IXbimEdge^>^ resEdges = gcnew List<IXbimEdge^>();
            for each (auto cEdges in groupEdges)
            {
                resEdges->Add(MergeBeamLines(cEdges));
            }
            return resEdges;
        }

        /// <summary>
        /// 合并beamline为一条线
        /// </summary>
        /// <param name="edges"></param>
        /// <returns></returns>
        IXbimEdge^ XbimGeometryUtils::MergeBeamLines(List<IXbimEdge^>^ edges)
        {
            List<XbimPoint3D>^ ptLst = gcnew List<XbimPoint3D>();
            for each (IXbimEdge ^ edge in edges)
            {
                ptLst->Add(edge->EdgeStart->VertexGeometry);
                ptLst->Add(edge->EdgeEnd->VertexGeometry);
            }
            auto firPt = ptLst[0];
            XbimPoint3D sp = firPt;
            XbimPointExtensionUtils pDistance;
            double dis = pDistance.PointDistance(firPt, sp);
            for each (auto pt in ptLst)
            {
                double ptDis = pDistance.PointDistance(firPt, pt);
                if (ptDis > dis)
                {
                    dis = ptDis;
                    sp = pt;
                }
            }

            XbimPoint3D ep = sp;
            double endDis = pDistance.PointDistance(ep, sp);
            for each (auto pt in ptLst)
            {
                double ptDis = pDistance.PointDistance(sp, pt);
                if (ptDis > endDis)
                {
                    endDis = ptDis;
                    ep = pt;
                }
            }

            auto pt1 = gp_Pnt(sp.X, sp.Y, sp.Z);
            auto pt2 = gp_Pnt(ep.X, ep.Y, ep.Z);
            auto beamEdge = BRepBuilderAPI_MakeEdge(pt1, pt2).Edge();
            XbimEdge^ resEdge = gcnew XbimEdge(beamEdge);
            return resEdge;
        }

#pragma region  test code
        //void XbimGeometryUtils::CurveProjectToSurface(XbimPoint3D sp, XbimPoint3D ep, IXbimFace^ face)
        //{
        //    double maxZValue = DBL_MIN;
        //    XbimFace^ xFace = dynamic_cast<XbimFace^>(face);
        //    for each (XbimPoint3D pt in  xFace->OuterBound->Points)
        //    {
        //        if (maxZValue < pt.Z)
        //        {
        //            maxZValue = pt.Z;
        //        }
        //    }
        //    maxZValue = maxZValue + 100;

        //    auto pt1 = gp_Pnt(sp.X, sp.Y, sp.Z);
        //    auto pt2 = gp_Pnt(ep.X, ep.Y, ep.Z);
        //    auto pt3 = gp_Pnt(ep.X, ep.Y, ep.Z + maxZValue);
        //    auto pt4 = gp_Pnt(sp.X, sp.Y, sp.Z + maxZValue);
        //    auto beamEdge = BRepBuilderAPI_MakeEdge(pt1, pt2).Edge();
        //    BRepBuilderAPI_MakePolygon maker(pt1, pt2, pt3, pt4, Standard_True);
        //    BRepBuilderAPI_MakeFace faceMaker(maker.Wire());
        //    BRepAlgoAPI_Section section(xFace, faceMaker.Face());
        //    for (TopExp_Explorer explorer(section.Shape(), TopAbs_EDGE); explorer.More(); explorer.Next())
        //    {
        //        TopoDS_Shape shape = explorer.Value();
        //    }

        //    auto aPnt1 = gp_Pnt(sp.X, sp.Y, sp.Z);
        //    auto aPnt2 = gp_Pnt(ep.X, ep.Y, ep.Z);
        //    auto aEdge1 = BRepBuilderAPI_MakeEdge(aPnt1, aPnt2).Edge();
        //    Standard_Real f, l;
        //    auto aCurve = BRep_Tool::Curve(aEdge1, f, l);
        //    auto adaptorCurve = GeomAdaptor_Curve(aCurve, f, l);
        //    auto tempCurve = adaptorCurve.Curve();

        //    Handle(Geom_Plane) aPlane = Handle(Geom_Plane)::DownCast(BRep_Tool::Surface(xFace));
        //    gp_Dir dir(0, 0, 1);
        //    Handle(Geom_Curve) resCurve = GeomProjLib::ProjectOnPlane(tempCurve, aPlane, dir, Standard_True);

        //    Handle(Geom_Curve) curve1 = GeomProjLib::Project(tempCurve, BRep_Tool::Surface(xFace));
        //    GeomAPI_IntCS Intersector(resCurve, BRep_Tool::Surface(xFace));
        //    auto nums2 = Intersector.NbPoints();

        //    TopoDS_Edge E;
        //    Standard_Real tol = 0.0001;
        //    BRep_Builder builder;
        //    builder.MakeEdge(E, resCurve, tol);
        //    auto qqqq = BRep_Tool::Curve(E, f, l);
        //    /* BOPAlgo_Section build;
        //     build.AddArgument(xFace);
        //     build.AddArgument(xOtherFace);
        //     build.Perform();
        //     BOPDS_DS* ds = build.PDS();
        //     BOPDS_VectorOfInterfEE& ee = ds->InterfEE();*/

        //     //auto aPnt11 = gp_Pnt((sp.X + ep.X)/ 2, (sp.Y + ep.Y) / 2, (sp.Z + ep.Z) / 2);
        //    auto aPnt11 = gp_Pnt(sp.X, sp.Y, sp.Z);
        //    auto aPnt21 = gp_Pnt(aPnt11.X(), aPnt11.Y(), aPnt11.Z() + 5);
        //    auto aEdge11 = BRepBuilderAPI_MakeEdge(aPnt11, aPnt21).Edge();
        //    Standard_Real f1, l1;
        //    auto aCurve1 = BRep_Tool::Curve(aEdge11, f1, l1);
        //    auto adaptorCurve1 = GeomAdaptor_Curve(aCurve, f1, l1);
        //    auto tempCurve1 = adaptorCurve1.Curve();

        //    GeomAPI_IntCS Intersector1(tempCurve1, BRep_Tool::Surface(xFace));
        //    auto nums1 = Intersector1.NbPoints();

        //    //GeomAPI_ExtremaCurveCurve extrem(resCurve, BRep_Tool::Surface(xFace));
        //}
#pragma endregion
    }
}