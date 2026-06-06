namespace _3dEngine;

public struct Vector2(float x = 0, float y = 0)
{
    public float X = x;
    public float Y = y;

    public static readonly Vector2 Zero = new Vector2();
    
    public static Vector2 operator +(Vector2 a, Vector2 b) 
    {  return new Vector2(a.X + b.X, a.Y + b.Y); }
    public static Vector2 operator +(Vector2 a, int b) 
    {  return new Vector2(a.X + b, a.Y + b); }
    
    public static Vector2 operator -(Vector2 a, Vector2 b) 
    {  return new Vector2(a.X - b.X, a.Y - b.Y); }
    public static Vector2 operator -(Vector2 a, int b) 
    {  return new Vector2(a.X - b, a.Y - b); }

    public static float operator *(Vector2 a, Vector2 b)
    { return a.X * b.X + a.Y * b.Y; }
    public static Vector2 operator *(Vector2 a, int b)
    {  return new Vector2(a.X * b, a.Y * b); }
    
    public static Vector2 operator /(Vector2 a, Vector2 b)
    {  return new Vector2(a.X / b.X, a.Y / b.Y); }
    public static Vector2 operator /(Vector2 a, int b)
    {  return new Vector2(a.X / b, a.Y / b); }
    public static Vector2 operator /(Vector2 a, Vector2Int b)
    {  return new Vector2(a.X / b.X, a.Y / b.Y); }

    
    public float Length() => (float)Math.Sqrt(X * X + Y * Y);
    public Vector2 Rotate(float a)
    {
        double xr = X * Math.Cos(a) - Y * Math.Sin(a);
        double yr = X * Math.Sin(a) + Y * Math.Cos(a);

        return new Vector2((float)xr, (float)yr);
    }
}