using System.Runtime.InteropServices;
using System.Text;
using _3dEngine.Inputs.Interfaces;
using _3dEngine.Interfaces;

namespace _3dEngine.Inputs.Implementations;

internal class User32InputProvider : IInputProvider
{
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);
    
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

    private const uint GW_OWNER = 4;
    
    public bool IsAvailable { get; private set; }
    public string? InitializationError { get; private set; }
    
    private bool _isFocused;
    
    public User32InputProvider()
    {
        try
        {
            _ = GetAsyncKeyState(0);
            _ = GetForegroundWindow();
            _ = GetConsoleWindow();
            _ = IsChild(IntPtr.Zero, IntPtr.Zero);
            _ = GetWindow(IntPtr.Zero, GW_OWNER);
            IsAvailable = true;
        }
        catch (DllNotFoundException)
        {
            InitializationError = "WARNING: Core Windows libraries (user32.dll or kernel32.dll) not found.";
            IsAvailable = false;
        }
        catch (EntryPointNotFoundException)
        {
            InitializationError = "WARNING: Required Windows API entry points (GetConsoleWindow/GetAsyncKeyState) not found.";
            IsAvailable = false;
        }
        catch (Exception ex)
        {
            InitializationError = $"WARNING: Windows Input failed to initialize: {ex.Message}";
            IsAvailable = false;
        }
    }
    
    public void Update()
    {
        _isFocused = IsAppFocused();
    }

    public bool IsGetKey(ConsoleKey key)
    {
        if(!_isFocused) return false;
        
        return (GetAsyncKeyState((int)key) & 0x8000) != 0;
    }

    public bool IsGetKey(int virtualKey)
    {
        if(!_isFocused) return false;
        
        return (GetAsyncKeyState(virtualKey) & 0x8000) != 0;
    }

    
    public bool IsShift => IsGetKey(0x10);
    public bool IsCtrl => IsGetKey(0x11);
    public bool IsAlt => IsGetKey(0x12);


    private bool IsAppFocused()
    {
        IntPtr foreground = GetForegroundWindow();
        IntPtr console = GetActualConsoleWindow();

        if (foreground == IntPtr.Zero || console == IntPtr.Zero) return false;

        return foreground == console;
    }
    
    private IntPtr GetActualConsoleWindow()
    {
        IntPtr console = GetConsoleWindow();
            
        IntPtr owner = GetWindow(console, GW_OWNER);
            
        return owner != IntPtr.Zero ? owner : console;
    }

    public void Dispose()
    {}
}