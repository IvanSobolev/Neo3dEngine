using _3dEngine.AbstractClass;
using _3dEngine.Interfaces;
using _3dEngine.StaticClass;

namespace _3dEngine;
public class Frame(Scene activeScene, Screen screen)
{
    private readonly Screen _screen = screen;
    private readonly Scene _activeScene = activeScene;

    public void MainLoop()
    {
        _activeScene.Start();

        while (true)
        {
            GameTime.StartFrame();

            _activeScene.Update();

            _screen.RenderFrame(_activeScene);

            _screen.PrintText("Fps: " + Double.Round(GameTime.GetFps(), 1) + "       ", Vector2Int.Zero);

            GameTime.EndFrame();
        }
    }
}