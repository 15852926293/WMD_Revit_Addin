using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;


namespace WMD_Revit_Addin.OprateEvent
{
    public class UpdateDuctParamEvent : IExternalEventHandler
    {
        public string categoryName { get; set; }
        public Duct duct = null;
        public string heigh = "";
        public string width = "";
        public string offset = "";

        UIApplication uIApplication = null;
        UIDocument uIDocument = null;
        Document document = null;
        Autodesk.Revit.ApplicationServices.Application application = null;
        public UpdateDuctParamEvent(ExternalCommandData commandData)
        {
            //ui应用程序
            uIApplication = commandData.Application;
            //应用程序
            application = uIApplication.Application;
            //UI文档
            uIDocument = uIApplication.ActiveUIDocument;
            //文档
            document = uIDocument.Document;

        }

        public void Execute(UIApplication app)
        {
            MainOperate(uIDocument);
        }

        public string GetName()
        {
            return "UpdateDuctParamEvent";
        }

        public void MainOperate(UIDocument uIDocument)
        {
            Document doc = uIDocument.Document;

            //string categoryName1 = this.categoryName;

            Parameter heightparam = duct.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM);
            Parameter widthparam = duct.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM);
            Parameter offsetparam = duct.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM);
            using (Transaction tr = new Transaction(doc, "RevitFirstTest"))//修改模型时需要开启事务
            {
                tr.Start();
                heightparam.SetValueString(heigh);
                widthparam.SetValueString(width);
                //areaparam.SetValueString(area);
                offsetparam.SetValueString(offset);
                tr.Commit();
                MessageBox.Show("设置成功");
            }


        }
    }
}
