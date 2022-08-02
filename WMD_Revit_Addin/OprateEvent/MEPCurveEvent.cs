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
    public delegate void MEPCurveExternal();
    public class MEPCurveEvent : IExternalEventHandler
    {
        public MEPCurveExternal MEPCurveExternal { get; set; }
        public ExternalEvent ExternalEvent { get; set; }

        ExternalCommandData commandData = null;
        UIDocument UIDoc = null;
        Document Doc = null;
        Application app = null;

        public MEPCurveEvent(ExternalCommandData commandData)
        {
            this.commandData = commandData;


            UIApplication uiApp = commandData.Application;
            app = uiApp.Application;
            //UI文档s
            UIDoc = uiApp.ActiveUIDocument;
            //文档
            Doc = UIDoc.Document;
        }

        public void Execute(UIApplication app)
        {
            MEPCurveExternal.Invoke();
        }

        //注册管道打断
        public ExternalEvent RegBreakMEPCurveExternalEvent(MEPCurveEvent breakMEPCurveEvent)
        {
            ExternalEvent = ExternalEvent.Create(breakMEPCurveEvent);
            return ExternalEvent;
        }

        //注册管道折弯
        public ExternalEvent RegBendMEPCurveExternalEvent(MEPCurveEvent bendMEPCurveEvent)
        {
            ExternalEvent = ExternalEvent.Create(bendMEPCurveEvent);
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
            MEPCurveExternal = null;
        }

        public string GetName()
        {
            return "MEPCurveEvent";
        }
    }
}
