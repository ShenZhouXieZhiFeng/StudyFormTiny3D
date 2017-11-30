using MyTiny3D.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyTiny3D.RenderData
{
    /// <summary>
    /// 顶点信息
    /// </summary>
    class Vertex
    {
        //顶点位置
        public Vector3D point;
        //纹理坐标
        public float u, v;
        //顶点颜色
        public Color vcolor;
        //法线
        public Vector3D normal;

        //光照颜色
        public Color lightingColor;
        //1/z，用于顶点信息的透视校正
        public float onePerZ;

        public Vertex() { }
        /// <summary>
        /// 定义一个顶点所需的所有信息
        /// </summary>
        /// <param name="point">坐标</param>
        /// <param name="normal">法线</param>
        /// <param name="u">纹理u</param>
        /// <param name="v">纹理v</param>
        /// <param name="r">颜色r</param>
        /// <param name="g">颜色r</param>
        /// <param name="b">颜色r</param>
        public Vertex(Vector3D point, Vector3D normal, float u, float v, float r, float g, float b) {
            this.point = point;
            this.normal = normal;
            this.point.w = 1;
            vcolor = new Color();
            vcolor.r = r;
            vcolor.g = g;
            vcolor.b = b;
            onePerZ = 1;
            this.u = u;
            this.v = v;
            lightingColor = new Color();
            lightingColor.r = lightingColor.g = lightingColor.b = 1;
        }

        public Vertex(Vertex v) {
            point = v.point;
            normal = v.normal;
            this.vcolor = v.vcolor;
            onePerZ = 1;
            this.u = v.u;
            this.v = v.v;
            this.lightingColor = v.lightingColor;
        }
    }
}
