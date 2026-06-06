using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3dEngine.AbstractClass;
public abstract class Screen
{
    public int Width { get; }
    public int Height { get; }

    protected readonly int[] BrightnessBuffer;
    protected readonly ConsoleColor[] ColorBuffer;
    private readonly float _aspectRatio;

    protected Screen(int width, int height)
    {
        Width = width;
        Height = height;
        BrightnessBuffer = new int[width * height];
        ColorBuffer = new ConsoleColor[width * height];
    }

    public virtual void RenderFrame(Scene scene)
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

        Present();
    }

    protected abstract Vector2 CalculateUV(int i, int j);
    protected abstract void Present();
    public abstract void PrintText(string text, Vector2Int position);
}