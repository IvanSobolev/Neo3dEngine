using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _3dEngine;
public class Input
{
    [DllImport("User32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    public static bool IsGetKey(ConsoleKey key)
    {
        if (!IsAppFocused()) return false;
        
        return (GetAsyncKeyState((int)key) & 0x8000) != 0;
    }
    
    public static bool IsGetKey(int virtualKey)
    {
        if (!IsAppFocused()) return false;

        return (GetAsyncKeyState(virtualKey) & 0x8000) != 0;
    }
    
    private static bool IsAppFocused()
    {
        IntPtr handle = GetForegroundWindow();
        if (handle == IntPtr.Zero) return false;

        const int nChars = 256;
        StringBuilder buffer = new StringBuilder(nChars);
        
        if (GetWindowText(handle, buffer, nChars) > 0)
        {
            string activeWindowTitle = buffer.ToString();
            
            return activeWindowTitle.Contains(Console.Title);
        }

        return false;
    }

    public static bool IsShift => IsGetKey(0x10);
    public static bool IsCtrl  => IsGetKey(0x11);
    public static bool IsAlt   => IsGetKey(0x12);
}
