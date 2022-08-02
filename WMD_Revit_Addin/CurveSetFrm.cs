using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMD_Revit_Addin.OprateEvent;

namespace WMD_Revit_Addin
{
    public partial class CurveSetFrm : System.Windows.Forms.Form
    {
        ExternalCommandData commandData1 = null;
        Document document1 = null;
        Duct duct = null;
        public OprateEvent.UpdateDuctParamEvent updateDuctParamEvent = null;
        public ExternalEvent externalUpdateDuctParamEvent = null;
        public CurveSetFrm(ExternalCommandData commandData,Duct dt, string height,string width,string offset)
        {
            commandData1 = commandData;
            document1 = commandData.Application.ActiveUIDocument.Document;

            InitializeComponent();

            tb_height.Text = height;
            tb_width.Text = width;
            tb_offset.Text = offset;
            duct = dt;

            //updateDuctParamEvent = new OprateEvent.UpdateDuctParamEvent(commandData);
            //externalUpdateDuctParamEvent = ExternalEvent.Create(updateDuctParamEvent);

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updateDuctParamEvent.categoryName = "UpdateDuctParamEvent";
            updateDuctParamEvent.duct = duct;
            updateDuctParamEvent.heigh = tb_height.Text;
            updateDuctParamEvent.width = tb_width.Text;
            updateDuctParamEvent.offset = tb_offset.Text;

            externalUpdateDuctParamEvent.Raise();
        }
    }
}
