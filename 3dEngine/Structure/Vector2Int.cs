namespace _3dEngine;

public class Vector2Int(int x = 0, int y = 0)
{
    public int X = x;
    public int Y = y;
    
    public static readonly Vector2Int Zero = new Vector2Int();
    public Vector2Int(int a) : this(a, a) { }
    public Vector2Int() : this(0, 0) { }
    
    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
    {  return new Vector2Int(a.X + b.X, a.Y + b.Y); }
    public static Vector2Int operator +(Vector2Int a, int b)
    {  return new Vector2Int(a.X + b, a.Y + b); }
    
    public static Vector2Int operator -(Vector2Int a, Vector2Int b)
    {  return new Vector2Int(a.X - b.X, a.Y - b.Y); }
    public static Vector2Int operator -(Vector2Int a, int b)
    {  return new Vector2Int(a.X - b, a.Y - b); }
    
    public static float operator *(Vector2Int a, Vector2Int b)
    { return a.X * b.X + a.Y * b.Y; }
    public static Vector2Int operator *(Vector2Int a, int b)
    {  return new Vector2Int(a.X * b, a.Y * b); }
    
    public static Vector2 operator /(Vector2Int a, Vector2Int b)
    {  return new Vector2((float)a.X / b.X, (float)a.Y / b.Y); }
    public static Vector2 operator /(Vector2Int a, Vector2 b)
    {  return new Vector2(a.X / b.X, a.Y / b.Y); }
    public static Vector2 operator /(Vector2Int a, int b)
    {  return new Vector2((float)a.X / b, (float)a.Y / b); }
}