using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace WMD_Revit_Addin.OprateEvent
{
    class DeleteElementEvent : IExternalEventHandler
    {
        public string categoryName { get; set; }
        public List<ElementId> ElementList { get; set; }

        UIApplication uIApplication = null;
        UIDocument uIDocument = null;
        Document document = null;
        Autodesk.Revit.ApplicationServices.Application application = null;
        public DeleteElementEvent(ExternalCommandData commandData)
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
            return "DeleteElementEvent";
        }

        public void MainOperate(UIDocument uIDocument)
        {
            Document doc = uIDocument.Document;

            //string categoryName1 = this.categoryName;


            using (Transaction tr = new Transaction(doc, "DeleteElementEvent"))
            {
                tr.Start();
                doc.Delete(ElementList); // 或 doc.Delete(id1);
                tr.Commit();

                System.Windows.MessageBox.Show("删除成功");
            }
        }

    }
}
