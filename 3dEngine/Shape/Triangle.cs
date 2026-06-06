using _3dEngine.AbstractClass;
using _3dEngine.Implementation;
using _3dEngine.Interfaces.modifier;

namespace _3dEngine.Shape;

public class Triangle(int[] indices, Vector3 normal)
{
    private readonly int _i0 = indices[0];
    private readonly int _i1 = indices[1];
    private readonly int _i2 = indices[2];
    private readonly Vector3 _normal = normal;

    public RenderData GetRenderData(Ray ray, Vector3[] worldVertices, Vector3 rotationForNormal)
    {
        Vector3 v0 = worldVertices[_i0];
        Vector3 v1 = worldVertices[_i1];
        Vector3 v2 = worldVertices[_i2];

        Vector3 E1 = v1 - v0;
        Vector3 E2 = v2 - v0;

        Vector3 geometricNormal = Vector3.Cross(E1, E2);

        if (geometricNormal * ray.RayDirection > 0)
        {
            return RenderData.NoRender;
        }

        Vector3 P = Vector3.Cross(ray.RayDirection, E2);
        float det = E1 * P;

        if (Math.Abs(det) < 1e-6) return RenderData.NoRender;

        float invDet = 1.0f / det;
        Vector3 T = ray.RayStart - v0;
        float u = T * P * invDet;

        if (u < 0 || u > 1) return RenderData.NoRender;

        Vector3 Q = Vector3.Cross(T, E1);
        float v = ray.RayDirection * Q * invDet;

        if (v < 0 || u + v > 1) return RenderData.NoRender;

        float intersection = E2 * Q * invDet;

        if (intersection < 0) return RenderData.NoRender;

        var rotatedNormal = _normal.Rotate(rotationForNormal).Norm();

        var intersectionPoint = ray.GetIntersectionPoint(intersection);

        return new RenderData(intersection, rotatedNormal, intersectionPoint);
    }
}