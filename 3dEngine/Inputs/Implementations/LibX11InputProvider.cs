using System.Runtime.InteropServices;
using _3dEngine.Inputs.Interfaces;

namespace _3dEngine.Inputs.Implementations;

internal class LibX11InputProvider : IInputProvider
{
    [DllImport("libX11.so.6")]
    private static extern IntPtr XOpenDisplay(IntPtr displayName);
    
    [DllImport("libX11.so.6")]
    private static extern int XQueryKeymap(IntPtr display, [Out] byte[] keys_return);
    
    [DllImport("libX11.so.6")]
    private static extern int XCloseDisplay(IntPtr display);
    
    [DllImport("libX11.so.6")]
    private static extern byte XKeysymToKeycode(IntPtr display, IntPtr keysym);
    
    [DllImport("libX11.so.6")]
    private static extern int XGetInputFocus(IntPtr display, out IntPtr focus_return, out int revert_to_return);

    [DllImport("libX11.so.6")]
    private static extern int XFetchName(IntPtr display, IntPtr window, out IntPtr window_name_return);

    [DllImport("libX11.so.6")]
    private static extern int XFree(IntPtr data);

    [DllImport("libX11.so.6")]
    private static extern int XQueryTree(IntPtr display, IntPtr window, out IntPtr root_return, out IntPtr parent_return, out IntPtr children_return, out int nchildren_return);
    
    
    private IntPtr _display;
    private byte[] _keyState = new byte[32];
    private readonly int[] _keyCodeCache = new int[256];
    private int lShiftCode;
    private int rShiftCode;
    private int lCtrlCode;
    private int rCtrlCode;
    private int lAltCode;
    private int rAltCode;
    
    public bool IsAvailable { get; private set; }
    public string? InitializationError { get; private set; }
    
    
    private readonly IntPtr _myWindowId = IntPtr.Zero;

    public LibX11InputProvider()
    {
        for (int i = 0; i < _keyCodeCache.Length; i++)
        {
            _keyCodeCache[i] = -1;
        }
        
        string? windowIdEnv = Environment.GetEnvironmentVariable("WINDOWID");
        if (!string.IsNullOrEmpty(windowIdEnv) && long.TryParse(windowIdEnv, out long winId))
        {
            _myWindowId = (IntPtr)winId;
        }
        
        try
        {
            _display = XOpenDisplay(IntPtr.Zero);

            if (_display == IntPtr.Zero)
            {
                InitializationError = "WARNING: No active X11 display detected (headless/server environment?).";
                IsAvailable = false;
                return;
            }
            BuildKeyCodeCache();
            IsAvailable = true;
        }
        catch (DllNotFoundException)
        {
            InitializationError = "WARNING: 'libX11.so.6' is missing. Controls might be less responsive.\n" +
                                  "-> Fix this by running: sudo apt install libx11-6";
            IsAvailable = false;
        }
    }

    public void Update()
    {
        if (IsAvailable && _display != IntPtr.Zero && IsAppFocused())
        {
            XQueryKeymap(_display, _keyState);
        }
        else
        {
            Array.Clear(_keyState, 0, _keyState.Length);
        }
    }

    public bool IsGetKey(ConsoleKey key)
    {
        int consoleKeyIndex = (int)key;
        if(consoleKeyIndex < 0 || consoleKeyIndex >= _keyCodeCache.Length) return false;
        
        int keyCode = _keyCodeCache[consoleKeyIndex];
        if(keyCode == -1) return false;
        
        return (_keyState[keyCode / 8] & (1 << (keyCode % 8))) != 0;
    }

    public bool IsGetKey(int virtualKey)
    {
        return IsGetKey((ConsoleKey)virtualKey);
    }

    public bool IsShift => IsPressed(rShiftCode) || IsPressed(lShiftCode);
    public bool IsCtrl  => IsPressed(rCtrlCode) || IsPressed(lCtrlCode);
    public bool IsAlt   => IsPressed(rAltCode) || IsPressed(lAltCode);

    private bool IsPressed(int keyCode)
    {
        if (keyCode <= 0 || keyCode >= 256) return false;
        return (_keyState[keyCode / 8] & (1 << (keyCode % 8))) != 0;
    }
    
