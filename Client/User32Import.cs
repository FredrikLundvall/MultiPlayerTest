using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace BlowtorchesAndGunpowder
{
    public class User32Import
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern short GetKeyState(int nVirtKey);
        [DllImport("user32.dll")]
        public static extern int GetKeyboardState(byte[] keystate);
        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool PeekMessage(out pkMessage msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);
        //How to use:
        //private void Form1_KeyDown(object sender, KeyEventArgs e)
        //{
        //    //Array has to be long enough for the keys to be checked for
        //    byte[] keys = new byte[256];

        //    GetKeyboardState(keys);

        //    if ((keys[(int)Keys.Up] & keys[(int)Keys.Right] & 128) == 128)
        //    {
        //        Console.WriteLine("Up Arrow key and Right Arrow key down.");
        //    }
        //}
        public static bool GetKeyState(System.Windows.Forms.Keys aKeyToCheck)
        {
            //return GetKeyState((int)aKeyToCheck) & 0x8000;
            return GetKeyState((int)aKeyToCheck) < 0;
        }

        public static bool AppStillIdle()
        {
            pkMessage msg;
            return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct pkMessage
        {
            public IntPtr hWnd;
            public System.Windows.Forms.Message msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }

    }
}
