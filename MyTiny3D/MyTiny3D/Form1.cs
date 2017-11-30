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
        private Bitmap _texture;
        private Bitmap _frameBuffer;
        private Graphics _frameG1;
        private Graphics _frameG2;
        private float[,] _zBuffer;

        private Color _ambientColor;

        public SoftRendererDemo()
        {
            InitializeComponent();

            try
            {
                //载入贴图
                Image img = Image.FromFile("../../Texture/texture.jpg");
                _texture = new Bitmap(img, 256, 256);
            }
            catch (Exception e) {
                _texture = new Bitmap(256, 256);

            }

            //System.Timers.Timer mainTimer = new System.Timers.Timer(1000 / 60f);
            //mainTimer.Elapsed += new ElapsedEventHandler(drawLine);
            //mainTimer.AutoReset = true;
            //mainTimer.Enabled = true;
            //mainTimer.Start();
        }

        /// <summary>
        /// 在没有加载到贴图是初始化texture，白或蓝
        /// </summary>
        void initTexture() {
            for (int i = 0; i < 256; i++) {
                for (int j = 0; j < 256; j++) {
                    _texture.SetPixel(i, j, ((i + j) % 32 == 0) ? Color.White : Color.Green);
                }
            }
        }

    }
}
