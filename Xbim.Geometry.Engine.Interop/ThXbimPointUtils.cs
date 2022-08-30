using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ThXbim.Geometry.Interface;
using Xbim.Common.Geometry;

namespace Xbim.Geometry.Engine.Interop
{
    public class ThXbimPointUtils : XbimPointExtensionInterface
    {
        private readonly XbimPointExtensionInterface _engine;
        public ThXbimPointUtils()
        {
            var conventions = new XbimArchitectureConventions();    // understands the process we run under
            string assemblyName = $"{conventions.ModuleName}.dll";// + conventions.Suffix; dropping the use of a suffix
            try
            {
                var ass = Assembly.Load(assemblyName);
                var t = ass.GetType("Xbim.Geometry.XbimPointExtensionUtils");
                var obj = Activator.CreateInstance(t);
                if (obj == null)
                {
                    throw new Exception("Failed to create Geometry Engine");
                }

                _engine = obj as XbimPointExtensionInterface;
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
        /// 求xbim两点距离
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public double PointDistance(XbimPoint3D pt1, XbimPoint3D pt2)
        {
            return _engine.PointDistance(pt1, pt2);
        }
    }
}
