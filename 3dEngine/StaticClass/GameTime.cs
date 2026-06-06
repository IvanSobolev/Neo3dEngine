namespace _3dEngine.StaticClass;

public static class GameTime
{
    private static DateTime _startFrameTime = DateTime.UtcNow;
    private static double _fps = 0;
    private static double _deltaTime = 0;

    public static void StartFrame()
    {
        _startFrameTime = DateTime.UtcNow;
    }

    public static void EndFrame()
    {
        _deltaTime = (DateTime.UtcNow - _startFrameTime).TotalSeconds;
        _fps = _deltaTime > 0? 1 / _deltaTime : 0;
    }

    public static double GetFps()
    { return _fps; }
    public static float GetDeltaTime()
    { return (float)_deltaTime; }
}