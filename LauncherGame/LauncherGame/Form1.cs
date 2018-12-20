using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LauncherGame
{
    /// <summary>
    /// Ejemplo simulado lanzar procesos con timer
    /// Luis Marino Martinez
    /// </summary>
    public partial class Form1 : Form
    {

        //Juego que se quiere ejecutar
        Process process;

        //Timer del juego
        System.Timers.Timer aTimer;

       [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPlacement(IntPtr hWnd,
           [In] ref WINDOWPLACEMENT lpwndpl);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPLACEMENT
        {
            public int Length;
            public int Flags;
            public ShowWindowCommands ShowCmd;
            public POINT MinPosition;
            public POINT MaxPosition;
            public RECT NormalPosition;
            public static WINDOWPLACEMENT Default
            {
                get
                {
                    WINDOWPLACEMENT result = new WINDOWPLACEMENT();
                    result.Length = Marshal.SizeOf(result);
                    return result;
                }
            }
        }

        internal enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            ShowMinimized = 2,
            Maximize = 3, // is this the right value?
            ShowMaximized = 3,
            ShowNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNA = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimize = 11
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            private int _Left;
            private int _Top;
            private int _Right;
            private int _Bottom;
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            process = new Process();
            //Nombre del proceso en este caso el juego
            process.StartInfo.FileName = "notepad.exe";//"D:\\lmartinez\\Downloads\\AkelPadPortable\\AkelPadPortable.exe";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            process.Start();
           
           

            ///Timer de tiempo del juego
            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += ATimer_Elapsed;
            aTimer.Interval = 5000; ///El tiempo de juego establecido (expresado en milisegundos)
            aTimer.Enabled = true;            


        }

        private void ATimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            aTimer.Stop();
            //Obtenemos la clave de la ventana
            IntPtr target_hwnd = process.MainWindowHandle;//FindWindowByCaption(IntPtr.Zero, "AkelPad"); //IntPtr.Zero;


            //Validamos si la encontramos
            if (target_hwnd == IntPtr.Zero)
            {
                MessageBox.Show("No se encontró la ventana");
                return;
            }


            //Se prepara la estructura de ubicación de la ventana
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.Length = Marshal.SizeOf(placement);

            // Se obtiene la ubicación actual
            GetWindowPlacement(target_hwnd, out placement);

            //Posibles comandos de lo que se quiera hacer con la vetana
            //placement.ShowCmd = ShowWindowCommands.ShowMaximized;
            placement.ShowCmd = ShowWindowCommands.ShowMinimized;
            //placement.ShowCmd = ShowWindowCommands.Normal;

            // Ejecutar la acción
            SetWindowPlacement(target_hwnd, ref placement);

            MessageBox.Show("El tiempo de tu juego se ha terminado");
            process.Kill();
        }
    }
}
