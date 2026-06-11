using _3dEngine.AbstractClass;
using _3dEngine.Inputs;
using _3dEngine.Interfaces;
using _3dEngine.StaticClass;

namespace _3dEngine;
public class Frame(Scene activeScene, Screen screen)
{
    private readonly Screen _screen = screen;
    private readonly Scene _activeScene = activeScene;
    
    private static string _currentTitle = "Neo 3D Console";
    
    public static string Title
    {
        get => _currentTitle;
        set
        {
            _currentTitle = value;
            try
            {
                Console.Title = value; 
            }
            catch { 
                //todo log
            }
        }
    }
    
    private bool _isRunning = true;

    public void MainLoop()
    {
        Console.Title = _currentTitle;
        _activeScene.Start();

        while (_isRunning)
        {
            GameTime.StartFrame();
            
            Input.Update(); 
            
            if (Input.IsGetKey(ConsoleKey.Escape))
            {
                _isRunning = false;
                continue;
            }

            _activeScene.Update();

            _screen.RenderFrame(_activeScene);

            _screen.PrintText("Fps: " + Double.Round(GameTime.GetFps(), 1) + "       ", Vector2Int.Zero);

            GameTime.EndFrame();
        }
        
        Input.Dispose(); 
        
        Console.ResetColor();
        Console.CursorVisible = true;
        Console.Clear();
        Console.WriteLine("Thanks for playing!");
    }
}