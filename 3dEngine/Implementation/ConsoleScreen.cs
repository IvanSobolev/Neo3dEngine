using _3dEngine.Interfaces;

namespace _3dEngine.Implementation;

public class ConsoleScreen() : IScreen
{
    private readonly Vector2Int _resolution = new Vector2Int(Console.WindowWidth, Console.WindowHeight);
    private readonly char[] _screenChar = new char[Console.WindowWidth * Console.WindowHeight];
    private readonly Vector2Int _pixelPos = new Vector2Int(0);
    private readonly float _windowAspect = (float)Console.WindowWidth / (Console.WindowHeight);
    private readonly float _pixelAspect = 11.0f / 24.0f;
    private const string Gradient = " .:!/r(l1Z4H9W8$@";
    
    public void SetPixelPos(int i, int j)
    {
        _pixelPos.X = i;
        _pixelPos.Y = j;
    }

    public Vector2 GetUv()
    {
        Vector2 uv = _pixelPos / _resolution * 2 - 1;
        uv.X *= _windowAspect * _pixelAspect;
        uv.Y = -uv.Y;
        
        return uv;
    }

    public int GetWidth()
    {
        return _resolution.X;
    }

    public int GetHeight()
    {
        return _resolution.Y;
    }
    
    public void Paint(int brightness)
    {
        brightness = int.Clamp(brightness, 0, Gradient.Length - 1);
        char sim = Gradient[brightness];
        Paint(sim);
    }

    public void Paint(char sim)
    {
        if (_screenChar[_pixelPos.Y * _resolution.X + _pixelPos.X] != sim && _pixelPos.X < Console.WindowWidth && _pixelPos.Y < Console.WindowHeight)
        {
            Console.SetCursorPosition(_pixelPos.X,_pixelPos.Y);
            Console.Write(sim);
            _screenChar[_pixelPos.Y * _resolution.X + _pixelPos.X] = sim;
        }
    }

    public void Paint(string text, Vector2Int position)
    {
        Console.SetCursorPosition(position.X,position.Y);
        Console.Write(text);
    }
}