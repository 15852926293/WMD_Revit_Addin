using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMD_Revit_Addin
{
    public class UnitConvers
    {

        /// <summary>
        /// 毫米转英尺
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static double Tofoot(double val)
        {
            double foot = UnitUtils.ConvertToInternalUnits(val, DisplayUnitType.DUT_MILLIMETERS);
            return foot;
        }


        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double AngleToRadian(double angle)
        {
            double radian = angle / 180 * Math.PI;
            return radian;
        }
    }
}
