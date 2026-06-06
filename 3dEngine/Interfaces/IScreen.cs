namespace _3dEngine.Interfaces;

public interface IScreen
{
    public void SetPixelPos(int i, int j);
    public Vector2 GetUv();
    public int GetWidth();
    public int GetHeight();
    public void Paint(int brightness);
    public void Paint(char sim);

    public void Paint(string text, Vector2Int position);
}