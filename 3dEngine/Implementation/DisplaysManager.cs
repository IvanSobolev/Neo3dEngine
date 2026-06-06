using _3dEngine.Interfaces;
using _3dEngine.Interfaces.modifier;

namespace _3dEngine.Implementation;

public class DisplaysManager : IDisplaysManager
{
    List<RenderData> _renderDatas  = new List<RenderData>{ };
    
    public void FindAllRenderData(Ray ray, List<IDisplays> displays)
    {
        _renderDatas.Clear();
        for(int i =0; i < displays.Count; i ++)
        {
            var renderData = displays[i].GetRenderData(ray);
            if(renderData.Intersection > -1)
            {
                _renderDatas.Add(renderData);
            }
        }
    }

    public RenderData GetNearbyRenderData()
    {
        if (_renderDatas.Count <= 0)
        { return RenderData.NoRender; }
        
        RenderData minIntersection = _renderDatas[0];
        for (int i = 0; i < _renderDatas.Count; i ++)
        {
            if (minIntersection.Intersection > _renderDatas[i].Intersection)
            {
                minIntersection = _renderDatas[i];
            }
        }
        
        return minIntersection;
    }
}