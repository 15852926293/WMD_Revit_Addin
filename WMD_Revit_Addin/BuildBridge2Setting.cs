using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMD_Revit_Addin.cmd;

namespace WMD_Revit_Addin
{
    public partial class BuildBridge2Setting : Form
    {
        BuildBridgeCmd2 BBC2 = null;
        public BuildBridge2Setting(BuildBridgeCmd2 bbc2)
        {
            BBC2 = bbc2;
            InitializeComponent();
            textBox1.Text = BBC2.NewFamilyDocumenPath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.ShowDialog();
            if (string.IsNullOrEmpty(file.SafeFileName))
                return;
            this.textBox1.Text = file.SafeFileName;
            BBC2.NewFamilyDocumenPath = System.IO.Path.GetFullPath(file.FileName);
        }
    }
}
