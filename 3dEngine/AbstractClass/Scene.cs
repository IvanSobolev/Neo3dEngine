using _3dEngine.Implementation;
using _3dEngine.Interfaces;
using _3dEngine.Interfaces.modifier;
using _3dEngine.UI;

namespace _3dEngine.AbstractClass;

public abstract class Scene(IDisplaysManagerAsync displaysManager)
{
    private readonly IDisplaysManagerAsync _displaysManager = displaysManager;
    private readonly List<IDisplays> _allDisplays = new List<IDisplays>();
    private readonly List<Light> _allLight = new List<Light>();
    private Camera _renderCamera = new Camera(Vector3.Zero, Vector3.Zero);
    
    public readonly UIManager UI = new UIManager();

    protected void SetMainCamera(Camera camera)
    { _renderCamera = camera; }
    protected void AddDisplaysObject(IDisplays @object)
    { _allDisplays.Add(@object); }

    protected void AddLight(Light light)
    { _allLight.Add(light); }
    
    public abstract void Start();
    public abstract void Update();

    public virtual (int Brightness, ConsoleColor Color) GetPixelData(Vector2 uv)
    {
        Ray ray = _renderCamera.GetRayForUv(uv);

        var renderData = _displaysManager.FindClosestIntersection(ray, _allDisplays);

        if (renderData.Intersection == -1)
        {
            return (0, ConsoleColor.Black);
        }

        int maxBrightness = 0;
        foreach (var light in _allLight)
        {
            var brightness = light.PointBright(renderData, _allDisplays, _displaysManager);
            if (brightness > maxBrightness)
            {
                maxBrightness = brightness;
            }
        }

        return (maxBrightness, renderData.Color);
    }
}