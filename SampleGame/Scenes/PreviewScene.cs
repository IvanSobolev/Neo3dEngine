using _3dEngine;
using _3dEngine.AbstractClass;
using _3dEngine.Implementation;
using _3dEngine.Inputs;
using _3dEngine.Interfaces;
using _3dEngine.Shape;
using _3dEngine.StaticClass;

namespace SampleGame.Scenes;

public class PreviewScene (IDisplaysManagerAsync iDisplaysManager) : Scene(iDisplaysManager)
{
    private readonly Camera _camera = new Camera(new Vector3(0,0,0), Vector3.Zero);
    
    private readonly Light _light = new Light(new Vector3(-1f,0f,0f),10);
    
    private Vector3 cameraPos = Vector3.Zero;

    private Object3d _importedModel;

    private readonly Sphere _sphere = new Sphere(new Vector3(0,0,0), Vector3.Zero);

    private readonly Object3d _cube = new Object3d(
        new Vector3[]
        {
            new Vector3(-1f, -1f, 1f),   
            new Vector3(-1f, 1f, 1f),
            new Vector3(-1f, -1f, -1f),
            new Vector3(-1f, 1f, -1f),
            new Vector3(1f, -1f, 1f),
            new Vector3(1f, 1f, 1f),
            new Vector3(1f, -1f, -1f),
            new Vector3(1f, 1f, -1f)
        },
        new Vector3[]
        {
            new Vector3(-1f, 0, 0),
            new Vector3(0, 0, -1f),
            new Vector3(1f, 0, 0),
            new Vector3(0, 0, 1f),
            new Vector3(0, -1f, 0),
            new Vector3(0, 1f, 0)
        },
        new FacingInfo[]
        {
            new FacingInfo(new int[] {2,3,1}, 1),
            new FacingInfo(new int[] {4,7,3}, 2),
            new FacingInfo(new int[] {8,5,7}, 3),
            new FacingInfo(new int[] {6,1,5}, 4),
            new FacingInfo(new int[] {7,1,3}, 5),
            new FacingInfo(new int[] {4,6,8}, 6),
            new FacingInfo(new int[] {2,4,3}, 1),
            new FacingInfo(new int[] {4,8,7}, 2),
            new FacingInfo(new int[] {8,6,5}, 3),
            new FacingInfo(new int[] {6,2,1}, 4),
            new FacingInfo(new int[] {7,5,1}, 5),
            new FacingInfo(new int[] {4,2,6}, 6)
        });

    private readonly Object3d _plane = new Object3d(
        new Vector3[]
        {
            new Vector3(-5f, 0f, 5f),   
            new Vector3(5f, 0f, 5f),
            new Vector3(-5f, 0f, -5),
            new Vector3(5f, 0f, -5f)
        },
        new Vector3[]
        {
            new Vector3(0f, 1f, 0f)
        },
        new FacingInfo[]
        {
            new FacingInfo(new int[] {2,3,1}, 1),
            new FacingInfo(new int[] {2,4,3}, 1),
        });
    

    public override void Start()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "monkey.obj");
        try
        {
            _importedModel = ObjLoader.Load(path);

            _importedModel.Position = new Vector3(2, 0, 0);

            AddDisplaysObject(_importedModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка загрузки OBJ: " + ex.Message);
        }

        _cube.Position = new Vector3(0,0,2);
        
        _sphere.Position = new Vector3(0, 0, -2);
        _plane.Position = new Vector3(0, -1f, 0);
        
        _camera.Position = new Vector3(0, 0, 0);
        _camera.LocalRotate.Z = -0.2f;

        _cube.UpdateGeometry();
        _plane.UpdateGeometry();
        if (_importedModel != null) _importedModel.UpdateGeometry();

        _light.Position = new Vector3(0, 0, 0);
        _sphere.Color = ConsoleColor.Red;
        _plane.Color = ConsoleColor.DarkGreen;
        _cube.Color = ConsoleColor.Cyan;
        _importedModel.Color = ConsoleColor.Yellow;
        AddDisplaysObject(_cube);
        AddDisplaysObject(_sphere);
        AddDisplaysObject(_plane);
        AddLight(_light);
        SetMainCamera(_camera);
    }

    public override void Update()
    {
        float rotateSpeed = 1.5f;
        float dt = GameTime.GetDeltaTime();

        _importedModel.LocalRotate.Y += 1f * dt;
        if (_importedModel != null) _importedModel.UpdateGeometry();

        if (Input.IsGetKey(ConsoleKey.LeftArrow))
            _camera.LocalRotate.Y += rotateSpeed * dt;
        if (Input.IsGetKey(ConsoleKey.RightArrow))
            _camera.LocalRotate.Y -= rotateSpeed * dt;

        if (Input.IsGetKey(ConsoleKey.UpArrow))
            _camera.LocalRotate.Z += rotateSpeed * dt;
        if (Input.IsGetKey(ConsoleKey.DownArrow))
            _camera.LocalRotate.Z -= rotateSpeed * dt;

        _camera.LocalRotate.Z = Math.Clamp(_camera.LocalRotate.Z, -1.5f, 1.5f);

        _camera.LocalRotate.X = 0;

        float moveSpeed = 4.0f;

        Vector3 forward = new Vector3(1, 0, 0).Rotate(_camera.LocalRotate);
        Vector3 right = new Vector3(0, 0, 1).Rotate(_camera.LocalRotate);
        Vector3 up = new Vector3(0, 1, 0).Rotate(_camera.LocalRotate);
        
        if (Input.IsCtrl)
        {
            
            if(Input.IsGetKey(ConsoleKey.W))
                _light.Position += forward * moveSpeed * dt;
            
            if (Input.IsGetKey(ConsoleKey.S))
                _light.Position -= forward *moveSpeed * dt;
            
            if (Input.IsGetKey(ConsoleKey.D))
                _light.Position += right * moveSpeed * dt;
            
            if (Input.IsGetKey(ConsoleKey.A))
                _light.Position -= right * moveSpeed * dt;
            
            if (Input.IsGetKey(ConsoleKey.Spacebar))
                _light.Position.Y += moveSpeed * dt;
            
            if (Input.IsShift)
                _light.Position.Y -= moveSpeed * dt;
            _camera.Position = _light.Position;
        }
        else
        {
            _camera.Position = cameraPos;
            
            if (Input.IsGetKey(ConsoleKey.W))
                _camera.Position += forward * moveSpeed * dt;

            if (Input.IsGetKey(ConsoleKey.S))
                _camera.Position -= forward * moveSpeed * dt;

            if (Input.IsGetKey(ConsoleKey.D))
                _camera.Position += right * moveSpeed * dt;

            if (Input.IsGetKey(ConsoleKey.A))
                _camera.Position -= right * moveSpeed * dt;

            if (Input.IsGetKey(ConsoleKey.Spacebar))
                _camera.Position.Y += moveSpeed * dt;

            if (Input.IsShift)
                _camera.Position.Y -= moveSpeed * dt;
            cameraPos = _camera.Position;
        }
        
        

       

        float lightPowerSpeed = 10f;
        if (Input.IsGetKey(ConsoleKey.OemPlus))
            _light.LightPower += lightPowerSpeed * dt;
        if (Input.IsGetKey(ConsoleKey.OemMinus))
            _light.LightPower -= lightPowerSpeed * dt;

        //_camera.GlobalRotate.Y += 1f * GameTime.GetDeltaTime();
        //_light.Position = _camera.GetRo();
    }
}