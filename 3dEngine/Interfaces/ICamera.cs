namespace _3dEngine.Interfaces;

public interface ICamera
{
    Ray GetRayForUv(Vector2 uv);
}