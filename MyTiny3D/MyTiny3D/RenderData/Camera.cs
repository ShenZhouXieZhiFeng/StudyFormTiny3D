using MyTiny3D.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyTiny3D.RenderData
{
    /// <summary>
    /// 相机
    /// </summary>
    public struct Camera
    {
        /// <summary>
        /// 相机坐标
        /// </summary>
        public Vector3D pos;
        /// <summary>
        /// 相机朝向
        /// </summary>
        public Vector3D lookAt;
        /// <summary>
        /// 相机朝上向量
        /// </summary>
        public Vector3D up;

        /// <summary>
        /// 观察角度
        /// </summary>
        public float fov;
        /// <summary>
        /// 长宽比
        /// </summary>
        public float aspect;
        /// <summary>
        /// 近裁剪面
        /// </summary>
        public float zn;
        /// <summary>
        /// 远裁剪面
        /// </summary>
        public float zf;

        public Camera(Vector3D pos, Vector3D lookAt, Vector3D up, float fov, float aspect, float zn, float zf)
        {
            this.pos = pos;
            this.lookAt = lookAt;
            this.up = up;
            this.fov = fov;
            this.aspect = aspect;
            this.zn = zn;
            this.zf = zf;
        }
    }
}
