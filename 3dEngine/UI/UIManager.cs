namespace _3dEngine.UI;

public class UIManager
{
    private readonly List<UIText> _elements = new();

    public void AddText(string text, Vector2Int pos, ConsoleColor color = ConsoleColor.White)
    {
        _elements.Add(new UIText { Text = text, Position = pos, Color = color });
    }

    public List<UIText> GetElements() => _elements;

    public void Clear() => _elements.Clear();
}