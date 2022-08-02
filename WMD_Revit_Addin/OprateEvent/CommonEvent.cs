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
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;


namespace WMD_Revit_Addin.OprateEvent
{
    public delegate void CommonEventExternal();
    public class CommonEvent : IExternalEventHandler
    {
        public CommonEventExternal CommonEventExternal { get; set; }
        public ExternalEvent ExternalEvent { get; set; }

        ExternalCommandData commandData = null;
        UIDocument uiDoc = null;
        Document doc = null;
        Application app = null;

        public CommonEvent(ExternalCommandData commandData)
        {
            this.commandData = commandData;

            UIApplication uiApp = commandData.Application;
            app = uiApp.Application;
            //UI文档s
            uiDoc = uiApp.ActiveUIDocument;
            //文档
            doc = uiDoc.Document;
        }

        public void Execute(UIApplication app)
        {
            CommonEventExternal.Invoke();
        }

        //注册快速轴网标注事件
        public ExternalEvent RegQuickGridDimensionExternalEvent(CommonEvent quickGridDimension)
        {
            ExternalEvent = ExternalEvent.Create(quickGridDimension);
            return ExternalEvent;
        }



        //执行外部事件
        public void ExecEvent()
        {
            ExternalEvent.Raise();
        }

        //清理委托
        public void ClearExternal()
        {
            CommonEventExternal = null;
        }

        public string GetName()
        {
            return "CommonEvent";
        }
    }
}
