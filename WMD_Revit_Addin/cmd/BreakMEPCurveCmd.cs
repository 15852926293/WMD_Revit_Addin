using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMD_Revit_Addin.Filter;

namespace WMD_Revit_Addin.cmd
{
    public class BreakMEPCurveCmd
    {
        ExternalCommandData commandData = null;
        UIDocument UIDoc = null;
        Document Doc = null;
        Application app = null;

        public BreakMEPCurveCmd(ExternalCommandData commandData)
        {
            this.commandData = commandData;
            UIApplication uiApp = commandData.Application;
            app = uiApp.Application;
            //UI文档s
            UIDoc = uiApp.ActiveUIDocument;
            //文档
            Doc = UIDoc.Document;
        }

        public void BreakMEPCurve()
        {
            using (Transaction tr = new Transaction(Doc, "123"))
            {
                tr.Start();
                //选择一根需打断的管道
                Reference reference = UIDoc.Selection.PickObject(ObjectType.Element, new MEPCurveFilter());
                MEPCurve mepCurve = Doc.GetElement(reference) as MEPCurve;

                //在管道上选择打断点
                XYZ breakPoint = UIDoc.Selection.PickPoint();

                //复制一根一模一样的管
                ICollection<ElementId> ids = ElementTransformUtils.CopyElement(Doc, mepCurve.Id, new XYZ(0, 0, 0));
                ElementId newId = ids.FirstOrDefault();
                MEPCurve mepCurveCopy = Doc.GetElement(newId) as MEPCurve;

                //获取原来的起点和终点
                Curve curve = (mepCurve.Location as LocationCurve).Curve;
                XYZ startPoint = curve.GetEndPoint(0);
                XYZ endPoint = curve.GetEndPoint(1);

                //映射点，确保打断点在管道的线上
                breakPoint = curve.Project(breakPoint).XYZPoint;

                //重新定义两根管的起点和终点，达到打断效果
                Line line = Line.CreateBound(startPoint, breakPoint);
                Line line2 = Line.CreateBound(breakPoint, endPoint);


                Connector connector = null;//管1连接的连接器（弯头）实体
                foreach (Connector con in mepCurve.ConnectorManager.Connectors)
                {
                    if (con.Id == 1 && con.IsConnected)
                    {
                        //获取管道的弯头，并解绑
                        Connector con2 = con.AllRefs.Cast<Connector>().ToList().Where(o => o.Owner is FamilyInstance).FirstOrDefault();
                        if (con2 != null)
                        {
                            con.DisconnectFrom(con2);
                            connector = con2;//记录下弯头实体
                            break;
                        }
                    }
                }
                if (connector != null)
                {
                    //将复制的管道与上面的connector相连
                    foreach (Connector con in mepCurveCopy.ConnectorManager.Connectors)
                    {
                        if (con.Id == 1)
                            con.ConnectTo(connector);
                    }
                }


                (mepCurve.Location as LocationCurve).Curve = line;
                (mepCurveCopy.Location as LocationCurve).Curve = line2;

                tr.Commit();
            }

        }


        public void BreakMEPCurve2()
        {
            using (Transaction tr = new Transaction(Doc, "123"))
            {
                tr.Start();

                //选择一根需打断的管道
                Reference reference = UIDoc.Selection.PickObject(ObjectType.Element);
                MEPCurve mepCurve = Doc.GetElement(reference) as MEPCurve;

                //在管道上选择打断点
                XYZ breakPoint1 = UIDoc.Selection.PickPoint();
                XYZ breakPoint2 = UIDoc.Selection.PickPoint();

                //复制一根一模一样的管
                ICollection<ElementId> ids = ElementTransformUtils.CopyElement(Doc, mepCurve.Id, new XYZ(0, 0, 0));
                ElementId newId = ids.FirstOrDefault();
                MEPCurve mepCurveCopy = Doc.GetElement(newId) as MEPCurve;

                //获取原来的起点和终点
                Curve curve = (mepCurve.Location as LocationCurve).Curve;
                XYZ startPoint = curve.GetEndPoint(0);
                XYZ endPoint = curve.GetEndPoint(1);

                //映射点，确保打断点在管道的线上
                breakPoint1 = curve.Project(breakPoint1).XYZPoint;
                breakPoint2 = curve.Project(breakPoint2).XYZPoint;
                //始终保持point1在左侧
                if (breakPoint1.DistanceTo(startPoint) > breakPoint2.DistanceTo(startPoint))
                {
                    XYZ tmpPoint = breakPoint1;
                    breakPoint1 = breakPoint2;
                    breakPoint2 = tmpPoint;
                }

                //重新定义两根管的起点和终点，达到打断效果
                Line line = Line.CreateBound(startPoint, breakPoint1);
                Line line2 = Line.CreateBound(breakPoint2, endPoint);
                (mepCurve.Location as LocationCurve).Curve = line;
                (mepCurveCopy.Location as LocationCurve).Curve = line2;

                tr.Commit();
            }
        }
    }
}
