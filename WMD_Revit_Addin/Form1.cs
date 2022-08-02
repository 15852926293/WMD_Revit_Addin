using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMD_Revit_Addin.cmd;
using WMD_Revit_Addin.OprateEvent;

namespace WMD_Revit_Addin
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        ExternalCommandData commandData1 = null;
        Document document1 = null;

        DeleteElementEvent deleteElementEvent = null;
        ExternalEvent externalDeleteElementEvent = null;
        UpdateDuctParamEvent updateDuctParamEvent = null;
        ExternalEvent externalUpdateDuctParamEvent = null;

        MEPCurveEvent breakMEPCurveEvent = null;
        MEPCurveEvent bendMEPCurveEvent = null;
        BreakMEPCurveCmd breakMEPCurveCmd = null;
        BendMEPCurveCmd bendMEPCurveCmd = null;

        CommonEvent quickGridDimEvent = null;
        QuickGridDimensionCmd quickGridDimCmd = null;

        string[] combstr = AppConstants.AllBuiltInCategory.Keys.ToArray<string>();

        public Form1(ExternalCommandData commandData)
        {
            commandData1 = commandData;
            document1 = commandData.Application.ActiveUIDocument.Document;

            this.breakMEPCurveEvent = new MEPCurveEvent(commandData);
            this.bendMEPCurveEvent = new MEPCurveEvent(commandData);

            this.breakMEPCurveCmd = new BreakMEPCurveCmd(commandData);
            breakMEPCurveEvent.RegBreakMEPCurveExternalEvent(breakMEPCurveEvent);

            this.bendMEPCurveCmd = new BendMEPCurveCmd(commandData);
            bendMEPCurveEvent.RegBreakMEPCurveExternalEvent(bendMEPCurveEvent);

            this.quickGridDimEvent = new CommonEvent(commandData);
            this.quickGridDimCmd = new QuickGridDimensionCmd(commandData);
            quickGridDimEvent.RegQuickGridDimensionExternalEvent(quickGridDimEvent);

            this.TopMost = true;
            InitializeComponent();

            deleteElementEvent = new OprateEvent.DeleteElementEvent(commandData);
            externalDeleteElementEvent = ExternalEvent.Create(deleteElementEvent);

            updateDuctParamEvent = new OprateEvent.UpdateDuctParamEvent(commandData1);
            externalUpdateDuctParamEvent = ExternalEvent.Create(updateDuctParamEvent);

            string[] combstr = AppConstants.AllBuiltInCategory.Keys.ToArray<string>();
            comboBox1.Items.AddRange(combstr);

            //获取轴网类型
            DimensionType dimType = null;
            FilteredElementCollector elems = new FilteredElementCollector(document1);
            List<DimensionType> dtlist = elems.OfClass(typeof(DimensionType)).Cast<DimensionType>().Where(o => o.FamilyName.Contains("线性尺寸") && (o.FamilyName != o.Name)).ToList();
            dtlist.ForEach(o =>
            {
                cb_Dimfont.Items.Add(o.Name);
            });
            cb_Dimfont.SelectedIndex = 0;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            UIApplication uiApp = commandData1.Application;
            MessageBox.Show(uiApp.Application.VersionName,"Revit版本号");//版本名
            UIDocument UIDocument = uiApp.ActiveUIDocument;
            Autodesk.Revit.DB.View view = UIDocument.ActiveView;
            MessageBox.Show(view.Name,"当前视图");//当前视图名

            //UIDocument uidoc = commandData1.Application.ActiveUIDocument;
            //Document doc = uidoc.Document;

            ////删除元素
            //ElementId id1 = new ElementId(Int32.Parse(textBox1.Text.ToString()));
            //List<ElementId> elelist = new List<ElementId>();
            //elelist.Add(id1);
            //deleteElementEvent.categoryName = "DeleteElementEvent";
            //deleteElementEvent.ElementList = elelist;
            //externalDeleteElementEvent.Raise();

            //获取图中所有风管的偏移
            //FilteredElementCollector col = new FilteredElementCollector(doc);
            //col.OfClass(typeof(Duct));
            //foreach (Duct duct1 in col)
            //{
            //    Parameter parameter = duct1.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM);
            //    MessageBox.Show("test", parameter.AsValueString());
            //}


            //选择一根风管
            //Selection select = uidoc.Selection;
            //Reference r = select.PickObject(ObjectType.PointOnElement);
            //Element element = doc.GetElement(r);
            //Duct duct = element as Duct;
            //if (duct == null)
            //{
            //    MessageBox.Show("错误", "未选择风管！");
            //    return;
            //}

            ////获取风管面积
            //Parameter para = duct.get_Parameter(BuiltInParameter.RBS_CURVE_SURFACE_AREA);
            //double area = para.AsDouble();
            //MessageBox.Show("风管面积", "风管面积：" + area);

        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认删除模型中所有"+ comboBox1.Text +"？", "提示：", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            BuiltInCategory selectType;
            AppConstants.AllBuiltInCategory.TryGetValue(comboBox1.Text,out selectType);

            UIDocument uidoc = commandData1.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<ElementId> delElelist = new List<ElementId>();
            //FilteredElementCollector col = new FilteredElementCollector(doc);
            //col.OfCategory(selectType);

            // Find all Wall instances in the document by using category filter
            ElementCategoryFilter filter = new ElementCategoryFilter(selectType);

            // Apply the filter to the elements in the active document,
            // Use shortcut WhereElementIsNotElementType() to find wall instances only
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> col = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            foreach (var item in col)
            {
                delElelist.Add(item.Id);
            }

            deleteElementEvent.categoryName = "DeleteElementEvent";
            deleteElementEvent.ElementList = delElelist;
            externalDeleteElementEvent.Raise();
        }

        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox1.Text))
            {
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(combstr);
                comboBox1.DroppedDown = true;
                return;
            }
            string[] combStrUpdate = combstr.Where(o => o.Contains(comboBox1.Text.ToString())).ToArray<string>();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(combStrUpdate);
            comboBox1.SelectionStart = comboBox1.Text.Length;
            Cursor = Cursors.Default;
            comboBox1.DroppedDown = true;
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            comboBox1.SelectionStart = comboBox1.Text.Length;
            Cursor = Cursors.Default;
            comboBox1.DroppedDown = true;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            UIDocument uidoc = commandData1.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            //选择一根风管
            Selection select = uidoc.Selection;
            Reference r = select.PickObject(ObjectType.PointOnElement);
            Element element = doc.GetElement(r);
            Duct duct = element as Duct;
            if (duct == null)
            {
                MessageBox.Show("错误", "未选择风管！");
                return;
            }

            ////获取风管面积
            Parameter heightPara = duct.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM);
            string height = heightPara.AsValueString();
            Parameter widthPara = duct.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM);
            string width = widthPara.AsValueString();
            Parameter areaPara = duct.get_Parameter(BuiltInParameter.RBS_CURVE_SURFACE_AREA);
            string area = areaPara.AsValueString();
            Parameter offsetPara = duct.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM);
            string offset = offsetPara.AsValueString();
            MessageBox.Show("风管高度: " + height + "\n" + "风管宽度: " + width + "\n" + "风管面积: " + area + "\n" + "风管偏移: " + offset + "\n", "风管参数");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UIDocument uidoc = commandData1.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            //选择一根风管
            Selection select = uidoc.Selection;
            Reference r = select.PickObject(ObjectType.PointOnElement);
            Element element = doc.GetElement(r);
            Duct duct = element as Duct;
            if (duct == null)
            {
                MessageBox.Show("错误", "未选择风管！");
                return;
            }

            Parameter heightPara = duct.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM);
            string height = heightPara.AsValueString();
            Parameter widthPara = duct.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM);
            string width = widthPara.AsValueString();
            Parameter offsetPara = duct.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM);
            string offset = offsetPara.AsValueString();

            CurveSetFrm csf = new CurveSetFrm(commandData1,duct, height,width,offset);

            csf.updateDuctParamEvent = updateDuctParamEvent;
            csf.externalUpdateDuctParamEvent = externalUpdateDuctParamEvent;
            csf.Show();

            csf.TopLevel = true;
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            breakMEPCurveEvent.ClearExternal();
            breakMEPCurveEvent.MEPCurveExternal += breakMEPCurveCmd.BreakMEPCurve;
            breakMEPCurveEvent.ExecEvent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            breakMEPCurveEvent.ClearExternal();
            breakMEPCurveEvent.MEPCurveExternal += breakMEPCurveCmd.BreakMEPCurve2;
            breakMEPCurveEvent.ExecEvent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.bendMEPCurveCmd.BendValue = double.Parse(txtBendValue.Text.ToString());
            this.bendMEPCurveCmd.directionTxt = cmbDirection.Text.ToString();
            this.bendMEPCurveCmd.AngleValue = double.Parse(txtAngle.Text.ToString());

            bendMEPCurveEvent.ClearExternal();
            bendMEPCurveEvent.MEPCurveExternal += bendMEPCurveCmd.BendMEPCurve;
            bendMEPCurveEvent.ExecEvent();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            quickGridDimCmd.gridDimensionName = cb_Dimfont.Text.ToString();
            quickGridDimEvent.ClearExternal();
            quickGridDimEvent.CommonEventExternal += quickGridDimCmd.QuickGridDimension;
            quickGridDimEvent.ExecEvent();
        }
    }
}
