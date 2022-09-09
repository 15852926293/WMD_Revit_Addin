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
    public partial class BuildBridgeFrm2 : System.Windows.Forms.Form
    {
        ExternalCommandData commandData1 = null;
        Document document1 = null;
        CommonEvent buildBridgeQuickEvent = null;
        BuildBridgeCmd2 buildBridgeCmd2 = null;

        public BuildBridgeFrm2(ExternalCommandData commandData)
        {
            commandData1 = commandData;
            document1 = commandData.Application.ActiveUIDocument.Document;

            this.buildBridgeQuickEvent = new CommonEvent(commandData1);
            this.buildBridgeCmd2 = new BuildBridgeCmd2(commandData1);
            buildBridgeQuickEvent.RegBuildBridgeQuickEvent(buildBridgeQuickEvent);

            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_outside.Text) || string.IsNullOrEmpty(txt_inside.Text) || dataGridView1.Rows.Count <= 1)
            {
                MessageBox.Show("请完善具体参数信息！", "提示");
                return;
            }

            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                buildBridgeCmd2.ListXYZ_X.Add(double.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString()));
                buildBridgeCmd2.ListXYZ_Y.Add(double.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString()));
                buildBridgeCmd2.ListXYZ_Z.Add(double.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString()));
                buildBridgeCmd2.List_height.Add(double.Parse(dataGridView1.Rows[i].Cells[3].Value.ToString()));
                buildBridgeCmd2.List_Hi5.Add(double.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString()));
                buildBridgeCmd2.List_t.Add(double.Parse(dataGridView1.Rows[i].Cells[5].Value.ToString()));
                buildBridgeCmd2.List_Hi1.Add(double.Parse(dataGridView1.Rows[i].Cells[6].Value.ToString()));
            }


            buildBridgeQuickEvent.ClearExternal();
            buildBridgeQuickEvent.CommonEventExternal += buildBridgeCmd2.BuildBridgeMethod2;
            buildBridgeQuickEvent.ExecEvent();
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

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog outsideFile = new OpenFileDialog();
            outsideFile.ShowDialog();
            this.txt_outside.Text = outsideFile.SafeFileName;
            buildBridgeCmd2.filePath1 = System.IO.Path.GetFullPath(outsideFile.FileName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog insideFile = new OpenFileDialog();
            insideFile.ShowDialog();
            this.txt_inside.Text = insideFile.SafeFileName;
            buildBridgeCmd2.filePath2 = System.IO.Path.GetFullPath(insideFile.FileName);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BuildBridge2Setting bb2s = new BuildBridge2Setting(buildBridgeCmd2);
            bb2s.Show();
        }
    }
}
