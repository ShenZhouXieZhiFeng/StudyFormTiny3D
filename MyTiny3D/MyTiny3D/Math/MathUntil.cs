using System;
using System.Collections.Generic;
using System.Text;

namespace MyTiny3D.Math
{
    /// <summary>
    /// 数学工具类
    /// </summary>
    class MathUntil
    {

        /// <summary>
        /// 将数据截取到min和max之间
        /// </summary>
        /// <param name="v"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Range(int v,int min,int max) {
            if (v < min)
                return min;
            if (v > max)
                return max;
            return v;
        }

        public static float Range(float v, float min, float max)
        {
            if (v <= min)
            {
                return min;
            }
            if (v >= max)
            {
                return max;
            }
            return v;
        }

    }
}
