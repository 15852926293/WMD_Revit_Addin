using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMD_Revit_Addin
{
    [Transaction(TransactionMode.Manual)]
    public class QBridgeCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            BuildBridgeFrm buildBridgeFrm = new BuildBridgeFrm(commandData);
            buildBridgeFrm.ShowDialog();

            return Result.Succeeded;
        }
    }
}
