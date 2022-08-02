using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMD_Revit_Addin.cmd;
using WMD_Revit_Addin.OprateEvent;

namespace WMD_Revit_Addin
{
    [Transaction(TransactionMode.Manual)]
    class Demo1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            Form1 f = new Form1(commandData);
            f.Show();

            return Autodesk.Revit.UI.Result.Succeeded;
        }

    }
}
