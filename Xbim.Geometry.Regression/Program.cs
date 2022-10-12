using Xbim.Common.Logging;
using Xbim.Geometry.Engine.Interop;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace XbimRegression
{
    class Program
    {
        private static void Main(string[] args)
        {
            // ContextTesting is a class that has been temporarily created to test multiple files
            // ContextTesting.Run();
            // return;
            using (var m = IfcStore.Open(@"D:\IfcProject\test2.ifc"))
            {
                XbimGeometryEngine _xbimGeometryCreator = new XbimGeometryEngine();
                var bres = m.Instances[57015] as IIfcBooleanResult;
                var solid = _xbimGeometryCreator.Create(bres);
                ;
            }
            //var arguments = new Params(args);
            //if (!arguments.IsValid)
            //    return;
            //var processor = new BatchProcessor(arguments);
            //processor.Run();
        }
    }
}
