using _3dEngine.AbstractClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3dEngine.Implementation;
public class ConsoleScreenAsync : Screen
{
    private const string Gradient = " .:!/r(l1Z4H9W8$@";
    private readonly char[] _charBuffer;

    private readonly float _aspectRatio;

    public ConsoleScreenAsync() : base(Console.WindowWidth, Console.WindowHeight)
    {
        _charBuffer = new char[Width * Height];
        Console.CursorVisible = false;

        float windowAspect = (float)Width / Height;
        float pixelAspect = 11.0f / 24.0f;
        _aspectRatio = windowAspect * pixelAspect;
    }

    protected override Vector2 CalculateUV(int i, int j)
    {
        Vector2 uv = new Vector2((float)i / (Width - 1), (float)j / (Height - 1)) * 2 - 1;
        uv.X *= _aspectRatio;
        uv.Y = -uv.Y;
        return uv;
    }

    protected override void Present()
    {
        Console.SetCursorPosition(0, 0);

        int bufferLength = _charBuffer.Length;
        int currentIndex = 0;

        while (currentIndex < bufferLength)
        {
            ConsoleColor currentColor = ColorBuffer[currentIndex];
            int runLength = 0;

            while (currentIndex + runLength < bufferLength &&
                   ColorBuffer[currentIndex + runLength] == currentColor)
            {
                runLength++;
            }

            if (Console.ForegroundColor != currentColor)
            {
                Console.ForegroundColor = currentColor;
            }

            Console.Out.Write(_charBuffer, currentIndex, runLength);
            currentIndex += runLength;
        }
    }

    public override void PrintText(string text, Vector2Int position)
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(position.X, position.Y);
            Console.Write(text);
        }
        catch { }
    }
    
    public override void RenderFrame(Scene scene)
    {
        Parallel.For(0, Height, j =>
        {
            for (int i = 0; i < Width; i++)
            {
                Vector2 uv = CalculateUV(i, j);
                var pixelData = scene.GetPixelData(uv);

                int index = j * Width + i;
                BrightnessBuffer[index] = pixelData.Brightness;
                ColorBuffer[index] = pixelData.Color;
            }
        });

        Parallel.For(0, BrightnessBuffer.Length, i =>
        {
            int brightness = BrightnessBuffer[i];
            brightness = int.Clamp(brightness, 0, Gradient.Length - 1);
            _charBuffer[i] = Gradient[brightness];
        });

        var uiElements = scene.UI.GetElements();
        foreach (var element in uiElements)
        {
            DrawTextToBuffer(element.Text, element.Position, element.Color);
        }

        Present();
    }
    
    private void DrawTextToBuffer(string text, Vector2Int pos, ConsoleColor color)
    {
        for (int i = 0; i < text.Length; i++)
        {
            int x = pos.X + i;
            int y = pos.Y;

            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                int index = y * Width + x;
                _charBuffer[index] = text[i];
                ColorBuffer[index] = color;
            }
        }
    }
}
