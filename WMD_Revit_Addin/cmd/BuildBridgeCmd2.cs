using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace WMD_Revit_Addin.cmd
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class BuildBridgeCmd2
    {
        ExternalCommandData commandData = null;
        UIDocument uiDoc = null;
        Document revitDoc = null;
        Application app = null;
        UIApplication uiApp = null;

        public List<double> ListXYZ_X = new List<double>();
        public List<double> ListXYZ_Y = new List<double>();
        public List<double> ListXYZ_Z = new List<double>();
        public List<double> List_height = new List<double>();
        public List<double> List_Hi1 = new List<double>();
        public List<double> List_t = new List<double>();
        public List<double> List_Hi5 = new List<double>();
        public string filePath1 = "";
        public string filePath2 = "";

        public string NewFamilyDocumenPath = @"C:\ProgramData\Autodesk\RVT 2018\Family Templates\Chinese\概念体量\公制体量.rft";

        public BuildBridgeCmd2(ExternalCommandData commandData)
        {
            this.commandData = commandData;
            uiApp = commandData.Application;
            app = uiApp.Application;
            uiDoc = uiApp.ActiveUIDocument;
            revitDoc = uiDoc.Document;
        }

        public void BuildBridgeMethod2()
        {
            List<Family> familyList = new List<Family>();

            try
            {
                for (int i = 0; i < ListXYZ_X.Count; i++)
                {
                    Document familyDoc = app.NewFamilyDocument(NewFamilyDocumenPath);
                    familyList.Add(CreatFamiliesItem(familyDoc, revitDoc, app, i, filePath1, filePath2));
                    //保存体量文件
                    //familyDoc.SaveAs(@"D:\箱梁-" + i + ".rfa");
                    //familyDoc.Close();
                }
            }
            catch (Exception e)
            {
                //生成体量报错处理
            }

            FilteredElementCollector collector = new FilteredElementCollector(revitDoc);
            collector.OfClass(typeof(Level));
            Level level = null;
            foreach (Level le in collector)//找标高
            {
                if (le.Name.Contains("{三维}"))//找到三维视图
                {
                    level = le;
                }
            }

            using (Transaction tr = new Transaction(revitDoc, "BuildBridge2"))
            {
                tr.Start();
                familyList.ForEach(f =>
                {
                    ISet<ElementId> ids = f.GetFamilySymbolIds();

                    FamilySymbol familySymbol = revitDoc.GetElement(ids.FirstOrDefault()) as FamilySymbol;
                    if (!familySymbol.IsActive)
                        familySymbol.Activate();
                    revitDoc.Create.NewFamilyInstance(new XYZ(0, 0, 0), familySymbol, level, StructuralType.NonStructural);
                });

                //设置“显示体量”
                RevitCommandId cmdid = RevitCommandId.LookupPostableCommandId(PostableCommand.ShowMassFormAndFloors);
                //加载命令
                uiApp.PostCommand(cmdid);

                tr.Commit();
            }

        }

        /// <summary>
        /// 创建截面
        /// </summary>
        /// <param name="familyDoc"></param>
        /// <param name="revitDoc"></param>
        /// <param name="revitApp"></param>
        /// <param name="i"></param>
        /// <param name="filePath1"></param>
        /// <param name="filePath2"></param>
        /// <returns></returns>
        public Family CreatFamiliesItem(Document familyDoc, Document revitDoc, Application revitApp, int i, string filePath1, string filePath2)
        {
            FamilyInstance famIns1;
            FamilyInstance famIns2;
            FamilyInstance famIns3;
            FamilyInstance famIns4;
            Plane plane1 = null;
            Plane plane2 = null;

            using (Transaction transaction1 = new Transaction(familyDoc))
            {
                //忽略草图线偏离轴的提示
                FailureHandlingOptions fho = transaction1.GetFailureHandlingOptions();
                fho.SetFailuresPreprocessor(new FailurePreprocessor());
                transaction1.SetFailureHandlingOptions(fho);

                transaction1.Start("主题");
                #region 平曲线     
                ReferencePointArray refPointArray = new ReferencePointArray();
                for (int j = 0; j < ListXYZ_X.Count; j++)
                {
                    ReferencePoint referencePoint = familyDoc.FamilyCreate.NewReferencePoint(new XYZ(UnitConvers.Tofoot(ListXYZ_X[j]), UnitConvers.Tofoot(ListXYZ_Y[j]), 0));   //在体量族里建成空间参照点
                    refPointArray.Append(referencePoint);
                }
                CurveByPoints curve = familyDoc.FamilyCreate.NewCurveByPoints(refPointArray);//在体量族里将空间参照点串成空间曲线

                #endregion
                #region 纵曲线
                ReferencePointArray refPointArray2 = new ReferencePointArray();
                for (int j = 0; j < ListXYZ_X.Count; j++)
                {
                    ReferencePoint referencePoint2 = familyDoc.FamilyCreate.NewReferencePoint(new XYZ(UnitConvers.Tofoot(ListXYZ_X[j]), UnitConvers.Tofoot(ListXYZ_Y[j]), UnitConvers.Tofoot(ListXYZ_Z[j])));   //在体量族里建成空间参照点
                    refPointArray2.Append(referencePoint2);
                }
                CurveByPoints curve2 = familyDoc.FamilyCreate.NewCurveByPoints(refPointArray2);//在体量族里将空间参照点串成空间曲线
                //ReferenceArray path2 = new ReferenceArray();
                //path2.Append(curve2.GeometryCurve.Reference);                              
                #endregion
                famIns1 = LocationFamilyInstance(familyDoc, filePath1, curve2.GetPoints().get_Item(i).Position, (curve.GeometryCurve as HermiteSpline).Tangents[i], i);
                famIns2 = LocationFamilyInstance(familyDoc, filePath1, curve2.GetPoints().get_Item(i + 1).Position, (curve.GeometryCurve as HermiteSpline).Tangents[i + 1], i + 1);
                famIns3 = LocationFamilyInstance2(familyDoc, filePath2, curve2.GetPoints().get_Item(i).Position, (curve.GeometryCurve as HermiteSpline).Tangents[i], i);
                famIns4 = LocationFamilyInstance2(familyDoc, filePath2, curve2.GetPoints().get_Item(i + 1).Position, (curve.GeometryCurve as HermiteSpline).Tangents[i + 1], i + 1);

                try
                {
                    plane1 = Plane.CreateByNormalAndOrigin((curve.GeometryCurve as HermiteSpline).Tangents[i], curve.GetPoints().get_Item(i).Position);
                    plane2 = Plane.CreateByNormalAndOrigin((curve.GeometryCurve as HermiteSpline).Tangents[i + 1], curve.GetPoints().get_Item(i + 1).Position);
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message, "123");
                }
                familyDoc.Delete(curve.Id);
                familyDoc.Delete(curve2.Id);
                refPointArray.Clear();
                refPointArray2.Clear();
                transaction1.Commit();
            }
            using (Transaction transaction2 = new Transaction(familyDoc))
            {
                //忽略草图线偏离轴的提示
                FailureHandlingOptions fho = transaction2.GetFailureHandlingOptions();
                fho.SetFailuresPreprocessor(new FailurePreprocessor());
                transaction2.SetFailureHandlingOptions(fho);

                transaction2.Start("主题");
                ReferenceArray profiles0 = GetReferenceArrayFromFamilyInstance(familyDoc, famIns1, plane1);
                ReferenceArray profiles1 = GetReferenceArrayFromFamilyInstance(familyDoc, famIns2, plane2);

                ReferenceArray profiles2 = GetReferenceArrayFromFamilyInstance(familyDoc, famIns3, plane1);
                ReferenceArray profiles3 = GetReferenceArrayFromFamilyInstance(familyDoc, famIns4, plane2);

                ReferenceArrayArray profilesArray = new ReferenceArrayArray();
                profilesArray.Append(profiles0);
                profilesArray.Append(profiles1);
                Form form = familyDoc.FamilyCreate.NewLoftForm(true, profilesArray);
                ReferenceArrayArray profilesArray2 = new ReferenceArrayArray();
                profilesArray2.Append(profiles2);
                profilesArray2.Append(profiles3);
                Form form2 = familyDoc.FamilyCreate.NewLoftForm(false, profilesArray2);

                string paramName = "材质";
                familyDoc.FamilyManager.AddParameter(paramName, BuiltInParameterGroup.PG_MATERIALS, ParameterType.Material, true);
                transaction2.Commit();
                Family loadedFamily = familyDoc.LoadFamily(revitDoc, new ProjectFamLoadOption());
                return loadedFamily;
            }
        }
        /// <summary>
        /// 得到截面的referenceArray
        /// </summary>
        /// <param name="familyDoc"></param>
        /// <param name="famIns"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        private ReferenceArray GetReferenceArrayFromFamilyInstance(Document familyDoc, FamilyInstance famIns, Plane plane)
        {
            ReferenceArray profilesArray = new ReferenceArray();
            Options opt = new Options();

            IList<Curve> curves1 = new List<Curve>();
            GeometryElement e = famIns.get_Geometry(opt);
            foreach (GeometryObject obj in e)
            {
                GeometryInstance geoInstance = obj as GeometryInstance;
                GeometryElement geoElement = geoInstance.GetInstanceGeometry();

                foreach (GeometryObject obj2 in geoElement)
                {
                    if (obj2.GetType().ToString() == "Autodesk.Revit.DB.Line" || obj2.GetType().ToString() == "Autodesk.Revit.DB.Arc")
                    {
                        Curve curve = obj2 as Curve;
                        ModelCurve modelcurve = familyDoc.FamilyCreate.NewModelCurve(curve, SketchPlane.Create(familyDoc, plane));
                        profilesArray.Append(modelcurve.GeometryCurve.Reference);
                    }
                }
            }
            return profilesArray;
        }
        /// <summary>
        /// 把外截面族准确的放在相应位置
        /// </summary>
        /// <param name="familyDoc"></param>
        /// <param name="filePath"></param>
        /// <param name="location"></param>
        /// <param name="normal"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private FamilyInstance LocationFamilyInstance(Document familyDoc, string filePath, XYZ location, XYZ normal, int i)
        {
            FamilySymbol familySymbol = null;
            bool modelCurveZhu = familyDoc.LoadFamilySymbol(filePath, System.IO.Path.GetFileNameWithoutExtension(filePath), out familySymbol);   //这里的族一定要有类型才行
            familySymbol.Activate();   //激活族类型            
            List<Autodesk.Revit.Creation.FamilyInstanceCreationData> list = new List<Autodesk.Revit.Creation.FamilyInstanceCreationData>();
            list.Add(new Autodesk.Revit.Creation.FamilyInstanceCreationData(location, familySymbol, StructuralType.NonStructural));
            var familyInstancesIds = familyDoc.FamilyCreate.NewFamilyInstances2(list);
            FamilyInstance familyInstance = familyDoc.GetElement(familyInstancesIds.ElementAt(0)) as FamilyInstance;
            LocationPoint point = familyInstance.Location as LocationPoint;
            Line line1 = Line.CreateBound(location, new XYZ(location.X, location.Y, (location.Z + 1)));
            double angle = normal.AngleTo(new XYZ(1, 0, 0));
            //Math.Acos(normal.DotProduct(new XYZ(1, 0, 0)));
            if (normal.CrossProduct(new XYZ(1, 0, 0)).Z < 0)
            {
                point.Rotate(line1, angle);
            }
            else
            {
                point.Rotate(line1, -angle);
            }

            IList<Parameter> listParameters = familyInstance.GetParameters("H");
            listParameters[0].Set(UnitConvers.Tofoot(List_height[i]));
            return familyInstance;
        }

        /// <summary>
        /// 把内截面族放到相应位置
        /// </summary>
        /// <param name="familyDoc"></param>
        /// <param name="filePath"></param>
        /// <param name="location"></param>
        /// <param name="normal"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private FamilyInstance LocationFamilyInstance2(Document familyDoc, string filePath, XYZ location, XYZ normal, int i)
        {
            FamilySymbol familySymbol = null;
            bool modelCurveZhu = familyDoc.LoadFamilySymbol(filePath, System.IO.Path.GetFileNameWithoutExtension(filePath), out familySymbol);   //这里的族一定要有类型才行
            familySymbol.Activate();   //激活族类型            
            List<Autodesk.Revit.Creation.FamilyInstanceCreationData> list = new List<Autodesk.Revit.Creation.FamilyInstanceCreationData>();
            list.Add(new Autodesk.Revit.Creation.FamilyInstanceCreationData(location, familySymbol, StructuralType.NonStructural));
            var familyInstancesIds = familyDoc.FamilyCreate.NewFamilyInstances2(list);
            FamilyInstance familyInstance = familyDoc.GetElement(familyInstancesIds.ElementAt(0)) as FamilyInstance;
            LocationPoint point = familyInstance.Location as LocationPoint;
            Line line1 = Line.CreateBound(location, new XYZ(location.X, location.Y, (location.Z + 1)));
            double angle = normal.AngleTo(new XYZ(1, 0, 0));
            //Math.Acos(normal.DotProduct(new XYZ(1, 0, 0)));
            if (normal.CrossProduct(new XYZ(1, 0, 0)).Z < 0)
            {
                point.Rotate(line1, angle);
            }
            else
            {
                point.Rotate(line1, -angle);
            }

            IList<Parameter> listParameters = familyInstance.GetParameters("TH");
            listParameters[0].Set(UnitConvers.Tofoot(List_Hi1[i]));
            IList<Parameter> listParameters1 = familyInstance.GetParameters("BH");
            listParameters1[0].Set(UnitConvers.Tofoot(List_Hi5[i]));
            IList<Parameter> listParameters2 = familyInstance.GetParameters("FH");
            listParameters2[0].Set(UnitConvers.Tofoot(List_t[i]));
            IList<Parameter> listParameters3 = familyInstance.GetParameters("H");
            listParameters3[0].Set(UnitConvers.Tofoot(List_height[i]));
            return familyInstance;
        }
    }
}
