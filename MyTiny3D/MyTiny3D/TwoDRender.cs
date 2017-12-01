using MyTiny3D.Math;
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
    public partial class TwoDRender : Form
    {
        private Graphics _frameG1;//画笔1
        private Graphics _frameG2;//画笔2
        private Bitmap _frameBuff;

        public TwoDRender()
        {
            InitializeComponent();
            initData();

            _frameBuff = new Bitmap(this.MaximumSize.Width, this.MaximumSize.Height);
            _frameG1 = Graphics.FromImage(_frameBuff);
            _frameG2 = CreateGraphics();

            System.Timers.Timer mainTimer = new System.Timers.Timer(1000 / 100f);
            mainTimer.Elapsed += new ElapsedEventHandler(Tick);
            mainTimer.AutoReset = true;
            mainTimer.Enabled = true;
            mainTimer.Start();
        }

        private void Tick(object sender, ElapsedEventArgs e) {
            lock (_frameBuff) {
                clear();

                _frameG2.Clear(Color.Black);

                draw();

                _frameG2.DrawImage(_frameBuff, 0, 0);
            }
        }

        void draw() {
            //for (int i = 0; i < _points.Length; i++) {
            //    _frameBuff.SetPixel((int)_points[i].x, (int)_points[i].y, Color.White);
            //}
            drawLine(new Point2D(5, 20), new Point2D(30, 50), Color.White);
        }

        /// <summary>
        /// 根据起点和终点的坐标画直线
        /// </summary>
        void drawLine(Point2D start,Point2D end,Color col) {
            for (int i = (int)start.x; i < (int)end.x; i++) {
                for (int j = (int)start.y; j < (int)end.y; j++) {
                    _frameBuff.SetPixel(i, j, col);
                }
            }
        }

        void initData() {
            //_points = new Point2D[10];
            //for (int i = 0; i < 10; i++) {
            //    _points[i] = new Point2D(i, i);
            //}
        }

        void clear() {
            _frameG1.Clear(Color.Black);
        }
    }
}
