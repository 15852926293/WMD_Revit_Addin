using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using WMD_Revit_Addin.cmd;
using WMD_Revit_Addin.OprateEvent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WMD_Revit_Addin
{
    public partial class BuildBridgeFrm : System.Windows.Forms.Form
    {
        ExternalCommandData commandData1 = null;
        Document document1 = null;
        CommonEvent buildBridgeEvent = null;
        BuildBridgeCmd buildBridgeQuickCmd = null;

        public BuildBridgeFrm(ExternalCommandData commandData)
        {
            commandData1 = commandData;
            document1 = commandData.Application.ActiveUIDocument.Document;

            this.buildBridgeEvent = new CommonEvent(commandData1);
            this.buildBridgeQuickCmd = new BuildBridgeCmd(commandData1);
            buildBridgeEvent.RegBuildBridgeQuickEvent(buildBridgeEvent);

            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                buildBridgeQuickCmd.List_SJGC.Add(double.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString()));
                buildBridgeQuickCmd.List_XLG.Add(double.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString()));
                buildBridgeQuickCmd.List_DBH.Add(double.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString()));
                buildBridgeQuickCmd.List_FBH.Add(double.Parse(dataGridView1.Rows[i].Cells[3].Value.ToString()));
                buildBridgeQuickCmd.List_TBH.Add(double.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString()));
                buildBridgeQuickCmd.List_JDCD.Add(double.Parse(dataGridView1.Rows[i].Cells[5].Value.ToString()));
                buildBridgeQuickCmd.List_X.Add(double.Parse(dataGridView1.Rows[i].Cells[6].Value.ToString()));
                buildBridgeQuickCmd.List_Y.Add(double.Parse(dataGridView1.Rows[i].Cells[7].Value.ToString()));
            }


            buildBridgeEvent.ClearExternal();
            buildBridgeEvent.CommonEventExternal += buildBridgeQuickCmd.BuildBridgeMethod;
            buildBridgeEvent.ExecEvent();
            this.Close();
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                DataObject d = dataGridView1.GetClipboardContent();
                Clipboard.SetDataObject(d);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                string s = Clipboard.GetText();
                string[] lines = s.Split('\n');
                int row = dataGridView1.CurrentCell.RowIndex;
                int col = dataGridView1.CurrentCell.ColumnIndex;

                //check if need add row
                if ((dataGridView1.Rows.Count - row) < lines.Length)
                {
                    dataGridView1.Rows.Add(lines.Length - (dataGridView1.Rows.Count - row));
                }

                foreach (string line in lines)
                {
                    if ((line.Length > 0) && row < dataGridView1.RowCount)
                    {
                        string[] cells = line.Split('\t');
                        for (int i = 0; i < cells.GetLength(0); ++i)
                        {
                            if (col + i < this.dataGridView1.ColumnCount)
                            {
                                dataGridView1[col + i, row].Value = Convert.ChangeType(cells[i], dataGridView1[col + i, row].ValueType);
                            }
                            else
                            {
                                break;
                            }
                        }
                        row++;
                    }
                    else if (row == dataGridView1.RowCount && line.Length > 0)
                    {
                        break;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog outsideFile = new OpenFileDialog();
            outsideFile.ShowDialog();
            this.tb_Beam.Text = outsideFile.SafeFileName;
            buildBridgeQuickCmd.filePath = System.IO.Path.GetFullPath(outsideFile.FileName);
        }
    }
}
