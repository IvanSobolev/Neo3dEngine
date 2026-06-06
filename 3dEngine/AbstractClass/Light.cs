using _3dEngine.Implementation;
using _3dEngine.Interfaces;
using _3dEngine.Interfaces.modifier;

namespace _3dEngine.AbstractClass;

public class Light (Vector3 position, float lightPower) : GameObject(position, Vector3.Zero)
{
    public float LightPower = lightPower;
    //private readonly IDisplaysManager _displaysManager = new DisplaysManager();

    private float CalculateBrightness(RenderData renderData)
    {
        Vector3 offset = Position - renderData.IntersectionPoint;
        float distance = offset.Length();

        Vector3 lightDir = offset / distance;

        float angleFactor = Math.Max(0, renderData.Normal * lightDir);

        float attenuation = LightPower / (distance * distance + 1f);

        return angleFactor * attenuation;
    }

    public virtual int PointBright(RenderData renderData)
    {
        return (int)CalculateBrightness(renderData);
    }

    public virtual int PointBright(RenderData renderData, List<IDisplays> sceneObjects, IDisplaysManagerAsync displaysManager)
    {
        Vector3 lightDir = (Position - renderData.IntersectionPoint).Norm();

        float dot = renderData.Normal * lightDir;
        if (dot <= 0) return 0;

        float distanceToLight = (Position - renderData.IntersectionPoint).Length();
        const float epsilon = 0.01f;

        Ray shadowRay = new Ray(renderData.IntersectionPoint + renderData.Normal * epsilon, lightDir);
        RenderData shadowHit = displaysManager.FindClosestIntersection(shadowRay, sceneObjects);

        if (shadowHit.Intersection > -1 && shadowHit.Intersection < distanceToLight)
        {
            return 0;
        }

        return (int)CalculateBrightness(renderData);
    }
}