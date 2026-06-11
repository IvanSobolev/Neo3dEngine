using System.Collections.Concurrent;
using _3dEngine.Inputs.Interfaces;
using _3dEngine.Interfaces;

namespace _3dEngine.Inputs.Implementations;

internal class DotNetInputProvider : IInputProvider
{
    private readonly ConcurrentDictionary<ConsoleKey, DateTime> _pressedKeys = new();
    private ConsoleModifiers _modifiers;

    private readonly TimeSpan _keyReleaseTimeout = TimeSpan.FromMilliseconds(90);


    public void Update()
    {
        while (Console.KeyAvailable)
        {
            var keyInfo = Console.ReadKey(intercept: true);
                
            _pressedKeys[keyInfo.Key] = DateTime.UtcNow;
            _modifiers = keyInfo.Modifiers;
        }

        var now = DateTime.UtcNow;
        foreach (var kvp in _pressedKeys)
        {
            if ((now - kvp.Value) > _keyReleaseTimeout)
            {
                _pressedKeys.TryRemove(kvp.Key, out _);
            }
        }
        
        if (_pressedKeys.IsEmpty)
        {
            _modifiers = 0;
        }
    }

    public bool IsGetKey(ConsoleKey key)
    {
        return _pressedKeys.ContainsKey(key);
    }

    public bool IsGetKey(int virtualKey)
    {
        return IsGetKey((ConsoleKey)virtualKey);
    }

    public bool IsShift => (_modifiers & ConsoleModifiers.Shift) != 0;
    public bool IsCtrl  => (_modifiers & ConsoleModifiers.Control) != 0;
    public bool IsAlt   => (_modifiers & ConsoleModifiers.Alt) != 0;
    public void Dispose()
    {}
}