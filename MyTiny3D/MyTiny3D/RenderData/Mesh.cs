using MyTiny3D.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyTiny3D.RenderData
{
    /// <summary>
    /// 网格类
    /// </summary>
    class Mesh
    {
        private Vertex[] _verts;
        /// <summary>
        /// 顶点数组
        /// </summary>
        public Vertex[] vertices {
            get { return _verts; }
        }

        private Material _mat;
        /// <summary>
        /// 材质
        /// </summary>
        public Material material {
            get { return _mat; }
        }

        /// <summary>
        /// 定义一个mesh所需的信息
        /// </summary>
        /// <param name="pointList">顶点坐标列表</param>
        /// <param name="indexs">顶点索引列表</param>
        /// <param name="uvs">uv坐标列表</param>
        /// <param name="vertColors">顶点颜色列表</param>
        /// <param name="normals">顶点法线列表</param>
        /// <param name="mat">材质</param>
        public Mesh(Vector3D[] pointList, int[] indexs, Point2D[] uvs, Vector3D[] vertColors, Vector3D[] normals, Material mat) {
            _verts = new Vertex[indexs.Length];
            for (int i = 0; i < indexs.Length; i++) {
                int pointIndex = indexs[i];
                Vector3D point = pointList[pointIndex];
                _verts[i] = new Vertex(point, normals[i], uvs[i].x, uvs[i].y, vertColors[i].x, vertColors[i].y, vertColors[i].z);
            }
            _mat = mat;
        }

    }
}
