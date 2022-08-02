using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WMD_Revit_Addin
{
    [Transaction(TransactionMode.Manual)]
    class UIPanel : IExternalApplication
    {
        internal static Application thisApp = null;
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;//获取当前dll(程序集)的路径

            application.CreateRibbonTab("WMD管理工具");
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("WMD管理工具", "TEST");
            //RibbonPanel ribbonPanel2 = application.CreateRibbonPanel("WMD管理工具", "PANEL2");

            //注意：图标一定要调整属性为 Resource。

            //添加分隔符
            ribbonPanel.AddSeparator();

            #region 1
            PushButtonData pushButtonData = new PushButtonData("INFAMATION1", "Demo1",
                                          thisAssemblyPath, "WMD_Revit_Addin.Demo1");
            PushButton pushButton = ribbonPanel.AddItem(pushButtonData) as PushButton;
            string pngName = "1";
            SetIconByName(pngName, pushButton);
            pushButton.ToolTip = "Tip：区域1按钮1";
            #endregion

            //添加分隔符
            //ribbonPanel.AddSeparator();

            #region 2
            PushButtonData pushButtonData2 = new PushButtonData("INFAMATION2", "Demo2",
                                          thisAssemblyPath, "WMD_Revit_Addin.Demo2");
            PushButton pushButton2 = ribbonPanel.AddItem(pushButtonData2) as PushButton;
            string pngName02 = "2";
            SetIconByName(pngName02, pushButton2);
            pushButton2.ToolTip = "Tip：区域1按钮2";
            //string pngName052 = "族几个定义的介绍";
            //SetToolTipImageByName(pngName052, pushButton2);

            #endregion

            //ribbonPanel.AddSeparator();
            //ribbonPanel2.AddSeparator();

            //#region 创建明细表
            ////创建pushButton
            //PushButtonData pushButtonData8 = new PushButtonData("INFAMATION8", "创建\n明细表",
            //                              thisAssemblyPath, "WM_Addin.SchuduleOperate.CreateSchuduleMainProgram");
            //PushButton pushButton8 = ribbonPanel2.AddItem(pushButtonData8) as PushButton;
            //string pngName08 = "创建明细表";
            //SetIconByName(pngName08, pushButton8);
            //pushButton8.ToolTip = "创建指定族类别的明细表。";
            //#endregion

            //ribbonPanel2.AddSeparator();

            //#region 创建参数导入功能
            ////创建pushButton
            //PushButtonData pushButtonData2 = new PushButtonData("INFAMATION2", "参数数据\n导入",
            //                              thisAssemblyPath, "WM_Addin.ImportEvent.MainProgram");
            //PushButton pushButton2 = ribbonPanel2.AddItem(pushButtonData2) as PushButton;
            //string pngName02 = "表格导入";
            //SetIconByName(pngName02, pushButton2);
            //pushButton2.ToolTip = "将设计、施工的参数数据，如：设备厂商、施工单位等信息，以Excel表格导入，并自动绑定到所有族的项目参数中。注意：表格里必须有UNIGUID值";
            //#endregion

            //#region 创建参数导出功能
            ////创建PushButton
            //PushButtonData pushButtonData = new PushButtonData("INFAMATION", "明细表\n导出",
            //                              thisAssemblyPath, "WM_Addin.OutTOExcelEvent.MainPromgran");//INFAMATION 这个是唯一值不能重复

            ////添加PushButton到ribbonPanel中 
            //PushButton pushButton = ribbonPanel2.AddItem(pushButtonData) as PushButton;
            ////给PushButton添加图片（大图标32px，小图标16px）
            //string pngName01 = "表格导出";

            //SetIconByName(pngName01, pushButton);
            ////给图标设置提示信息
            //pushButton.ToolTip = "选择构件的明细表，并导出为表格";
            //#endregion

            //ribbonPanel2.AddSeparator();

            return Result.Succeeded;
        }

        public void SetIconByName(string PngName, PushButton pushButton)
        {

            //pack://application:,,,/命名空间;component/pic/" + PngName + ".png"
            string file = "pack://application:,,,/WMD_Revit_Addin;component/pic/" + PngName + ".png";
            Uri uriImage = new Uri(file, UriKind.Absolute);    //UriKind.Absolute 绝对路径
            BitmapImage largeImage = new BitmapImage(uriImage);//加载图片
            //pushButton.Image //16*16
            pushButton.LargeImage = largeImage;                //设置到UIButton 上    32*32

        }

        public void SetToolTipImageByName(string PngName, PushButton pushButton)
        {

            //pack://application:,,,/命名空间;component/pic/" + PngName + ".png"
            string file = "pack://application:,,,/WMD_Revit_Addin;component/pic/" + PngName + ".png";
            Uri uriImage = new Uri(file, UriKind.Absolute);    //UriKind.Absolute 绝对路径
            BitmapImage largeImage = new BitmapImage(uriImage);//加载图片

            pushButton.ToolTipImage = largeImage;

        }

    }
}