    private bool IsAppFocused()
    {
        if(_display == IntPtr.Zero) return false;
        
        XGetInputFocus(_display, out IntPtr focusedWindow, out _);
        if (focusedWindow == IntPtr.Zero) return false;
        
        if (_myWindowId != IntPtr.Zero)
        {
            IntPtr current = focusedWindow;
            while (current != IntPtr.Zero)
            {
                if (current == _myWindowId) return true;

                if (XQueryTree(_display, current, out _, out IntPtr parent, out IntPtr children, out _) != 0)
                {
                    if (children != IntPtr.Zero) XFree(children);
                    current = parent;
                }
                else break;
            }
            return false;
        }
        
        IntPtr curr = focusedWindow;
        while (curr != IntPtr.Zero)
        {
            string? title = null;
            if (XFetchName(_display, curr, out IntPtr namePtr) != 0 && namePtr != IntPtr.Zero)
            {
                title = Marshal.PtrToStringAnsi(namePtr);
                XFree(namePtr);
            }
            
            if (!string.IsNullOrEmpty(title))
            {
                return title.Contains(Frame.Title, StringComparison.OrdinalIgnoreCase);
            }

            if (XQueryTree(_display, curr, out _, out IntPtr parent, out IntPtr children, out _) != 0)
            {
                if (children != IntPtr.Zero) XFree(children);
                curr = parent;
            }
            else break;
        }
        
        return false;
    }
    
    
    private void BuildKeyCodeCache()
    {
        foreach (var kvp in ConsoleKeyToKeysymMap)
        {
            int consoleKeyIndex = (int)kvp.Key;
            if (consoleKeyIndex >= 0 && consoleKeyIndex < _keyCodeCache.Length)
            {
                byte physicalKeycode = XKeysymToKeycode(_display, kvp.Value);
                _keyCodeCache[consoleKeyIndex] = physicalKeycode != 0 ? physicalKeycode : -1;
            }
        }

        rShiftCode = XKeysymToKeycode(_display, (IntPtr)0xffe1);
        lShiftCode = XKeysymToKeycode(_display, (IntPtr)0xffe2);
        rCtrlCode = XKeysymToKeycode(_display, (IntPtr)0xffe3);
        lCtrlCode = XKeysymToKeycode(_display, (IntPtr)0xffe4);
        rAltCode = XKeysymToKeycode(_display, (IntPtr)0xffe9);
        lAltCode = XKeysymToKeycode(_display, (IntPtr)0xffea);
    }

    ~LibX11InputProvider()
    {
        if (_display != IntPtr.Zero) 
        {
            //todo logs
        }
    }

    public void Dispose()
    {
        if (_display != IntPtr.Zero)
        {
            XCloseDisplay(_display);
            _display = IntPtr.Zero;
        }
        GC.SuppressFinalize(this);
    }

    #region КАРТА СОПОСТАВЛЕНИЯ CONSOLEKEY -> X11 KEYSYM

