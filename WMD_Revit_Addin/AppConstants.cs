using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace WMD_Revit_Addin
{
    class AppConstants
    {
        public static Dictionary<string, BuiltInCategory> AllBuiltInCategory = new Dictionary<string, BuiltInCategory>()
        {
            ["体量"] = BuiltInCategory.OST_Mass,                     //体量
            ["地形"] = BuiltInCategory.OST_Topography,               //地形
            ["场地"] = BuiltInCategory.OST_Site,                     //场地
            ["坡道"] = BuiltInCategory.OST_Ramps,                    //坡道      
            ["天花板"] = BuiltInCategory.OST_Ceilings,               //天花板 
            ["家具"] = BuiltInCategory.OST_Furniture,                //家具
            ["家具系统"] = BuiltInCategory.OST_FurnitureSystems,     //家具系统
            ["屋顶"] = BuiltInCategory.OST_Roofs,                    //屋顶
            ["常规模型"] = BuiltInCategory.OST_GenericModel,         //常规模型
            ["幕墙嵌板"] = BuiltInCategory.OST_CurtainWallPanels,    //幕墙嵌板
            ["幕墙竖梃"] = BuiltInCategory.OST_CurtainWallMullions,  //幕墙竖梃
            ["房间"] = BuiltInCategory.OST_Rooms,                   //房间
            ["柱"] = BuiltInCategory.OST_Columns,                   //柱
            ["栏杆扶手"] = BuiltInCategory.OST_StairsRailing,        //栏杆扶手
            ["植物"] = BuiltInCategory.OST_Planting,                 //植物
            ["橱柜"] = BuiltInCategory.OST_Casework,                 //橱柜
            ["环境"] = BuiltInCategory.OST_Entourage,                //环境
            ["窗"] = BuiltInCategory.OST_Windows,                    //窗
            ["墙"] = BuiltInCategory.OST_Walls,                      //墙
            ["楼板"] = BuiltInCategory.OST_Floors,                   //楼板
            ["楼板边缘"] = BuiltInCategory.OST_EdgeSlab,             //楼板边缘                                             
            ["竖井洞口"] = BuiltInCategory.OST_ShaftOpening,         //竖井洞口
            ["道路"] = BuiltInCategory.OST_Roads,                    //道路
            ["门"] = BuiltInCategory.OST_Doors,                      //门
            ["环境"] = BuiltInCategory.OST_Entourage,                //环境
            ["封檐板"] = BuiltInCategory.OST_Fascia,                 //封檐板
            ["檐沟"] = BuiltInCategory.OST_Gutter,                   //檐沟
            ["屋檐底板"] = BuiltInCategory.OST_RoofSoffit,            //屋檐底板

            ["结构基础"] = BuiltInCategory.OST_StructuralFoundation,     //结构基础
            ["结构柱"] = BuiltInCategory.OST_StructuralColumns,        //结构柱
            ["结构桁架"] = BuiltInCategory.OST_StructuralTruss,          //结构桁架
            ["楼板"] = BuiltInCategory.OST_Floors,                   //楼板（结构）     
            ["楼梯"] = BuiltInCategory.OST_Stairs,                   //楼梯 
            ["墙"] = BuiltInCategory.OST_Walls,                    //墙
            ["结构框架"] = BuiltInCategory.OST_StructuralFraming,        //结构框架
            ["结构连接"] = BuiltInCategory.OST_StructConnections,        //结构连接
            ["结构钢筋"] = BuiltInCategory.OST_Rebar,                    //结构钢筋
            ["结构钢筋网"] = BuiltInCategory.OST_FabricReinforcement,      //结构钢筋网
            ["楼板边缘"] = BuiltInCategory.OST_EdgeSlab,                  //楼板边缘

            ["管件"] = BuiltInCategory.OST_PipeFitting,              //管件
            ["管道"] = BuiltInCategory.OST_PipeCurves,               //管道
            ["风管"] = BuiltInCategory.OST_DuctCurves,               //风管
            ["风管内衬"] = BuiltInCategory.OST_DuctLinings,              //风管内衬
            ["风管管件"] = BuiltInCategory.OST_DuctFitting,              //风管管件
            ["软管"] = BuiltInCategory.OST_FlexPipeCurves,           //软管
            ["软风管"] = BuiltInCategory.OST_FlexDuctCurves,           //软风管
            ["电缆桥架"] = BuiltInCategory.OST_CableTray,                //电缆桥架
            ["电缆桥架配件"] = BuiltInCategory.OST_CableTrayFitting,         //电缆桥架配件
            ["线管"] = BuiltInCategory.OST_Conduit,                  //线管
            ["线管配件"] = BuiltInCategory.OST_ConduitFitting,           //线管配件
            ["导线"] = BuiltInCategory.OST_Wire,                     //导线

            ["专用设备"] = BuiltInCategory.OST_SpecialityEquipment,      //专用设备
            ["喷头"] = BuiltInCategory.OST_Sprinklers,               //喷头
            ["安全设备"] = BuiltInCategory.OST_SecurityDevices,          //安全设备
            ["机械设备"] = BuiltInCategory.OST_MechanicalEquipment,      //机械设备 
            ["数据设备"] = BuiltInCategory.OST_DataDevices,              //数据设备
            ["护理呼叫设备"] = BuiltInCategory.OST_NurseCallDevices,         //护理呼叫设备
            ["火警设备"] = BuiltInCategory.OST_FireAlarmDevices,         //火警设备
            ["灯具"] = BuiltInCategory.OST_LightingDevices,          //灯具
            ["照明设备"] = BuiltInCategory.OST_LightingFixtures,         //照明设备
            ["电气设备"] = BuiltInCategory.OST_ElectricalEquipment,      //电气设备
            ["电气装置"] = BuiltInCategory.OST_ElectricalFixtures,       //电气装置
            ["电话装置"] = BuiltInCategory.OST_TelephoneDevices,         //电话装置
            ["管道附件"] = BuiltInCategory.OST_PipeAccessory,            //管道附件
            ["通讯设备"] = BuiltInCategory.OST_CommunicationDevices,     //通讯设备
            ["风管附件"] = BuiltInCategory.OST_DuctAccessory,            //风管附件
            ["风道末端"] = BuiltInCategory.OST_DuctTerminal,             //风道末端
            ["卫浴装置"] = BuiltInCategory.OST_PlumbingFixtures,         //卫浴装置
        };
    }
}
