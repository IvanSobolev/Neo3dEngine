namespace _3dEngine;

public struct RenderData(float intersection = -1, Vector3 normal = default, Vector3 intersectionPoint = default, ConsoleColor color = ConsoleColor.White)
{
    public float Intersection = intersection;
    public Vector3 Normal = normal;
    public Vector3 IntersectionPoint = intersectionPoint;
    public ConsoleColor Color = color;

    public static RenderData NoRender = new RenderData(-1, Vector3.Zero, Vector3.Zero, ConsoleColor.Black);
}