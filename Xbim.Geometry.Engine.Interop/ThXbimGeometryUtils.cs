using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ThXbim.Geometry.Interface;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Geometry.Engine.Interop
{
    public class ThXbimGeometryUtils : ThXbimGeometryUtilInterface
    {
        private readonly ThXbimGeometryUtilInterface _engine;
        public ThXbimGeometryUtils()
        {
            var conventions = new XbimArchitectureConventions();    // understands the process we run under
            string assemblyName = $"{conventions.ModuleName}.dll";// + conventions.Suffix; dropping the use of a suffix
            try
            {
                var ass = Assembly.Load(assemblyName);
                var t = ass.GetType("Xbim.Geometry.XbimGeometryUtils");
                var obj = Activator.CreateInstance(t);
                if (obj == null)
                {
                    throw new Exception("Failed to create Geometry Engine");
                }

                _engine = obj as ThXbimGeometryUtilInterface;
                if (_engine == null)
                {
                    throw new Exception("Failed to cast Geometry Engine to IXbimGeometryEngine");
                }

            }
            catch (Exception e)
            {
                throw new FileLoadException($"Failed to load Xbim.Geometry.Engine{conventions.Suffix}.dll", e);
            }
        }

        /// <summary>
        /// 判断两个face是否相交
        /// </summary>
        /// <param name="face1"></param>
        /// <param name="face2"></param>
        /// <returns></returns>
        public bool FaceIntersects(IXbimFace face1, IXbimFace face2)
        {
            return _engine.FaceIntersects(face1, face2);
        }

        /// <summary>
        /// 合并面
        /// </summary>
        /// <param name="face"></param>
        /// <param name="otherFace"></param>
        /// <returns></returns>
        public IXbimFace FaceUnion(IXbimFace face, IXbimFace otherFace)
        {
            return _engine.FaceUnion(face, otherFace);
        }

        /// <summary>
        /// 合并面
        /// </summary>
        /// <param name="faceSet"></param>
        /// <returns></returns>
        public IXbimFace FaceUnion(List<IXbimFace> faceSet)
        {
            return _engine.FaceUnion(faceSet);
        }


        public void CurveProjectToSurface(XbimPoint3D sp, XbimPoint3D ep, IXbimFace face)
        {
            _engine.CurveProjectToSurface(sp, ep, face);
        }
    }
}
