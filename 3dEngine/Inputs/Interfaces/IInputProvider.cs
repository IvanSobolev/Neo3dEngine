using _3dEngine.Interfaces;

namespace _3dEngine.Inputs.Interfaces;

internal interface IInputProvider : IDisposable
{
    void Update();
    bool IsGetKey(ConsoleKey key);
    bool IsGetKey(int virtualKey);
    bool IsShift { get; }
    bool IsCtrl  { get; }
    bool IsAlt { get; }
}