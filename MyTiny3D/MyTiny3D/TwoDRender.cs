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
            drawLine2(new Point(1, 1), new Point(100, 10), Color.White);
        }

        /// <summary>
        /// 直接画线法，效率低
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="col"></param>
        void drawLine1(Point start, Point end, Color col) {
            int dx = end.X - start.X;
            int dy = end.Y - start.Y;
            int stepY = dy / dx;
            for (int i = start.X; i < end.X; i++)
            {
                _frameBuff.SetPixel(i, i * stepY, col);
            }
        }

        /// <summary>
        /// Bresenham算法画线
        /// </summary>
        void drawLine2(Point start, Point end,Color col) {
            int x, y, dx, dy, d;
            y = start.Y;
            dx = end.X - start.X;
            dy = end.Y - start.Y;
            d = 2 * dy - dx;
            for (x = start.X; x < end.X; x++)
            {
                _frameBuff.SetPixel(x, y, col);
                if (d < 0)
                {
                    d += 2 * dy;
                }
                else
                {
                    y++;
                    d += 2 * dy - 2 * dx;
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
