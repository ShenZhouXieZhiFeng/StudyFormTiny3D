using MyTiny3D.Math;
using MyTiny3D.RenderData;
using MyTiny3D.Test;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace MyTiny3D
{
    public partial class SoftRendererDemo : Form
    {
        private Bitmap _texture;//纹理
        private Bitmap _frameBuffer;//缓冲
        private Graphics _frameG1;//画笔1
        private Graphics _frameG2;//画笔2
        private float[,] _zBuffer;//z缓冲，做深度测试
        private Mesh _mesh;
        private Light _light;
        private Camera _camera;

        private RenderMode _currentMode;
        private LightMode _lightMode;
        private TextureFilterMode _textureFilterMode;

        private MyTiny3D.RenderData.Color _ambientColor;//全局环境光颜色

        private uint _showTrisCount;//测试数据

        public SoftRendererDemo()
        {
            InitializeComponent();

            try
            {
                //载入贴图
                Image img = Image.FromFile("../../Texture/texture.jpg");
                _texture = new Bitmap(img, 256, 256);
            }
            catch (Exception) {
                _texture = new Bitmap(256, 256);
                initTexture();
            }

            _currentMode = RenderMode.Wireframe;
            _lightMode = LightMode.OFF;
            _textureFilterMode = TextureFilterMode.Bilinear;

            _frameBuffer = new Bitmap(MaximumSize.Width, MaximumSize.Height);
            _frameG1 = Graphics.FromImage(_frameBuffer);
            _zBuffer = new float[MaximumSize.Height, MaximumSize.Width];
            _ambientColor = new RenderData.Color(1, 1, 1);

            _mesh = new Mesh(CubeTestData.pointList, CubeTestData.indexs, CubeTestData.uvs, CubeTestData.vertColors, CubeTestData.norlmas, CubeTestData.mat);

            Vector3D lightPos = new Vector3D(50, 0, 0);
            RenderData.Color lightColor = new RenderData.Color(1, 1, 1);
            _light = new Light(lightPos, lightColor);

            Vector3D cameraPos = new Vector3D(0, 0, 0, 1);
            Vector3D cameraLookAt = new Vector3D(0, 0, 1, 1);
            Vector3D cameraUp = new Vector3D(0, 1, 0, 0);
            float cameraFov = (float)System.Math.PI / 4;
            float cameraAspect = MaximumSize.Width / (float)MaximumSize.Height;
            float cameraZn = 1f;
            float cameraZf = 500f;
            _camera = new Camera(cameraPos, cameraLookAt, cameraUp, cameraFov, cameraAspect, cameraZn, cameraZf);

            System.Timers.Timer mainTimer = new System.Timers.Timer(1000 / 60f);
            mainTimer.Elapsed += new ElapsedEventHandler(Tick);
            mainTimer.AutoReset = true;
            mainTimer.Enabled = true;
            mainTimer.Start();
        }


        private float rota = 0;//旋转
        private void Tick(object sender, ElapsedEventArgs e)
        {
            lock (_frameBuffer) {
                clearBuff();
                //rota += 0.05f;
                Matrix4x4 m = MathUntil.GetRotateX(rota) *
                    MathUntil.GetRotateY(rota) * MathUntil.GetTranslate(0, 0, 10);
                Matrix4x4 v = MathUntil.GetView(_camera.pos, _camera.lookAt, _camera.up);
                Matrix4x4 p = MathUntil.GetProjection(_camera.fov, _camera.aspect, _camera.zn, _camera.zf);
                Draw(m,v,p);

                if (_frameG2 == null) {
                    _frameG2 = CreateGraphics();
                }

                _frameG2.Clear(System.Drawing.Color.Black);
                _frameG2.DrawImage(_frameBuffer, 0, 0);
            }
        }

        private void Draw(Matrix4x4 m, Matrix4x4 v, Matrix4x4 p) {
            _showTrisCount = 0;
            for (int i = 0; i + 2 < _mesh.vertices.Length; i += 3) {
                DrawTriangle(_mesh.vertices[i], _mesh.vertices[i + 1], _mesh.vertices[i + 2], m, v, p);
            }
            Console.WriteLine("显示的三角形数：" + _showTrisCount);
        }

        /// <summary>
        /// 绘制三角形
        /// </summary>
        void DrawTriangle(Vertex p1, Vertex p2, Vertex p3, Matrix4x4 m, Matrix4x4 v, Matrix4x4 p) {
            if (_lightMode == LightMode.ON) {
                //进行顶点光照计算
                Lighting(m, _camera.pos, ref p1);
                Lighting(m, _camera.pos, ref p2);
                Lighting(m, _camera.pos, ref p3);
            }

            //变换到相机空间
            setMVTransform(m, v, ref p1);
            setMVTransform(m, v, ref p2);
            setMVTransform(m, v, ref p3);

            //在相机空间进行背面消隐
            if (!BackFaceCulling(p1, p2, p3)) {
                return;
            }

            //变换到齐次空间
            SetProjectionTransform(p, ref p1);
            SetProjectionTransform(p, ref p2);
            SetProjectionTransform(p, ref p3);

            //裁剪
            //if (!Clip(p1) || !Clip(p2) || !Clip(p3))
            //{
            //    return;
            //}
            if (Clip(p1) == false || Clip(p2) == false || Clip(p3) == false)
            {
                return;
            }

            //变换到屏幕坐标
            TransformToScreen(ref p1);
            TransformToScreen(ref p2);
            TransformToScreen(ref p3);

            //--------------------光栅化阶段---------------------------

            if (_currentMode == RenderMode.Wireframe)
            {
                //线框模式
                BresenhamDrawLine(p1, p2);
                BresenhamDrawLine(p2, p3);
                BresenhamDrawLine(p3, p1);
            }
            else {
                TriangleRasterization(p1, p2, p3);
            }
        }

        /// <summary>
        /// 实现了“基础光照模型”，在世界空间进行顶点光照处理
        /// </summary>
        /// <param name="v"></param>
        private void Lighting(Matrix4x4 m, Vector3D worldEyePositon, ref Vertex v)
        {
            Vector3D worldPoint = v.point * m;//世界空间顶点位置
            Vector3D normal = v.normal * m.Inverse().Transpose();//模型空间法线乘以世界矩阵的逆转置得到世界空间法线
            normal = normal.Normalize();
            RenderData.Color emissiveColor = _mesh.material.emissive;//自发光
            RenderData.Color ambientColor = _ambientColor * _mesh.material.ka;//环境光 

            Vector3D inLightDir = (_light.worldPosition - worldPoint).Normalize();
            float diffuse = Vector3D.Dot(normal, inLightDir);
            if (diffuse < 0)
            {
                diffuse = 0;
            }
            RenderData.Color diffuseColor = _mesh.material.diffuse * diffuse * _light.lightColor;//漫反射
            //
            Vector3D inViewDir = (worldEyePositon - worldPoint).Normalize();
            Vector3D h = (inViewDir + inLightDir).Normalize();
            float specular = 0;
            if (diffuse != 0)
            {//防止出现光源在物体背面产生高光的情况
                specular = (float)System.Math.Pow(MathUntil.Range(Vector3D.Dot(h, normal), 0, 1), _mesh.material.shininess);
            }
            RenderData.Color specularColor = _mesh.material.specular * specular * _light.lightColor;//镜面高光
            //
            v.lightingColor = emissiveColor + ambientColor + diffuseColor + specularColor;
        }

        /// <summary>
        /// 光栅化三角形
        /// </summary>
        private void TriangleRasterization(Vertex p1, Vertex p2, Vertex p3) {
            if (p1.point.y == p2.point.y)
            {
                if (p1.point.y < p3.point.y)
                {//平顶
                    DrawTriangleTop(p1, p2, p3);
                }
                else
                {//平底
                    DrawTriangleBottom(p3, p1, p2);
                }
            }
            else if (p1.point.y == p3.point.y)
            {
                if (p1.point.y < p2.point.y)
                {//平顶
                    DrawTriangleTop(p1, p3, p2);
                }
                else
                {//平底
                    DrawTriangleBottom(p2, p1, p3);
                }
            }
            else if (p2.point.y == p3.point.y)
            {
                if (p2.point.y < p1.point.y)
                {//平顶
                    DrawTriangleTop(p2, p3, p1);
                }
                else
                {//平底
                    DrawTriangleBottom(p1, p2, p3);
                }
            }
            else
            {//分割三角形
                Vertex top;

                Vertex bottom;
                Vertex middle;
                if (p1.point.y > p2.point.y && p2.point.y > p3.point.y)
                {
                    top = p3;
                    middle = p2;
                    bottom = p1;
                }
                else if (p3.point.y > p2.point.y && p2.point.y > p1.point.y)
                {
                    top = p1;
                    middle = p2;
                    bottom = p3;
                }
                else if (p2.point.y > p1.point.y && p1.point.y > p3.point.y)
                {
                    top = p3;
                    middle = p1;
                    bottom = p2;
                }
                else if (p3.point.y > p1.point.y && p1.point.y > p2.point.y)
                {
                    top = p2;
                    middle = p1;
                    bottom = p3;
                }
                else if (p1.point.y > p3.point.y && p3.point.y > p2.point.y)
                {
                    top = p2;
                    middle = p3;
                    bottom = p1;
                }
                else if (p2.point.y > p3.point.y && p3.point.y > p1.point.y)
                {
                    top = p1;
                    middle = p3;
                    bottom = p2;
                }
                else
                {
                    //三点共线
                    return;
                }
                //插值求中间点x
                float middlex = (middle.point.y - top.point.y) * (bottom.point.x - top.point.x) / (bottom.point.y - top.point.y) + top.point.x;
                float dy = middle.point.y - top.point.y;
                float t = dy / (bottom.point.y - top.point.y);
                //插值生成左右顶点
                Vertex newMiddle = new Vertex();
                newMiddle.point.x = middlex;
                newMiddle.point.y = middle.point.y;
                MathUntil.ScreenSpaceLerpVertex(ref newMiddle, top, bottom, t);

                //平底
                DrawTriangleBottom(top, newMiddle, middle);
                //平顶
                DrawTriangleTop(newMiddle, middle, bottom);
            }
        }

        //x = (y-y1) * (x2-x1) / (y2-y1) + x1
        /// <summary>
        /// 平顶，p1,p2,p3为下顶点
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        private void DrawTriangleTop(Vertex p1, Vertex p2, Vertex p3)
        {
            for (float y = p1.point.y; y <= p3.point.y; y += 0.5f)
            {
                int yIndex = (int)(System.Math.Round(y, MidpointRounding.AwayFromZero));
                if (yIndex >= 0 && yIndex < this.MaximumSize.Height)
                {
                    float xl = (y - p1.point.y) * (p3.point.x - p1.point.x) / (p3.point.y - p1.point.y) + p1.point.x;
                    float xr = (y - p2.point.y) * (p3.point.x - p2.point.x) / (p3.point.y - p2.point.y) + p2.point.x;

                    float dy = y - p1.point.y;
                    float t = dy / (p3.point.y - p1.point.y);
                    //插值生成左右顶点
                    Vertex new1 = new Vertex();
                    new1.point.x = xl;
                    new1.point.y = y;
                    MathUntil.ScreenSpaceLerpVertex(ref new1, p1, p3, t);
                    //
                    Vertex new2 = new Vertex();
                    new2.point.x = xr;
                    new2.point.y = y;
                    MathUntil.ScreenSpaceLerpVertex(ref new2, p2, p3, t);
                    //扫描线填充
                    if (new1.point.x < new2.point.x)
                    {
                        ScanlineFill(new1, new2, yIndex);
                    }
                    else
                    {
                        ScanlineFill(new2, new1, yIndex);
                    }
                }
            }
        }
        /// <summary>
        /// 平底，p1为上顶点,p2，p3
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>

        private void DrawTriangleBottom(Vertex p1, Vertex p2, Vertex p3)
        {
            for (float y = p1.point.y; y <= p2.point.y; y += 0.5f)
            {
                int yIndex = (int)(System.Math.Round(y, MidpointRounding.AwayFromZero));
                if (yIndex >= 0 && yIndex < this.MaximumSize.Height)
                {
                    float xl = (y - p1.point.y) * (p2.point.x - p1.point.x) / (p2.point.y - p1.point.y) + p1.point.x;
                    float xr = (y - p1.point.y) * (p3.point.x - p1.point.x) / (p3.point.y - p1.point.y) + p1.point.x;

                    float dy = y - p1.point.y;
                    float t = dy / (p2.point.y - p1.point.y);
                    //插值生成左右顶点
                    Vertex new1 = new Vertex();
                    new1.point.x = xl;
                    new1.point.y = y;
                    MathUntil.ScreenSpaceLerpVertex(ref new1, p1, p2, t);
                    //
                    Vertex new2 = new Vertex();
                    new2.point.x = xr;
                    new2.point.y = y;
                    MathUntil.ScreenSpaceLerpVertex(ref new2, p1, p3, t);
                    //扫描线填充
                    if (new1.point.x < new2.point.x)
                    {
                        ScanlineFill(new1, new2, yIndex);
                    }
                    else
                    {
                        ScanlineFill(new2, new1, yIndex);
                    }
                }
            }
        }

        /// <summary>
        /// 扫描线填充
        /// </summary>
        /// <param name="left">左端点，值已经经过插值</param>
        /// <param name="right">右端点，值已经经过插值</param>
        private void ScanlineFill(Vertex left, Vertex right, int yIndex)
        {
            float dx = right.point.x - left.point.x;
            float step = 1;
            if (dx != 0)
            {
                step = 1 / dx;
            }
            for (float x = left.point.x; x <= right.point.x; x += 0.5f)
            {
                int xIndex = (int)(x + 0.5f);
                if (xIndex >= 0 && xIndex < this.MaximumSize.Width)
                {
                    float lerpFactor = 0;
                    if (dx != 0)
                    {
                        lerpFactor = (x - left.point.x) / dx;
                    }
                    //1/z’与x’和y'是线性关系的
                    float onePreZ = MathUntil.Lerp(left.onePerZ, right.onePerZ, lerpFactor);
                    if (onePreZ >= _zBuffer[yIndex, xIndex])//使用1/z进行深度测试
                    {//通过测试
                        float w = 1 / onePreZ;
                        _zBuffer[yIndex, xIndex] = onePreZ;
                        //uv 插值，求纹理颜色
                        float u = MathUntil.Lerp(left.u, right.u, lerpFactor) * w * (_texture.Width - 1);
                        float v = MathUntil.Lerp(left.v, right.v, lerpFactor) * w * (_texture.Height - 1);
                        //纹理采样
                        RenderData.Color texColor = new RenderData.Color(1, 1, 1);
                        if (_textureFilterMode == TextureFilterMode.Point)
                        {//点采样
                            int uIndex = (int)System.Math.Round(u, MidpointRounding.AwayFromZero);
                            int vIndex = (int)System.Math.Round(v, MidpointRounding.AwayFromZero);
                            uIndex = MathUntil.Range(uIndex, 0, _texture.Width - 1);
                            vIndex = MathUntil.Range(vIndex, 0, _texture.Height - 1);
                            //uv坐标系采用dx风格
                            texColor = new RenderData.Color(ReadTexture(uIndex, vIndex));//转到我们自定义的color进行计算
                        }
                        else if (_textureFilterMode == TextureFilterMode.Bilinear)
                        {//双线性采样
                            float uIndex = (float)System.Math.Floor(u);
                            float vIndex = (float)System.Math.Floor(v);
                            float du = u - uIndex;
                            float dv = v - vIndex;

                            RenderData.Color texcolor1 = new RenderData.Color(ReadTexture((int)uIndex, (int)vIndex)) * (1 - du) * (1 - dv);
                            RenderData.Color texcolor2 = new RenderData.Color(ReadTexture((int)uIndex + 1, (int)vIndex)) * du * (1 - dv);
                            RenderData.Color texcolor3 = new RenderData.Color(ReadTexture((int)uIndex, (int)vIndex + 1)) * (1 - du) * dv;
                            RenderData.Color texcolor4 = new RenderData.Color(ReadTexture((int)uIndex + 1, (int)vIndex + 1)) * du * dv;
                            texColor = texcolor1 + texcolor2 + texcolor3 + texcolor4;
                        }

                        //插值顶点颜色
                        RenderData.Color vertColor = MathUntil.Lerp(left.vcolor, right.vcolor, lerpFactor) * w;
                        //插值光照颜色
                        RenderData.Color lightColor = MathUntil.Lerp(left.lightingColor, right.lightingColor, lerpFactor) * w; ;


                        if (_lightMode == LightMode.ON)
                        {//光照模式，需要混合光照的颜色
                            if (RenderMode.Textured == _currentMode)
                            {
                                RenderData.Color finalColor = texColor * lightColor;
                                _frameBuffer.SetPixel(xIndex, yIndex, finalColor.TransFormToSystemColor());
                            }
                            else if (RenderMode.VertexColor == _currentMode)
                            {
                                RenderData.Color finalColor = vertColor * lightColor;
                                _frameBuffer.SetPixel(xIndex, yIndex, finalColor.TransFormToSystemColor());
                            }
                        }
                        else
                        {
                            if (RenderMode.Textured == _currentMode)
                            {
                                _frameBuffer.SetPixel(xIndex, yIndex, texColor.TransFormToSystemColor());
                            }
                            else if (RenderMode.VertexColor == _currentMode)
                            {
                                _frameBuffer.SetPixel(xIndex, yIndex, vertColor.TransFormToSystemColor());
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 使用bresenham算法绘制直线
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        private void BresenhamDrawLine(Vertex p1, Vertex p2) {
            int x = (int)(System.Math.Round(p1.point.x, MidpointRounding.AwayFromZero));
            int y = (int)(System.Math.Round(p1.point.y, MidpointRounding.AwayFromZero));
            int dx = (int)(System.Math.Round(p2.point.x - p1.point.x, MidpointRounding.AwayFromZero));
            int dy = (int)(System.Math.Round(p2.point.y - p1.point.y, MidpointRounding.AwayFromZero));
            int stepx = 1;
            int stepy = 1;

            if (dx >= 0)
            {
                stepx = 1;
            }
            else
            {
                stepx = -1;
                dx = System.Math.Abs(dx);
            }

            if (dy >= 0)
            {
                stepy = 1;
            }
            else
            {
                stepy = -1;
                dy = System.Math.Abs(dy);
            }

            int dx2 = 2 * dx;
            int dy2 = 2 * dy;

            if (dx > dy)
            {
                int error = dy2 - dx;
                for (int i = 0; i <= dx; i++)
                {
                    _frameBuffer.SetPixel(x, y, System.Drawing.Color.White);
                    if (error >= 0)
                    {
                        error -= dx2;
                        y += stepy;
                    }
                    error += dy2;
                    x += stepx;
                }
            }
            else
            {
                int error = dx2 - dy;
                for (int i = 0; i <= dy; i++)
                {
                    _frameBuffer.SetPixel(x, y, System.Drawing.Color.White);
                    if (error >= 0)
                    {
                        error -= dy2;
                        x += stepx;
                    }
                    error += dx2;
                    y += stepy;
                }
            }
        }

        /// <summary>
        /// 从齐次坐标系变换到屏幕坐标
        /// </summary>
        /// <param name="v"></param>
        private void TransformToScreen(ref Vertex v) {
            if (v.point.w != 0) {
                //先进行透视除法，转到cvv
                v.point.x *= 1 / v.point.w;
                v.point.y *= 1 / v.point.w;
                v.point.z *= 1 / v.point.w;
                v.point.w = 1;
                //cvv到屏幕坐标
                v.point.x = (v.point.x + 1) * 0.5f * this.MaximumSize.Width;
                v.point.y = (1 - v.point.y) * 0.5f * this.MaximumSize.Height;
            }
        }

        /// <summary>
        /// 检查是否裁剪这个顶点,简单的cvv裁剪,在透视除法之前
        /// </summary>
        /// <param name="v"></param>
        bool Clip(Vertex v) {
            //cvv为 x-1,1  y-1,1  z0,1
            if (v.point.x >= -v.point.w && v.point.x <= v.point.w &&
               v.point.y >= -v.point.w && v.point.y <= v.point.w &&
               v.point.z >= 0f && v.point.z <= v.point.w)
            {
                return true;
            }
            return false;
        }



        /// <summary>
        /// 变换到齐次空间
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vertex"></param>
        void SetProjectionTransform(Matrix4x4 p, ref Vertex vertex) {
            vertex.point = vertex.point * p;
            //得到齐次裁剪空间的点 v.point.w 中保存着原来的z(具体是z还是-z要看使用的投影矩阵,我们使用投影矩阵是让w中保存着z)


            //onePerZ 保存1/z，方便之后对1/z关于x’、y’插值得到1/z’ 
            vertex.onePerZ = 1 / vertex.point.w;
            //校正的推论： s/z、t/z和x’、y’也是线性关系。而我们之前知道1/z和x’、y’是线性关系。则我们得出新的思路：对1/z关于x’、y’插值得到1/z’，然后对s/z、t/z关于x’、y’进行插值得到s’/z’、t’/z’，然后用s’/z’和t’/z’分别除以1/z’，就得到了插值s’和t’
            //这里将需要插值的信息都乘以1/z 得到 s/z和t/z等，方便光栅化阶段进行插值
            vertex.u *= vertex.onePerZ;
            vertex.v *= vertex.onePerZ;
            //
            vertex.vcolor *= vertex.onePerZ;
            //
            vertex.lightingColor *= vertex.onePerZ;
        }

        /// <summary>
        /// 判断三角面是否在视野内
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        bool BackFaceCulling(Vertex p1,Vertex p2, Vertex p3) {
            if (_currentMode == RenderMode.Wireframe)
            {//线框模式不进行背面消隐
                return true;
            }
            else
            {
                Vector3D v1 = p2.point - p1.point;
                Vector3D v2 = p3.point - p2.point;
                Vector3D normal = Vector3D.Cross(v1, v2);
                //由于在视空间中，所以相机点就是（0,0,0）
                Vector3D viewDir = p1.point - new Vector3D(0, 0, 0);
                if (Vector3D.Dot(normal, viewDir) > 0)
                {
                    _showTrisCount++;
                    return true;
                }
                return false;
            }
        }

        void setMVTransform(Matrix4x4 m,Matrix4x4 v,ref Vertex vertex) {
            vertex.point = vertex.point * m * v;
        }
        
        /// <summary>
        /// 清缓存
        /// </summary>
        void clearBuff() {
            _frameG1.Clear(System.Drawing.Color.Black);
            Array.Clear(_zBuffer, 0, _zBuffer.Length);
        }

        /// <summary>
        /// 在没有加载到贴图是初始化texture，白或蓝
        /// </summary>
        void initTexture() {
            for (int i = 0; i < 256; i++) {
                for (int j = 0; j < 256; j++) {
                    _texture.SetPixel(i, j, ((i + j) % 32 == 0) ? System.Drawing.Color.White : System.Drawing.Color.Green);
                }
            }
        }

        private System.Drawing.Color ReadTexture(int uIndex, int vIndex)
        {
            int u = MathUntil.Range(uIndex, 0, _texture.Width - 1);
            int v = MathUntil.Range(vIndex, 0, _texture.Height - 1);
            return _texture.GetPixel(u, v);
        }

    }
}
