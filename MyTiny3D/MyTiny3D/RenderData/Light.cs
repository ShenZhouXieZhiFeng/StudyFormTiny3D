using MyTiny3D.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyTiny3D.RenderData
{
    /// <summary>
    /// 光照
    /// </summary>
    public struct Light
    {
        /// <summary>
        /// 灯光时间坐标
        /// </summary>
        public Vector3D worldPosition;
        /// <summary>
        /// 灯光颜色
        /// </summary>
        public Color lightColor;

        public Light(Vector3D _pos, Color _col) {
            worldPosition = _pos;
            lightColor = _col;
        }

    }
}
