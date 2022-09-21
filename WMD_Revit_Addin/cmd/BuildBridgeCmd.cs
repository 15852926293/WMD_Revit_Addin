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
    public class BuildBridgeCmd
    {

        ExternalCommandData commandData = null;
        UIDocument uiDoc = null;
        Document revitDoc = null;
        Application app = null;

        public string filePath = "";

        public List<double> List_XLG = new List<double>();
        public List<double> List_DBH = new List<double>();
        public List<double> List_FBH = new List<double>();
        public List<double> List_TBH = new List<double>();
        public List<double> List_JDCD = new List<double>();
        public List<double> List_SJGC = new List<double>();
        public List<double> List_X = new List<double>();
        public List<double> List_Y = new List<double>();

        public BuildBridgeCmd(ExternalCommandData commandData)
        {
            this.commandData = commandData;
            UIApplication uiApp = commandData.Application;
            app = uiApp.Application;
            //UI文档s
            uiDoc = uiApp.ActiveUIDocument;
            revitDoc = uiDoc.Document;
        }

        public FamilyInstance LoadBeamFamily()
        {
            FamilySymbol familySymbol = null;
            FamilyInstance beamFamilyInstance = null;


            XYZ startPoint = new XYZ(0, 0, 0);
            XYZ endPoint = new XYZ(2625, 0, 0); //箱梁预设整体长度800000（尽可能长，防止重叠）
            Curve beamCurve = Line.CreateBound(startPoint, endPoint);
            try
            {

                using (Transaction tr = new Transaction(revitDoc, "LoadBeamFamily"))
                {
                    tr.Start();

                    Level level = revitDoc.GetElement(uiDoc.ActiveView.LevelId) as Level;
                    bool modelCurveZhu = revitDoc.LoadFamilySymbol(filePath, System.IO.Path.GetFileNameWithoutExtension(filePath), out familySymbol);   //这里的族一定要有类型才行
                    familySymbol.Activate();   //激活族类型
                    beamFamilyInstance = revitDoc.Create.NewFamilyInstance(beamCurve, familySymbol, level, StructuralType.Beam);

                    tr.Commit();
                }
            }
            catch (Exception e)
            {
                //载入出错异常捕获
            }

            return beamFamilyInstance;
        }


        public void BuildBridgeMethod()
        {
            #region 手动选择
            //选择箱梁单室的实例
            //Selection sel = uiDoc.Selection;
            //Reference ref1 = sel.PickObject(ObjectType.Element, "选择一个变截面箱梁实例");
            //Element elem = revitDoc.GetElement(ref1);
            //FamilyInstance familyInstance = elem as FamilyInstance;
            #endregion

            FamilyInstance familyInstance = LoadBeamFamily();



            if (familyInstance == null)
            {
                TaskDialog.Show("错误1", "箱梁标准段载入失败！");
                return;
            }

            List<ElementId> list = new List<ElementId>();
            list.Add(familyInstance.Id);
            ICollection<ElementId> elementIds = list;

            //获取相应的截面参数
            IList<Parameter> list1 = familyInstance.GetParameters("aH");   //变截面箱梁a截面的高度
            Parameter param1 = list1[0];
            IList<Parameter> list2 = familyInstance.GetParameters("aBH");   //变截面箱梁a截面底板的高度
            Parameter param2 = list2[0];
            IList<Parameter> list3 = familyInstance.GetParameters("aFH");   //变截面箱梁a截面腹板的宽度
            Parameter param3 = list3[0];
            IList<Parameter> list4 = familyInstance.GetParameters("aTH");   //变截面箱梁a截面顶板的高度
            Parameter param4 = list4[0];
            IList<Parameter> list5 = familyInstance.GetParameters("bH");   //变截面箱梁b截面的高度
            Parameter param5 = list5[0];
            IList<Parameter> list6 = familyInstance.GetParameters("bBH");   //变截面箱梁b截面底板的高度
            Parameter param6 = list6[0];
            IList<Parameter> list7 = familyInstance.GetParameters("bFH");   //变截面箱梁b截面腹板的宽度
            Parameter param7 = list7[0];
            IList<Parameter> list8 = familyInstance.GetParameters("bTH");   //变截面箱梁b截面顶板的高度
            Parameter param8 = list8[0];
            IList<Parameter> list9 = familyInstance.GetParameters("Length");   //变截面箱梁节段长度
            Parameter param9 = list9[0];
            //IList<Parameter> list10 = familyInstance.GetParameters("detZ");   //变截面箱梁两个截面的高差
            //Parameter param10 = list10[0];
            //IList<Parameter> list11 = familyInstance.GetParameters("detY");   //变截面箱梁两个截面的高差
            //Parameter param11 = list11[0];

            int j = 0;
            for (int i = 0; i < (Convert.ToInt32(List_SJGC.Count) - 1); i++)
            {
                j = i + 1;
                #region 将表格的族参数赋予给对应的族参数
                Transaction transaction1 = new Transaction(revitDoc);
                transaction1.Start("改变族参数");

                param1.Set(UnitConvers.Tofoot(List_XLG[i]));            //变截面箱梁a截面的高度
                param2.Set(UnitConvers.Tofoot(List_DBH[i]));           //变截面箱梁a截面底板的高度
                param3.Set(UnitConvers.Tofoot(List_FBH[i]));           //变截面箱梁a截面腹板的宽度
                param4.Set(UnitConvers.Tofoot(List_TBH[i]));           //变截面箱梁a截面顶板的高度
                param5.Set(UnitConvers.Tofoot(List_XLG[j]));            //变截面箱梁b截面的高度
                param6.Set(UnitConvers.Tofoot(List_DBH[j]));           //变截面箱梁b截面底板的高度
                param7.Set(UnitConvers.Tofoot(List_FBH[j]));            //变截面箱梁b截面腹板的宽度
                param8.Set(UnitConvers.Tofoot(List_TBH[j]));           //变截面箱梁b截面顶板的高度
                param9.Set(UnitConvers.Tofoot(List_JDCD[j]));         //变截面箱梁节段长度
                //param10.Set(List_SJGC[i] / 304.8 - List_SJGC[j] / 304.8);     //变截面箱梁两个z截面的高差
                //param11.Set(List_Y[i] / 304.8 - List_Y[j] / 304.8);     //变截面箱梁两个Y截面的高差
                transaction1.Commit();
                #endregion

                #region 将改变参数后的族放到相应位置
                Transaction transaction2 = new Transaction(revitDoc);
                transaction2.Start("将族复制到相应位置");

                //LocationPoint familyInstancePoint = familyInstance.Location as LocationPoint;
                Curve elemLine = (familyInstance.Location as LocationCurve).Curve;
                XYZ xyzStart = elemLine.GetEndPoint(0);
                XYZ xyzEnd = elemLine.GetEndPoint(1);
                XYZ xyzOri = ((familyInstance.Location as LocationCurve).Curve as Line).Direction;
                //XYZ tanslation = new XYZ(List_X[i] / 304.8 - xyzEnd.X, List_Y[i] / 304.8 - xyzStart.Y, List_SJGC[i] / 304.8 - xyzEnd.Z);
                double offset = xyzOri.X >= 0 ? UnitConvers.Tofoot(xyzEnd.X + List_X[i]) : UnitConvers.Tofoot(xyzEnd.X - List_X[i]);
                XYZ tanslation = new XYZ(offset, 0, 0);
                ElementTransformUtils.CopyElements(revitDoc, elementIds, tanslation).ToList();


                if (i == Convert.ToInt32(List_SJGC.Count) - 2)
                {
                    revitDoc.Delete(elementIds);
                }

                transaction2.Commit();
                #endregion
            }
            //把存放在ListData里的数据清除掉
            List_DBH.Clear();
            List_FBH.Clear();
            List_JDCD.Clear();
            List_SJGC.Clear();
            List_TBH.Clear();
            List_X.Clear();
            List_XLG.Clear();
            List_Y.Clear();
        }
    }
}
