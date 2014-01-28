using System;
using System.IO;
using System.Windows.Forms;

namespace Horse_Creator
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
