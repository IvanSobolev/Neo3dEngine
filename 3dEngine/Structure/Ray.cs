namespace _3dEngine;

public struct Ray(Vector3 rayStart, Vector3 rayDirection)
{
    public Vector3 RayStart = rayStart;
    public Vector3 RayDirection = rayDirection;
    
    public Vector3 GetIntersectionPoint(float intersection)
    {
        return RayStart + RayDirection * intersection;
    }
}