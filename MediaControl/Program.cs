using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal class Program
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;

    private static LowLevelKeyboardProc _callback = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    private static void Main()
    {
        _hookID = SetHook(_callback);
        Application.Run();
        UnhookWindowsHookEx(_hookID);
    }

    private static IntPtr SetHook(LowLevelKeyboardProc callback)
    {
        Console.WriteLine("Running");
        return SetWindowsHookEx(WH_KEYBOARD_LL, callback, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            Keys key = (Keys)vkCode;

            if (key is Keys.Pause)
            {
                keybd_event((int)Keys.MediaPlayPause, 0, 0, 0);
                Console.WriteLine("Play/Pause");
            }
            else if (key is Keys.PageUp)
            {
                keybd_event((int)Keys.MediaNextTrack, 0, 0, 0);
                Console.WriteLine("Next Track");

            }
            else if(key is Keys.PageDown)
            {
                keybd_event((int)Keys.MediaPreviousTrack, 0, 0, 0);
                Console.WriteLine("Previous Track");
            }

        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);


    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);


    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern void keybd_event(uint bVk, uint bScan, uint dwFlags, uint dwExtraInfo);


}