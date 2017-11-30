using System;
using System.Collections.Generic;
using System.Text;

namespace MyTiny3D.Math
{
    /// <summary>
    /// 向量类
    /// </summary>
    class Vertex3D
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Vertex3D() { }
        public Vertex3D(float _x, float _y, float _z, float _w) {
            x = _x;
            y = _y;
            z = _z;
            w = _w;
        }
        public Vertex3D(float _x, float _y, float _z) {
            x = _x;
            y = _y;
            z = _z;
            w = 0;
        }

        /// <summary>
        /// 向量长度
        /// </summary>
        public float Length {
            get {
                float sq = x * x + y * y + z * z;
                return (float)System.Math.Sqrt(sq);
            }
        }

        /// <summary>
        /// 规范化
        /// </summary>
        /// <returns></returns>
        public Vertex3D Normalize() {
            float length = Length;
            if (length == 0)
            {
                return this;
            }
            float oneOverLen = 1 / length;
            return new Vertex3D(x * oneOverLen, y * oneOverLen, z * oneOverLen);
        }

        #region 重载运算符
        public static Vertex3D operator -(Vertex3D fv,Vertex3D sv) {
            Vertex3D v = new Vertex3D();
            v.x = fv.x - sv.x;
            v.y = fv.y - sv.y;
            v.z = fv.z - sv.z;
            v.w = 0;
            return v;
        }

        public static Vertex3D operator *(Vertex3D fv, Matrix4x4 sm) {
            Vertex3D v = new Vertex3D();
            v.x = fv.x * sm[0, 0] + fv.y * sm[1, 0] + fv.z * sm[2, 0] + fv.w * sm[3, 0];
            v.y = fv.x * sm[0, 1] + fv.y * sm[1, 1] + fv.z * sm[2, 1] + fv.w * sm[3, 1];
            v.z = fv.x * sm[0, 2] + fv.y * sm[1, 2] + fv.z * sm[2, 2] + fv.w * sm[3, 2];
            v.w = fv.x * sm[0, 3] + fv.y * sm[1, 3] + fv.z * sm[2, 3] + fv.w * sm[3, 3];
            return v;
        }

        public static Vertex3D operator +(Vertex3D fv, Vertex3D sv) {
            Vertex3D v = new Vertex3D();
            v.x = fv.x + sv.x;
            v.y = fv.y + sv.y;
            v.z = fv.z + sv.z;
            v.w = fv.w + sv.w;
            return v;
        }

        public static float Dot(Vertex3D fv, Vertex3D sv) {
            return fv.x * sv.x + fv.y * sv.y + fv.z * sv.z;  
        }

        public static Vertex3D Cross(Vertex3D fv, Vertex3D sv) {
            Vertex3D v = new Vertex3D();
            v.x = fv.y * sv.z - fv.z * sv.y;
            v.y = fv.z * sv.x - fv.x * sv.z;
            v.z = fv.x * sv.y - fv.y * sv.x;
            return v;
        }

        #endregion
    }
}
