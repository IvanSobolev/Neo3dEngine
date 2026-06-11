using System.Runtime.CompilerServices;

namespace _3dEngine;

public struct Vector3(float x, float y, float z)
{
    public float X = x;
    public float Y = y;
    public float Z = z;

    public Vector3(float x, Vector2 a) : this(x, a.X, a.Y) { }
    public Vector3(Vector2 a, float z) : this(a.X, a.Y, z) { }
    public Vector3(float a) : this(a, a, a) { }
    public Vector3() : this(0, 0, 0) { }

    public static readonly Vector3 Zero = new Vector3();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator +(Vector3 a, Vector3 b) 
    {  return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator +(Vector3 a, Vector2 b) 
    {  return new Vector3(a.X + b.X, a.Y + b.Y, a.Z); }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator +(Vector3 a, int b) 
    {  return new Vector3(a.X + b, a.Y + b, a.Z + b); }

    public static Vector3 operator -(Vector3 a, Vector3 b)
    {  return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }
    public static Vector3 operator -(Vector3 a, int b)
    { return new Vector3(a.X - b, a.Y - b, a.Z - b);}
    public static Vector3 operator -(Vector3 a)
    {  return new Vector3(-a.X, -a.Y, -a.Z); }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float operator *(Vector3 a, Vector3 b)
    { return a.X * b.X + a.Y * b.Y + a.Z * b.Z; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator *(Vector3 a, float b)
    {  return new Vector3(a.X * b, a.Y * b, a.Z * b); }
    
    public static Vector3 operator /(Vector3 a, Vector3 b) 
    {  return new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z); }
    public static Vector3 operator /(Vector3 a, float b)
    {  return new Vector3(a.X / b, a.Y / b, a.Z / b); }
    public static Vector3 operator /(float b, Vector3 a)
    {  return new Vector3(b / a.X, b / a.Y, b / a.Z); }

    public static bool operator ==(Vector3 a, Vector3 b)
    {
        return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
    }

    public static bool operator !=(Vector3 a, Vector3 b)
    {
        return !(a == b);
    }
    
    public static implicit operator Vector3(int a)
    {
        return new Vector3(a);
    }


    public  Vector3 Norm() => this / this.Length();
    public float Length() => (float)Math.Sqrt(X * X + Y * Y + Z * Z);
    public Vector3 Rotate(Vector3 rotation)
    {
        var rotatedVector = RotateX(this, rotation.X);
        rotatedVector = RotateY(rotatedVector, rotation.Y);
        rotatedVector = RotateZ(rotatedVector, rotation.Z);

        return rotatedVector;
    }

    private static Vector3 RotateX(Vector3 rot, float a)
    {
        Vector3 rotatedVector = Vector3.Zero;

        rotatedVector.X = rot.X;
        rotatedVector.Y = (float)(rot.Y * Math.Cos(a) - rot.Z * Math.Sin(a));
        rotatedVector.Z = (float)(rot.Y * Math.Sin(a) + rot.Z * Math.Cos(a));
        
        return rotatedVector;
    }
    
    private static Vector3 RotateY(Vector3 rot, float a)
    {
        Vector3 rotatedVector = Vector3.Zero;

        rotatedVector.X = (float)(rot.X * Math.Cos(a) + rot.Z * Math.Sin(a));
        rotatedVector.Y = rot.Y;
        rotatedVector.Z = (float)(-rot.X * Math.Sin(a) + rot.Z * Math.Cos(a));
        
        return rotatedVector;
    }
    
    private static Vector3 RotateZ(Vector3 rot, float a)
    {
        Vector3 rotatedVector = Vector3.Zero;

        rotatedVector.X = (float)(rot.X * Math.Cos(a) - rot.Y * Math.Sin(a));
        rotatedVector.Y = (float)(rot.X * Math.Sin(a) + rot.Y * Math.Cos(a));
        rotatedVector.Z = rot.Z;
        
        return rotatedVector;
    }
    
    public static Vector3 Cross(Vector3 a, Vector3 b)
    {
        return new Vector3(
            a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X
        );
    }
    public Vector3 RotateInverse(Vector3 rotation)
    {
        var v = RotateZ(this, -rotation.Z);
        v = RotateY(v, -rotation.Y);
        v = RotateX(v, -rotation.X);
        return v;
    }
}