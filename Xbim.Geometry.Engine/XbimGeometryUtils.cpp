#include "XbimGeometryUtils.h"
#include "XbimFaceSet.h"

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
            BOPDS_VectorOfInterfFF& ff = ds->InterfFF();
            return ff.Length() > 0;
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
            /*TopTools_ListOfShape shape;
            XbimFace^ shapeFace = dynamic_cast<XbimFace^>(faceSet[0]);
            shape.Append(shapeFace);
            TopTools_ListOfShape tools;
            for (size_t i = 1; i < faceSet->Count; i++)
            {
                XbimFace^ xFace = dynamic_cast<XbimFace^>(faceSet[i]);
                tools.Append(xFace);
            }

            BRepAlgoAPI_Fuse build;
            build.SetArguments(shape);
            build.SetTools(tools);
            build.Build();
            TopoDS_Shape resShape = build.Shape();
            TopoDS_Face tFace = TopoDS::Face(resShape);
            XbimFace^ resFace = gcnew XbimFace(tFace);*/
            return FaceUnion(faceSet[0], faceSet[1]);
        }
    }
}

