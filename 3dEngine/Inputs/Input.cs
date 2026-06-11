using System.Runtime.InteropServices;
using _3dEngine.Inputs.Implementations;
using _3dEngine.Inputs.Interfaces;
using _3dEngine.Interfaces;
using System;

namespace _3dEngine.Inputs;
public static class Input
{
    private static readonly IInputProvider Provider;
    
    public static bool IsPollingEnabled { get; set; } = true; 
    
    public static string WarningMessage { get; private set; } = string.Empty;

    static Input()
    {
        IInputProvider? selectedProvider = null;
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var winProvider = new User32InputProvider();
            if (winProvider.IsAvailable)
            {
                Provider = winProvider;
            }
            else
            {
                WarningMessage = winProvider.InitializationError!;
            }
        }
        else
        {
            string? sessionType = Environment.GetEnvironmentVariable("XDG_SESSION_TYPE");
            string? waylandDisplay = Environment.GetEnvironmentVariable("WAYLAND_DISPLAY");
            
            if (sessionType == "wayland" && !string.IsNullOrEmpty(waylandDisplay))
            {
                WarningMessage = "WARNING: Wayland session detected. Direct X11 input polling is restricted by OS security.\n" +
                                 "-> Falling back to native .NET console buffer for safe and responsive input.";
            }
            else
            {
                var x11Provider = new LibX11InputProvider();
                if (x11Provider.IsAvailable)
                {
                    selectedProvider = x11Provider;
                }
                else
                {
                    WarningMessage = x11Provider.InitializationError!;
                }
            }
            
            if (selectedProvider != null)
            {
                Provider = selectedProvider;
            }
            else
            {
                Provider = new DotNetInputProvider();
                ShowWarningAndDelay();
            }
        }
        
        AppDomain.CurrentDomain.ProcessExit += (s, e) => Dispose();
        Console.CancelKeyPress += (s, e) => { Dispose(); };
    }
    
    private static void ShowWarningAndDelay()
    {
        Console.TreatControlCAsInput = true;
            
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n[Engine Input Initialization Warning]");
        Console.WriteLine(WarningMessage);
        Console.ResetColor();
        Console.WriteLine("Continuing startup with standard console buffer fallback in 2 seconds...\n");
            
        Thread.Sleep(2000);
    }

    public static void Update()
    {
        if (IsPollingEnabled) 
        {
            Provider.Update();
        }
    }

    public static bool IsGetKey(ConsoleKey key) => Provider.IsGetKey(key);
        
    public static bool IsGetKey(int virtualKey) => Provider.IsGetKey(virtualKey);

    public static bool IsShift => Provider.IsShift;
    public static bool IsCtrl  => Provider.IsCtrl;
    public static bool IsAlt   => Provider.IsAlt;
    
    public static void Dispose()
    {
        Provider?.Dispose();
    }
}
