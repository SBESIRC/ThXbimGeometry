using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Geometry;
using Xbim.Geometry.Engine.Interop;
using Xbim.Ifc;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc4.Interfaces;
using XbimRegression.FollowSlopeModel;

namespace XbimRegression.CreateIFCService
{
    public static class CreateBeamService
    {
        public static void CreateIFCBeamInIfc(IfcStore model, BeamModel beam)
        {
            var resBeam = CreateBeam(model, beam);
            var project = model.Instances.FirstOrDefault<IIfcProject>();
            var storey = project.Buildings.First().BuildingStoreys.Last() as IfcBuildingStorey;
            relContainBeams2Storey(model, resBeam, storey);
        }

        private static List<IfcBeam> CreateBeam(IfcStore model, BeamModel beam)
        {
            List<IfcBeam> resBeams = new List<IfcBeam>();
            ThXbimPointUtils thXbimPointUtils = new ThXbimPointUtils();
            using (var txn = model.BeginTransaction("Create Beam"))
            {
                foreach (var beamLine in beam.FollowSlopeLines)
                {
                    var beamLength = thXbimPointUtils.PointDistance(beamLine.StartPoint, beamLine.EndPoint);
                    var ret = model.Instances.New<IfcBeam>();
                    //create representation
                    var profile = model.Instances.New<IfcRectangleProfileDef>(d =>
                    {
                        d.XDim = beam.BeamWidth;
                        d.YDim = beam.BeamThick;
                        d.ProfileType = Xbim.Ifc2x3.ProfileResource.IfcProfileTypeEnum.AREA;
                        var placement = model.Instances.New<IfcAxis2Placement2D>();
                        var pt = model.Instances.New<IfcCartesianPoint>();
                        pt.SetXY(0, 0);
                        placement.Location = pt;
                        d.Position = placement;
                    });
                    profile.ProfileName = $"Rec_{beam.BeamWidth}*{beam.BeamThick}";
                    var solid = model.Instances.New<IfcExtrudedAreaSolid>(s =>
                    {
                        s.Depth = beamLength;
                        s.SweptArea = profile;
                        var direction = model.Instances.New<IfcDirection>();
                        direction.SetXYZ(0, 0, 1);
                        s.ExtrudedDirection = direction;
                        var placement = model.Instances.New<IfcAxis2Placement3D>();
                        var pt = model.Instances.New<IfcCartesianPoint>();
                        pt.SetXY(0, 0);
                        placement.Location = pt;
                        s.Position = placement;
                    });
                    ret.Representation = CreateProductDefinitionShape(model, solid);

                    //object placement
                    var transform = XbimMatrix3D.Identity;
                    //transform.RotateAroundZAxis(System.Math.PI / 2);
                    //var transform2 = XbimMatrix3D.Identity;
                    //transform2.RotateAroundYAxis(System.Math.PI / 2);
                    //transform = XbimMatrix3D.Multiply(transform, transform2);
                    //transform = XbimMatrix3D.Multiply(transform, transform2);
                    //transform = XbimMatrix3D.Multiply(transform, XbimMatrix3D.CreateTranslation(new XbimVector3D(-beamLength / 2, 0, beam.BeamThick / 2)));
                    //var floor_origin = thtchstorey.Origin;
                    //PreMultiplyBy(GetTransfrom(beam, floor_origin)));
                    ret.ObjectPlacement = model.Instances.New<IfcLocalPlacement>(l =>
                    {
                        l.PlacementRelTo = null;
                        l.RelativePlacement = model.Instances.New<IfcAxis2Placement3D>(p =>
                        {
                            p.Axis = model.ToIfcDirection(transform.Up);
                            p.RefDirection = model.ToIfcDirection(transform.Right);
                            var pt = model.Instances.New<IfcCartesianPoint>();
                            pt.SetXYZ(transform.OffsetX, transform.OffsetY, transform.OffsetZ);
                            p.Location = pt;
                        });
                    });

                    resBeams.Add(ret);
                }

                txn.Commit();
                return resBeams;
            }
        }

        private static IfcProductDefinitionShape CreateProductDefinitionShape(IfcStore model, IfcExtrudedAreaSolid solid)
        {
            //Create a Definition shape to hold the geometry
            var shape = model.Instances.New<IfcShapeRepresentation>();
            var modelContext = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            shape.ContextOfItems = modelContext;
            shape.RepresentationType = "SweptSolid";
            shape.RepresentationIdentifier = "Body";
            shape.Items.Add(solid);

            //Create a Product Definition and add the model geometry to the wall
            var rep = model.Instances.New<IfcProductDefinitionShape>();
            rep.Representations.Add(shape);
            return rep;
        }

        private static IfcDirection ToIfcDirection(this IfcStore model, XbimVector3D vector)
        {
            var direction = model.Instances.New<IfcDirection>();
            direction.SetXYZ(vector.X, vector.Y, vector.Z);
            return direction;
        }

        public static void relContainBeams2Storey(IfcStore model, List<IfcBeam> beams, IfcBuildingStorey Storey)
        {
            using (var txn = model.BeginTransaction("relContainColumns2Storey"))
            {
                //for ifc2x3
                var relContainedIn = model.Instances.New<IfcRelContainedInSpatialStructure>();
                Storey.ContainsElements.Append<IIfcRelContainedInSpatialStructure>(relContainedIn);
                foreach (var wall in beams)
                {
                    relContainedIn.RelatedElements.Add(wall);
                    //Storey.AddElement(wall);
                }
                relContainedIn.RelatingStructure = Storey;

                txn.Commit();
            }
        }

        //private static XbimMatrix3D GetTransfrom(ThTCHBeam beam, XbimPoint3D floor_origin)
        //{
        //    var offset = new XbimVector3D(floor_origin.X, floor_origin.Y, floor_origin.Z);
        //    return ThMatrix3dExtension.MultipleTransformFroms(1.0, beam.XVector, beam.Origin + offset);
        //}
    }
}
