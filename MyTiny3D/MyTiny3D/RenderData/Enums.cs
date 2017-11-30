using System;
using System.Collections.Generic;
using System.Text;

namespace MyTiny3D.RenderData
{
    /// <summary>
    /// 渲染模式
    /// </summary>
    public enum RenderMode {
        /// <summary>
        /// 线框模式
        /// </summary>
        Wireframe,
        /// <summary>
        /// 纹理模式
        /// </summary>
        Textured,
        /// <summary>
        /// 顶点色模式
        /// </summary>
        VertexColor
    }

    /// <summary>
    /// 光照模式
    /// </summary>
    public enum LightMode {
        ON,
        OFF
    }

    /// <summary>
    /// 纹理过滤模式
    /// </summary>
    public enum TextureFilterMode {
        /// <summary>
        /// 点采样
        /// </summary>
        Point,
        /// <summary>
        /// 双线性纹理
        /// </summary>
        Bilinear
    }
   
}
