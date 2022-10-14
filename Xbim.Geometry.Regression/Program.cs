using Xbim.Ifc;
using Xbim.IO.Memory;
using Xbim.Ifc4.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq;
using Xbim.Common.Geometry;
using System.Diagnostics;
using System;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using System.Collections.Generic;
using System.IO;
using Xbim.Ifc.Extensions;
using Xbim.Common.Exceptions;
using Xbim.Geometry.Engine.Interop;

namespace XbimRegression
{
    class Program
    {
        private static void Main(string[] args)
        {
            //顺序inter engine scene reg

            using (var m = IfcStore.Open(@"D:\IfcProject\test2.ifc"))
            {
                XbimGeometryEngine geomEngine = new XbimGeometryEngine();
                var bres = m.Instances[57015] as IIfcBooleanClippingResult;
                //var solid = _xbimGeometryCreator.Create(bres);
                //500 250 7~8k 正确答案
                var solidSet = geomEngine.CreateSolidSet(bres, null);
                ;
            }

            //// ContextTesting is a class that has been temporarily created to test multiple files
            //// ContextTesting.Run();
            //// return;
            //IfcStore.ModelProviderFactory.UseHeuristicModelProvider();
            //var arguments = new Params(args);
            //if (!arguments.IsValid)
            //    return;
            //var processor = new BatchProcessor(arguments);
            //processor.Run();
        }
    }
}
