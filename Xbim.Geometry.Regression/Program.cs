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
namespace XbimRegression
{
    class Program
    {
        private static void Main(string[] args)
        {
            using (var model = MemoryModel.OpenRead(@"""D:\IfcProject\test2.ifc"""))
            {
                var wallBCR = model.Instances[34] as IIfcBooleanClippingResult;
                var solidSet = geomEngine.CreateSolidSet(wallBCR, null);
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
