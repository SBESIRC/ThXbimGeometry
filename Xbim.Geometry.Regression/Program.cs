using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc2x3.ProductExtension;

namespace XbimRegression
{
    class Program
    {
        private static void Main(string[] args)
        {
            // ContextTesting is a class that has been temporarily created to test multiple files
            // ContextTesting.Run();
            // return;
            //IfcStore.ModelProviderFactory.UseHeuristicModelProvider();
            //var arguments = new Params(args);
            //if (!arguments.IsValid)
            //    return;
            //var processor = new BatchProcessor(arguments);
            //processor.Run();
            MyTest(@"D:\文档\三维平台项目\随坡图纸\建筑结构标高贴合(1)\建筑结构标高贴合\√咸宁际华园H-20#\FL1KA0BZ_S-plan.ifc");
        }

        public static void MyTest(string filePath1)
        {
            using (var model = IfcStore.Open(filePath1))
            {
                //...do something with the model
                var specificElement = model.Instances.OfType<IfcBuildingElementProxyType>().ToList();
                var s = model.Instances.OfType<IfcBuildingElement>().ToList();
                var beams = model.Instances.Where<IfcBuildingElement>(x => x.Name == "承重结构梁").ToList();
                var beam = beams.Where<IfcBuildingElement>(x => x.GlobalId == "3_LamJC_X4k8vXXDsAgh1V").ToList();

                foreach (var element in beam)
                {
                    CreateSolidFaceService createSolidFaceService = new CreateSolidFaceService();
                    createSolidFaceService.CreateFaces(element);
                }
            }
        }
    }
}