        // Мы сопоставляем ConsoleKey со значениями из keysymdef.h
        private static readonly Dictionary<ConsoleKey, IntPtr> ConsoleKeyToKeysymMap = new()
        {
            // Системные и управляющие клавиши
            { ConsoleKey.Backspace, (IntPtr)0xff08 },
            { ConsoleKey.Tab, (IntPtr)0xff09 },
            { ConsoleKey.Clear, (IntPtr)0xff0b },
            { ConsoleKey.Enter, (IntPtr)0xff0d },
            { ConsoleKey.Pause, (IntPtr)0xff13 },
            { ConsoleKey.Escape, (IntPtr)0xff1b },
            { ConsoleKey.Spacebar, (IntPtr)0x0020 },
            { ConsoleKey.PageUp, (IntPtr)0xff55 },
            { ConsoleKey.PageDown, (IntPtr)0xff56 },
            { ConsoleKey.End, (IntPtr)0xff57 },
            { ConsoleKey.Home, (IntPtr)0xff50 },
            { ConsoleKey.LeftArrow, (IntPtr)0xff51 },
            { ConsoleKey.UpArrow, (IntPtr)0xff52 },
            { ConsoleKey.RightArrow, (IntPtr)0xff53 },
            { ConsoleKey.DownArrow, (IntPtr)0xff54 },
            { ConsoleKey.Select, (IntPtr)0xff60 },
            { ConsoleKey.Print, (IntPtr)0xff61 },
            { ConsoleKey.Execute, (IntPtr)0xff62 },
            { ConsoleKey.PrintScreen, (IntPtr)0xff61 },
            { ConsoleKey.Insert, (IntPtr)0xff63 },
            { ConsoleKey.Delete, (IntPtr)0xffff },
            { ConsoleKey.Help, (IntPtr)0xff6a },
            { ConsoleKey.Applications, (IntPtr)0xff67 }, 

            // Цифры (основная клавиатура)
            { ConsoleKey.D0, (IntPtr)0x0030 },
            { ConsoleKey.D1, (IntPtr)0x0031 },
            { ConsoleKey.D2, (IntPtr)0x0032 },
            { ConsoleKey.D3, (IntPtr)0x0033 },
            { ConsoleKey.D4, (IntPtr)0x0034 },
            { ConsoleKey.D5, (IntPtr)0x0035 },
            { ConsoleKey.D6, (IntPtr)0x0036 },
            { ConsoleKey.D7, (IntPtr)0x0037 },
            { ConsoleKey.D8, (IntPtr)0x0038 },
            { ConsoleKey.D9, (IntPtr)0x0039 },

            // Алфавит (A-Z) - маппим на строчные KeySyms, X11 сам найдет их физические клавиши
            { ConsoleKey.A, (IntPtr)0x0061 },
            { ConsoleKey.B, (IntPtr)0x0062 },
            { ConsoleKey.C, (IntPtr)0x0063 },
            { ConsoleKey.D, (IntPtr)0x0064 },
            { ConsoleKey.E, (IntPtr)0x0065 },
            { ConsoleKey.F, (IntPtr)0x0066 },
            { ConsoleKey.G, (IntPtr)0x0067 },
            { ConsoleKey.H, (IntPtr)0x0068 },
            { ConsoleKey.I, (IntPtr)0x0069 },
            { ConsoleKey.J, (IntPtr)0x006a },
            { ConsoleKey.K, (IntPtr)0x006b },
            { ConsoleKey.L, (IntPtr)0x006c },
            { ConsoleKey.M, (IntPtr)0x006d },
            { ConsoleKey.N, (IntPtr)0x006e },
            { ConsoleKey.O, (IntPtr)0x006f },
            { ConsoleKey.P, (IntPtr)0x0070 },
            { ConsoleKey.Q, (IntPtr)0x0071 },
            { ConsoleKey.R, (IntPtr)0x0072 },
            { ConsoleKey.S, (IntPtr)0x0073 },
            { ConsoleKey.T, (IntPtr)0x0074 },
            { ConsoleKey.U, (IntPtr)0x0075 },
            { ConsoleKey.V, (IntPtr)0x0076 },
            { ConsoleKey.W, (IntPtr)0x0077 },
            { ConsoleKey.X, (IntPtr)0x0078 },
            { ConsoleKey.Y, (IntPtr)0x0079 },
            { ConsoleKey.Z, (IntPtr)0x007a },

            // Нампад
            { ConsoleKey.NumPad0, (IntPtr)0xffb0 },
            { ConsoleKey.NumPad1, (IntPtr)0xffb1 },
            { ConsoleKey.NumPad2, (IntPtr)0xffb2 },
            { ConsoleKey.NumPad3, (IntPtr)0xffb3 },
            { ConsoleKey.NumPad4, (IntPtr)0xffb4 },
            { ConsoleKey.NumPad5, (IntPtr)0xffb5 },
            { ConsoleKey.NumPad6, (IntPtr)0xffb6 },
            { ConsoleKey.NumPad7, (IntPtr)0xffb7 },
            { ConsoleKey.NumPad8, (IntPtr)0xffb8 },
            { ConsoleKey.NumPad9, (IntPtr)0xffb9 },
            { ConsoleKey.Multiply, (IntPtr)0xffaa },
            { ConsoleKey.Add, (IntPtr)0xffab },
            { ConsoleKey.Subtract, (IntPtr)0xffad },
            { ConsoleKey.Decimal, (IntPtr)0xffae },
            { ConsoleKey.Divide, (IntPtr)0xffaf },

            // Функциональные клавиши (F1-F24)
            { ConsoleKey.F1, (IntPtr)0xffbe },
            { ConsoleKey.F2, (IntPtr)0xffbf },
            { ConsoleKey.F3, (IntPtr)0xffc0 },
            { ConsoleKey.F4, (IntPtr)0xffc1 },
            { ConsoleKey.F5, (IntPtr)0xffc2 },
            { ConsoleKey.F6, (IntPtr)0xffc3 },
            { ConsoleKey.F7, (IntPtr)0xffc4 },
            { ConsoleKey.F8, (IntPtr)0xffc5 },
            { ConsoleKey.F9, (IntPtr)0xffc6 },
            { ConsoleKey.F10, (IntPtr)0xffc7 },
            { ConsoleKey.F11, (IntPtr)0xffc8 },
            { ConsoleKey.F12, (IntPtr)0xffc9 },
            { ConsoleKey.F13, (IntPtr)0xffca },
            { ConsoleKey.F14, (IntPtr)0xffcb },
            { ConsoleKey.F15, (IntPtr)0xffcc },
            { ConsoleKey.F16, (IntPtr)0xffcd },
            { ConsoleKey.F17, (IntPtr)0xffce },
            { ConsoleKey.F18, (IntPtr)0xffcf },
            { ConsoleKey.F19, (IntPtr)0xffd0 },
            { ConsoleKey.F20, (IntPtr)0xffd1 },
            { ConsoleKey.F21, (IntPtr)0xffd2 },
            { ConsoleKey.F22, (IntPtr)0xffd3 },
            { ConsoleKey.F23, (IntPtr)0xffd4 },
            { ConsoleKey.F24, (IntPtr)0xffd5 },

            // { ConsoleKey.NumLock, (IntPtr)0xff7f },
            // { ConsoleKey.ScrollLock, (IntPtr)0xff14 },
            // { ConsoleKey.LeftShift, (IntPtr)0xffe1 },
            // { ConsoleKey.RightShift, (IntPtr)0xffe2 },
            // { ConsoleKey.LeftCtrl, (IntPtr)0xffe3 },
            // { ConsoleKey.RightCtrl, (IntPtr)0xffe4 },
            // { ConsoleKey.LeftAlt, (IntPtr)0xffe9 },
            // { ConsoleKey.RightAlt, (IntPtr)0xffea },
            
            { ConsoleKey.LeftWindows, (IntPtr)0xffeb }, // Super_L
            { ConsoleKey.RightWindows, (IntPtr)0xffec }, // Super_R

            // Знаки препинания (OEM-клавиши)
            { ConsoleKey.Oem1, (IntPtr)0x003b },      // Semicolon (;)
            { ConsoleKey.OemPlus, (IntPtr)0x002b },   // Plus (+)
            { ConsoleKey.OemComma, (IntPtr)0x002c },  // Comma (,)
            { ConsoleKey.OemMinus, (IntPtr)0x002d },  // Minus (-)
            { ConsoleKey.OemPeriod, (IntPtr)0x002e }, // Period (.)
            { ConsoleKey.Oem2, (IntPtr)0x002f },      // Slash (/)
            { ConsoleKey.Oem3, (IntPtr)0x0060 },      // Grave/Tilde (`)
            { ConsoleKey.Oem4, (IntPtr)0x005b },      // Left bracket ([)
            { ConsoleKey.Oem5, (IntPtr)0x005c },      // Backslash (\)
            { ConsoleKey.Oem6, (IntPtr)0x005d },      // Right bracket (])
            { ConsoleKey.Oem7, (IntPtr)0x0027 },      // Apostrophe (')
            { ConsoleKey.Oem102, (IntPtr)0x005c }     // Дополнительный обратный слэш на европейских раскладках
        };

        #endregion
}