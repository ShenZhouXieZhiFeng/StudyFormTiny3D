using System;
using System.Collections.Generic;
using System.Text;

namespace MyTiny3D.RenderData
{
    /// <summary>
    /// 材质
    /// </summary>
    public struct Material
    {
        /// <summary>
        /// 自发光
        /// </summary>
        public Color emissive;
        /// <summary>
        /// 环境光反射系数
        /// </summary>
        public float ka;
        /// <summary>
        /// 漫反射
        /// </summary>
        public Color diffuse;
        /// <summary>
        /// 镜面反射，高光
        /// </summary>
        public Color specular;
        /// <summary>
        /// 光泽度
        /// </summary>
        public float shininess;

        public Material(Color _emissive,float _ka,Color _diffuse,Color _specular,float _shininess){
            emissive = _emissive;
            ka = _ka;
            diffuse = _diffuse;
            specular = _specular;
            shininess = _shininess;
        }
    }
}
