using _3dEngine.AbstractClass;
using _3dEngine.Implementation;
using _3dEngine.Interfaces;
using _3dEngine.Interfaces.modifier;

namespace _3dEngine.Shape;

public class Object3d : GameObject, IDisplays
{
    private readonly List<Triangle> _faces;

    private readonly Vector3[] _localVertices;
    private readonly Vector3[] _worldVertices;

    private float _boundingRadius = 0;

    public Object3d(Vector3[] vertex, Vector3[] normals, FacingInfo[] facingInfos) : base(Vector3.Zero, Vector3.Zero)
    {
        _localVertices = vertex;
        _worldVertices = new Vector3[vertex.Length];

        foreach (var v in _localVertices)
        {
            float dist = v.Length();
            if (dist > _boundingRadius) _boundingRadius = dist;
        }

        _faces = new List<Triangle>();
        foreach (var facingInfo in facingInfos)
        {
            _faces.Add(new Triangle(
                new int[] { facingInfo.Vertex1 - 1, facingInfo.Vertex2 - 1, facingInfo.Vertex3 - 1 },
                normals[facingInfo.NormalIndex - 1]
            ));
        }

        UpdateGeometry();
    }

    public void UpdateGeometry()
    {
        Vector3 totalRot = LocalRotate + GlobalRotate;

        Parallel.For(0, _localVertices.Length, i =>
        {
            _worldVertices[i] = (_localVertices[i].Rotate(totalRot)) + Position;
        });
    }

    public RenderData GetRenderData(Ray ray)
    {
        Vector3 L = Position - ray.RayStart;
        float tca = L * ray.RayDirection;
        float d2 = L * L - tca * tca;
        float r2 = _boundingRadius * _boundingRadius;

        if (d2 > r2) return RenderData.NoRender;


        RenderData closestData = RenderData.NoRender;
        Vector3 totalRot = LocalRotate + GlobalRotate;

        foreach (var face in _faces)
        {
            var currentData = face.GetRenderData(ray, _worldVertices, totalRot);

            if (currentData.Intersection > -1)
            {
                if (closestData.Intersection == -1 || currentData.Intersection < closestData.Intersection)
                {
                    closestData = currentData;
                }
            }
        }
        if (closestData.Intersection > -1)
        {
            closestData.Color = this.Color;
        }
        return closestData;
    }
}