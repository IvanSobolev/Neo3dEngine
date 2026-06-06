using _3dEngine.AbstractClass;
using _3dEngine.Implementation;
using _3dEngine.Interfaces.modifier;

namespace _3dEngine.Shape;

public class Sphere(Vector3 position, Vector3 localRotate, float r = 1) : GameObject(position, localRotate), IDisplays
{
    public float R = r;

    public RenderData GetRenderData(Ray ray)
    {
        
        Vector3 l = (ray.RayStart - Position).Rotate(localRotate);
        float a = ray.RayDirection * ray.RayDirection;
        float b = 2 * (l * ray.RayDirection);
        float c = l * l - R * R;

        float d = b * b - 4 * a * c;
        if (d < 0)
        { return RenderData.NoRender; }

        d = (float)Math.Sqrt(d);
        
        float intersection = (-b - d) / (2 * a);
        Vector3 intersectionPoint = ray.GetIntersectionPoint(intersection);
        Vector3 normal = intersectionPoint - Position;

        return new RenderData(intersection, normal, intersectionPoint, this.Color);
    }
}