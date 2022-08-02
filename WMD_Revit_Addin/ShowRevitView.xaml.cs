using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WMD_Revit_Addin
{
    /// <summary>
    /// ShowRevitView.xaml 的交互逻辑
    /// </summary>
    public partial class ShowRevitView : Window
    {
        Document doc = null;

        PreviewControl previewControl = null;
        public ShowRevitView(Document doc)
        {
            this.doc = doc;
            InitializeComponent();

            FilteredElementCollector elements = new FilteredElementCollector(doc);
            List<View> views = elements.OfClass(typeof(View)).Where(o => o.GetTypeId() != null && doc.GetElement(o.GetTypeId()) is ViewFamilyType).Cast<View>().ToList();

            cb_views.ItemsSource = views;
            cb_views.SelectedIndex = 0;
        }

        public void LoadView(View view)
        {
            if (previewControl != null)
            {
                this.grid1.Children.Clear();
                previewControl.Dispose();
            }

            previewControl = new PreviewControl(doc, view.Id);

            grid1.Children.Add(previewControl);
        }

        private void cb_views_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            View selectedView = cb_views.SelectedItem as View;
            if (selectedView != null)
            {
                LoadView(selectedView);
            }
        }
    }
}
