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
    public class BendMEPCurveCmd
    {
        ExternalCommandData commandData = null;
        UIDocument UIDoc = null;
        Document Doc = null;
        Application app = null;

        public double BendValue = 0;
        public double AngleValue = 90;
        public string directionTxt = "纵向 (Z轴)";

        public BendMEPCurveCmd(ExternalCommandData commandData)
        {
            this.commandData = commandData;
            UIApplication uiApp = commandData.Application;
            app = uiApp.Application;
            //UI文档s
            UIDoc = uiApp.ActiveUIDocument;
            //文档
            Doc = UIDoc.Document;
        }
        
        //创建两根MEPCurve之间的弯头
        public void CreateElbow(MEPCurve mepCurve1, MEPCurve mepCurve2)
        {
            Connector con1 = null;
            Connector con2 = null;
            double minDistance = double.MaxValue;

            mepCurve1.ConnectorManager.Connectors.Cast<Connector>().ToList().ForEach(item =>
            {
                mepCurve2.ConnectorManager.Connectors.Cast<Connector>().ToList().ForEach(item2 =>
                {
                    double distance = item.Origin.DistanceTo(item2.Origin);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        con1 = item;
                        con2 = item2;
                    }
                });
            });

            try
            {
                FamilyInstance instance = Doc.Create.NewElbowFitting(con1, con2);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void BendMEPCurve()
        {
            using (Transaction tr = new Transaction(Doc, "123"))
            {
                tr.Start();
                #region 打断管
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
                #endregion

                double bendValue = UnitConvers.Tofoot(BendValue);

                //立管1起点
                XYZ vStart = breakPoint1;
                //立管2起点
                XYZ vStart2 = breakPoint2;
                //立管1,2 终点
                XYZ vend = new XYZ(breakPoint1.X, breakPoint1.Y, breakPoint1.Z);
                XYZ vend2 = new XYZ(breakPoint2.X, breakPoint2.Y, breakPoint2.Z);

                switch (directionTxt)
                {
                    case "横向 (X轴)":
                        if (AngleValue == 90)
                        {
                            vend = new XYZ(breakPoint1.X + bendValue, breakPoint1.Y, breakPoint1.Z);
                            vend2 = new XYZ(breakPoint2.X + bendValue, breakPoint2.Y, breakPoint2.Z);
                        }
                        else
                        {
                            XYZ direction = null;
                            if (bendValue >= 0)
                                direction = XYZ.BasisX;
                            else
                                direction = XYZ.BasisX.Negate();

                            //计算Vend1
                            vStart = breakPoint1;
                            XYZ vend90 = vStart + direction * bendValue;
                            XYZ direction2 = breakPoint2.Subtract(vStart).Normalize();
                            double distance = vend90.DistanceTo(vStart);
                            vend = vend90 + (distance / Math.Tan(UnitConvers.AngleToRadian(AngleValue)) * direction2);

                            //计算Vend2
                            vStart2 = breakPoint2;
                            vend90 = vStart2 + direction * bendValue;
                            direction2 = vStart.Subtract(vStart2).Normalize();
                            distance = vend90.DistanceTo(vStart2);
                            vend2 = vend90 + (distance / Math.Tan(UnitConvers.AngleToRadian(AngleValue)) * direction2);
                        }
                        break;
                    case "竖向 (Y轴)":
                        if (AngleValue == 90)
                        {
                            vend = new XYZ(breakPoint1.X, breakPoint1.Y + bendValue, breakPoint1.Z);
                            vend2 = new XYZ(breakPoint2.X, breakPoint2.Y + bendValue, breakPoint2.Z);
                        }
                        else
                        {
                            XYZ direction = null;
                            if (bendValue >= 0)
                                direction = XYZ.BasisY;
                            else
                                direction = XYZ.BasisY.Negate();

                            //计算Vend1
                            vStart = breakPoint1;
                            XYZ vend90 = vStart + direction * bendValue;
                            XYZ direction2 = breakPoint2.Subtract(vStart).Normalize();
                            double distance = vend90.DistanceTo(vStart);
                            vend = vend90 + (distance / Math.Tan(UnitConvers.AngleToRadian(AngleValue)) * direction2);

                            //计算Vend2
                            vStart2 = breakPoint2;
                            vend90 = vStart2 + direction * bendValue;
                            direction2 = vStart.Subtract(vStart2).Normalize();
                            distance = vend90.DistanceTo(vStart2);
                            vend2 = vend90 + (distance / Math.Tan(UnitConvers.AngleToRadian(AngleValue)) * direction2);
                        }

                        break;
                    case "纵向 (Z轴)":
                        if (AngleValue == 90)
                        {
                            vend = new XYZ(breakPoint1.X, breakPoint1.Y, breakPoint1.Z + bendValue);
                            vend2 = new XYZ(breakPoint2.X, breakPoint2.Y, breakPoint2.Z + bendValue);
                        }
                        else
                        {
                            XYZ direction = null;
                            if (bendValue >= 0)
                                direction = XYZ.BasisZ;
                            else
                                direction = XYZ.BasisZ.Negate();

                            //计算Vend1
                            vStart = breakPoint1;
                            XYZ vend90 = vStart + direction * bendValue;
                            XYZ direction2 = breakPoint2.Subtract(vStart).Normalize();
                            double distance = vend90.DistanceTo(vStart);
                            vend = vend90 + (distance / Math.Tan(UnitConvers.AngleToRadian(AngleValue)) * direction2);

                            //计算Vend2
                            vStart2 = breakPoint2;
                            vend90 = vStart2 + direction * bendValue;
                            direction2 = vStart.Subtract(vStart2).Normalize();
                            distance = vend90.DistanceTo(vStart2);
                            vend2 = vend90 + (distance / Math.Tan(UnitConvers.AngleToRadian(AngleValue)) * direction2);
                        }
                        break;
                }

                Line vline = Line.CreateBound(vStart, vend);    //立管1
                Line vline2 = Line.CreateBound(vStart2, vend2); //立管2
                Line hline = Line.CreateBound(vend, vend2);     //横管

                //创建立管1
                ICollection<ElementId> idsV1 = ElementTransformUtils.CopyElement(Doc, mepCurve.Id, new XYZ(0, 0, 0));
                ElementId IdV1 = idsV1.FirstOrDefault();
                MEPCurve mepCurveV1 = Doc.GetElement(IdV1) as MEPCurve;
                (mepCurveV1.Location as LocationCurve).Curve = vline;

                //创建立管2
                ICollection<ElementId> idsV2 = ElementTransformUtils.CopyElement(Doc, mepCurve.Id, new XYZ(0, 0, 0));
                ElementId IdV2 = idsV2.FirstOrDefault();
                MEPCurve mepCurveV2 = Doc.GetElement(IdV2) as MEPCurve;
                (mepCurveV2.Location as LocationCurve).Curve = vline2;

                //创建横管
                ICollection<ElementId> idsH = ElementTransformUtils.CopyElement(Doc, mepCurve.Id, new XYZ(0, 0, 0));
                ElementId IdH = idsH.FirstOrDefault();
                MEPCurve mepCurveH = Doc.GetElement(IdH) as MEPCurve;
                (mepCurveH.Location as LocationCurve).Curve = hline;

                CreateElbow(mepCurve, mepCurveV1);
                CreateElbow(mepCurveV1, mepCurveH);
                CreateElbow(mepCurveH, mepCurveV2);
                CreateElbow(mepCurveV2, mepCurveCopy);

                tr.Commit();
            }

        }

    }
}
