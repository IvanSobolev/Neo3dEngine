using _3dEngine.Implementation;

namespace _3dEngine.Interfaces.modifier;

public interface IDisplays
{
    public RenderData GetRenderData(Ray ray);
}