using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Geometry;
using Xbim.Geometry.Engine.Interop;
using XbimRegression.FollowSlopeModel;

namespace XbimRegression
{
    public class FollowTheSlopeService
    {
        List<XbimPoint3D> beamPts;
        List<List<IXbimFace>> roofFaces;
        ThXbimGeometryUtils thXbimGeometryUtils;
        public FollowTheSlopeService(List<XbimPoint3D> _beamPts, List<List<IXbimFace>> _roofFaces)
        {
            beamPts = _beamPts;
            roofFaces = _roofFaces;
            thXbimGeometryUtils = new ThXbimGeometryUtils();
        }

        public BeamModel FollowSlope()
        {
            BeamModel beam = new BeamModel(beamPts);
            ProjectToFollowSlope(beam);
            return beam;
        }

        private void ProjectToFollowSlope(BeamModel beam)
        {
            var resBeamLines = thXbimGeometryUtils.CurveProjectToSurface(beam.BeamLine.StartPoint, beam.BeamLine.EndPoint, roofFaces.First());
            var moveWidth = beam.BeamWidth / 2;
            foreach (var beamLine in resBeamLines)
            {
                var sPt = beamLine.EdgeStart.VertexGeometry;
                var ePt = beamLine.EdgeEnd.VertexGeometry;
                BeamLine resBeamLine = new BeamLine(sPt, ePt);
                beam.FollowSlopeLines.Add(resBeamLine);
                //var dir = (ePt - sPt).Normalized();
                //var zDir = new XbimVector3D(0, 0, 1);
                //var moveDir = zDir.CrossProduct(dir);
                //var pt1 = sPt + moveDir * moveWidth;
                //var pt2 = ePt + moveDir * moveWidth;
                //var pt3 = ePt - moveDir * moveWidth;
                //var pt4 = sPt - moveDir * moveWidth;
            }
        }
    }
}