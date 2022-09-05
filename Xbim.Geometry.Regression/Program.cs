using System;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.IO;

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
            string path1 = @"D:\文档\三维平台项目\随坡图纸\建筑结构标高贴合(1)\建筑结构标高贴合\√咸宁际华园H-20#\FL1KA0BZ_S-plan.ifc";
            string path2 = @"D:\文档\三维平台项目\随坡图纸\建筑结构标高贴合(1)\建筑结构标高贴合\√咸宁际华园H-20#\FLA0EXWJ_A10-H-20#线框文件 - 副本.ifc";
            MyTest(path1, path2);
            
        }

        public static void MyTest(string filePath1, string filePath2)
        {
            using (var model = IfcStore.Open(filePath1))
            using (var roofModel = IfcStore.Open(filePath2))
            {
                var test = roofModel.Instances.FirstOrDefault<Xbim.Ifc4.Interfaces.IIfcProject>();
                //...do something with the model
                var specificElement = model.Instances.OfType<IfcBuildingElementProxyType>().ToList();
                var beams = model.Instances.Where<IfcBuildingElement>(x => x.Name == "承重结构梁").ToList();
                var beam = beams.Where<IfcBuildingElement>(x => x.GlobalId == "2_Fy5vstD2ZBlaQGH_MMAd").ToList();

                var specificRoof = roofModel.Instances.OfType<IfcBuildingElementProxyType>().ToList();
                var roofs = roofModel.Instances.Where<IfcBuildingElement>(x => x.Name == "常规模型").ToList();
                var roof = roofs.Where(x => x.GlobalId == "1w70dCYSL3Che9XNKZPZA$").ToList();

                CreateSolidFaceService createSolidFaceService = new CreateSolidFaceService();
                createSolidFaceService.CreateFaces(roofModel, beam.First(), roof.First());

                string filePath = @"D:\文档\三维平台项目\随坡图纸\建筑结构标高贴合(1)\建筑结构标高贴合\√咸宁际华园H-20#\FLA0EXWJ_A10-H-20#线框文件 - 副本1.ifc";
                SaveIfcModel(roofModel, filePath);
            }
        }

        public static void SaveIfcModel(IfcStore Model, string filepath)
        {
            if (Model != null)
            {
                using (var txn = Model.BeginTransaction("save ifc file"))
                {
                    try
                    {
                        Model.SaveAs(filepath, StorageType.Ifc);
                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine("Failed to save HelloWall.ifc");
                        Console.WriteLine(e.Message);
                    }
                    txn.Commit();
                }
            }
        }
    }
}
