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

        public void FollowSlope()
        {
            BeamModel beam = new BeamModel(beamPts);
            ProjectToFollowSlope(beam);
        }

        private void ProjectToFollowSlope(BeamModel beam)
        {
            thXbimGeometryUtils.CurveProjectToSurface(beam.BeamLine.StartPoint, beam.BeamLine.EndPoint, roofFaces.First().First());
        }
    }
}