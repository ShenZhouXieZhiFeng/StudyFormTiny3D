using System;
using System.Collections.Generic;
using System.Text;

namespace MyTiny3D.Math
{
    /// <summary>
    /// 4x4矩阵
    /// </summary>
    class Matrix4x4
    {
        private float[,] _m = new float[4,4];

        public Matrix4x4() { }
        public Matrix4x4(float m11, float m12, float m13, float m14,
                         float m21, float m22, float m23, float m24,
                         float m31, float m32, float m33, float m34,
                         float m41, float m42, float m43, float m44)
        {
            _m[0, 0] = m11; _m[0, 1] = m12; _m[0, 2] = m13; _m[0, 3] = m14;
            _m[1, 0] = m21; _m[1, 1] = m22; _m[1, 2] = m23; _m[1, 3] = m24;
            _m[2, 0] = m31; _m[2, 1] = m32; _m[2, 2] = m33; _m[2, 3] = m34;
            _m[3, 0] = m41; _m[3, 1] = m42; _m[3, 2] = m43; _m[3, 3] = m44;
        }

        public float this[int i, int j] {
            get { return _m[i, j]; }
            set { _m[i, j] = value; }
        }

        public void SetZero() {
            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    _m[i, j] = 0;
                }
            }
        }

        /// <summary>
        /// 矩阵乘法
        /// </summary>
        /// <param name="fm"></param>
        /// <param name="sm"></param>
        /// <returns></returns>
        public static Matrix4x4 operator *(Matrix4x4 lhs, Matrix4x4 rhs)
        {
            Matrix4x4 nm = new Matrix4x4();
            nm.SetZero();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        nm._m[i, j] += lhs._m[i, k] * rhs._m[k, j];
                    }
                }
            }
            return nm;
        }

        /// <summary>
        /// 单位化矩阵
        /// </summary>
        public void Identity() {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (i == j)
                    {
                        _m[i, j] = 1;
                    }
                    else {
                        _m[i, j] = 0;
                    }
                }
            }
        }

        /// <summary>
        /// 求转置
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 Transpose()
        {
            Matrix4x4 m = new Matrix4x4();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    m[i, j] = _m[j, i];
                }
            }
            return m;
        }

        /// <summary>
        /// 求矩阵行列式
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public float Determinate()
        {
            return Determinate(_m, 4);
        }

        private float Determinate(float[,] m, int n)
        {
            if (n == 1)
            {//递归出口
                return m[0, 0];
            }
            else
            {
                float result = 0;
                float[,] tempM = new float[n - 1, n - 1];
                for (int i = 0; i < n; i++)
                {
                    //求代数余子式
                    for (int j = 0; j < n - 1; j++)//行
                    {
                        for (int k = 0; k < n - 1; k++)//列
                        {
                            int x = j + 1;//原矩阵行
                            int y = k >= i ? k + 1 : k;//原矩阵列
                            tempM[j, k] = m[x, y];
                        }
                    }

                    result += (float)System.Math.Pow(-1, 1 + (1 + i)) * m[0, i] * Determinate(tempM, n - 1);
                }
                return result;
            }
        }

        /// <summary>
        /// 获取当前矩阵的伴随矩阵
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 GetAdjoint()
        {
            int x, y;
            float[,] tempM = new float[3, 3];
            Matrix4x4 result = new Matrix4x4();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int t = 0; t < 3; ++t)
                        {
                            x = k >= i ? k + 1 : k;
                            y = t >= j ? t + 1 : t;

                            tempM[k, t] = _m[x, y];
                        }
                    }
                    result._m[i, j] = (float)System.Math.Pow(-1, (1 + j) + (1 + i)) * Determinate(tempM, 3);
                }
            }
            return result.Transpose();
        }

        /// <summary>
        /// 求当前矩阵的逆矩阵
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 Inverse()
        {
            float a = Determinate();
            if (a == 0)
            {
                Console.WriteLine("矩阵不可逆");
                return null;
            }
            Matrix4x4 adj = GetAdjoint();//伴随矩阵
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    adj._m[i, j] = adj._m[i, j] / a;
                }
            }
            return adj;
        }
    }
}
