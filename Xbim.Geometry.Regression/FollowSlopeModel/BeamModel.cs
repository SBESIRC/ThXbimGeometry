using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Geometry;
using Xbim.Geometry.Engine.Interop;
using Xbim.Ifc2x3.GeometryResource;

namespace XbimRegression.FollowSlopeModel
{
    public class BeamModel
    {
        ThXbimPointUtils thXbimPointUtils = new ThXbimPointUtils();
        public BeamModel(List<XbimPoint3D> beamPts)
        {
            var tempPts = new List<XbimPoint3D>(beamPts);
            var pt1 = tempPts.First();
            tempPts.Remove(pt1);
            var pt2 = tempPts.OrderBy(x => thXbimPointUtils.PointDistance(x, pt1)).First();
            tempPts.Remove(pt2);
            var pt3 = tempPts.First();
            var pt4 = tempPts.Last();

            var sp = new XbimPoint3D((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2, (pt1.Z + pt2.Z) / 2);
            var ep = new XbimPoint3D((pt3.X + pt4.X) / 2, (pt3.Y + pt4.Y) / 2, (pt3.Z + pt4.Z) / 2);
            BeamLine = new BeamLine(sp, ep);
            BeamWidth = thXbimPointUtils.PointDistance(pt1, pt2);
            BeamThick = 300;
            FollowSlopeLines = new List<BeamLine>();
        }

        /// <summary>
        /// 原始梁线
        /// </summary>
        public BeamLine BeamLine { get; set; }

        /// <summary>
        /// 随破后的梁线
        /// </summary>
        public List<BeamLine> FollowSlopeLines { get; set; }

        /// <summary>
        /// 梁宽度
        /// </summary>
        public double BeamWidth { get; set; }

        /// <summary>
        /// 梁厚度
        /// </summary>
        public double BeamThick { get; set; }
    }

    public class BeamLine
    {
        public BeamLine(XbimPoint3D sp, XbimPoint3D ep)
        {
            StartPoint = sp;
            EndPoint = ep;
        }
        /// <summary>
        /// 梁线起点
        /// </summary>
        public XbimPoint3D StartPoint { get; set; }

        /// <summary>
        /// 梁线终点
        /// </summary>
        public XbimPoint3D EndPoint { get; set; }
    }
}
