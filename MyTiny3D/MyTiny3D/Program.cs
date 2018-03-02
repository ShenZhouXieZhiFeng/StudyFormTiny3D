using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MyTiny3D
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //2d渲染
            //Application.Run(new TwoDRender());
            //3d渲染
            Application.Run(new SoftRendererDemo());
        }
    }
}
