using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Geometry;

namespace XbimRegression
{
    public class FilterFace
    {
        public List<XbimPoint3D> FilterBeamFace(List<List<IXbimFace>> xbimFaces)
        {
            var resFaces = FilterFaceByNormal(xbimFaces);
            var face = GetNeedBeamFace(resFaces);
            return FacePoint(face);
        }

        public List<List<IXbimFace>> FilterRoofFace(List<List<IXbimFace>> xbimFaces)
        {
            var resFaces = FilterFaceByNormal(xbimFaces);
            return FilterNeedRoofFace(resFaces);
        }

        /// <summary>
        /// 获取屋顶需要的面
        /// </summary>
        /// <param name="xbimFaces"></param>
        /// <returns></returns>
        private List<List<IXbimFace>> FilterNeedRoofFace(List<List<IXbimFace>> xbimFaces)
        {
            var resFaces = new List<List<IXbimFace>>();
            var groupFaces = GroupFaceByArea(xbimFaces);
            foreach (var gFaces in groupFaces)
            {
                if (gFaces.Value.Count() == 2)
                {
                    var faceDic = GetFaceDic(gFaces.Value);
                    resFaces.Add(faceDic.OrderByDescending(x => x.Value).First().Key);
                }
                else if (gFaces.Value.Count() == 1)
                {
                    resFaces.Add(gFaces.Value.First());
                }
                else if (gFaces.Value.Count() > 2)
                {
                    var gDirFaces = GroupFaceByDir(gFaces.Value);
                    foreach (var tempFaces in gDirFaces)
                    {
                        var faceDic = GetFaceDic(tempFaces.Value);
                        resFaces.Add(faceDic.OrderByDescending(x => x.Value).First().Key);
                    }
                }
            }

            return resFaces;
        }

        /// <summary>
        /// 根据面的面积分类面
        /// </summary>
        /// <param name="xbimFaces"></param>
        /// <returns></returns>
        private Dictionary<double, List<List<IXbimFace>>> GroupFaceByArea(List<List<IXbimFace>> xbimFaces)
        {
            Dictionary<double, List<List<IXbimFace>>> dic = new Dictionary<double, List<List<IXbimFace>>>();
            foreach (var faces in xbimFaces)
            {
                var area = faces.Sum(x => x.Area);
                var dirKeys = dic.Keys.Where(x => Math.Abs(x - area) < 0.1).ToList();
                if (dirKeys.Count > 0)
                {
                    dic[dirKeys.First()].Add(faces);
                }
                else
                {
                    dic.Add(area, new List<List<IXbimFace>>() { faces });
                }
            }

            return dic;
        }

        /// <summary>
        /// 根据面的法向分类面
        /// </summary>
        /// <param name="xbimFaces"></param>
        /// <returns></returns>
        private Dictionary<XbimVector3D, List<List<IXbimFace>>> GroupFaceByDir(List<List<IXbimFace>> xbimFaces)
        {
            Dictionary<XbimVector3D, List<List<IXbimFace>>> dic = new Dictionary<XbimVector3D, List<List<IXbimFace>>>();
            foreach (var faces in xbimFaces)
            {
                var dir = faces.First().Normal;
                var dirKeys = dic.Keys.Where(x => x == dir || x == dir.Negated()).ToList();
                if (dirKeys.Count > 0)
                {
                    dic[dirKeys.First()].Add(faces);
                }
                else
                {
                    dic.Add(dir, new List<List<IXbimFace>>() { faces });
                }
            }

            return dic;
        }

        /// <summary>
        /// 获取面上的点
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        private List<XbimPoint3D> FacePoint(List<IXbimFace> face)
        {
            List<XbimPoint3D> facePts = new List<XbimPoint3D>();
            foreach (var triangle in face)
            {
                foreach (var pt in triangle.OuterBound.Points)
                {
                    if (!facePts.Any(x => x.X == pt.X && x.Y == pt.Y && x.Z == pt.Z))
                    {
                        facePts.Add(pt);
                    }
                }
            }
            return facePts;
        }

        /// <summary>
        /// 筛选出需要的梁面
        /// </summary>
        /// <param name="xbimFaces"></param>
        /// <returns></returns>
        private List<IXbimFace> GetNeedBeamFace(List<List<IXbimFace>> xbimFaces)
        {
            var groupFaces = xbimFaces.GroupBy(x => x.Sum(y => y.Area)).OrderByDescending(x => x.Key).ToList();
            var resFaceLst = groupFaces.First().ToList();
            var faceDic = GetFaceDic(resFaceLst);

            return faceDic.OrderByDescending(x => x.Value).First().Key;
        }

        /// <summary>
        /// 计算面最高点
        /// </summary>
        /// <param name="resFaceLst"></param>
        /// <returns></returns>
        private Dictionary<List<IXbimFace>, double> GetFaceDic(List<List<IXbimFace>> resFaceLst)
        {
            Dictionary<List<IXbimFace>, double> faceDic = new Dictionary<List<IXbimFace>, double>();
            foreach (var faces in resFaceLst)
            {
                double z = double.MinValue;
                foreach (var face in faces)
                {
                    foreach (var pt in face.OuterBound.Points)
                    {
                        if (z < pt.Z)
                        {
                            z = pt.Z;
                        }
                    }
                }
                faceDic.Add(faces, z);
            }

            return faceDic;
        }

        /// <summary>
        /// 先过滤一遍面
        /// </summary>
        /// <param name="xbimFaces"></param>
        /// <returns></returns>
        private List<List<IXbimFace>> FilterFaceByNormal(List<List<IXbimFace>> xbimFaces)
        {
            List<List<IXbimFace>> resFaces = new List<List<IXbimFace>>();
            foreach (var faces in xbimFaces)
            {
                if (Math.Abs(faces.First().Normal.Z) > 0.2)
                {
                    resFaces.Add(faces);
                }
            }

            return resFaces;
        }
    }
}
