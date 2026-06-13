<p align="center"><img src="https://github.com/user-attachments/assets/93d11446-f67c-401e-8b35-693262f5d008" width="200"></p>
<h1 align="center"><b>Neo 3D</b></h1>
<h4 align="center">A minimalist console 3D engine with game development elements</h4>
<p align="center">
    <a href="https://github.com/IvanSobolev/Neo3dEngine/releases/tag/v0.1.1">
        <img alt="Latest Release" src="https://img.shields.io/static/v1?label=tag&message=v0.1.1&color=64B5F6&style=flat">
    </a>
   <a href="https://github.com/IvanSobolev/Neo3dEngine/">
        <img alt="Stars" src="https://img.shields.io/github/stars/IvanSobolev/Neo3dEngine?color=4B95DE&style=flat">
    </a>
   <a href="https://github.com/IvanSobolev/Neo3dEngine/">
        <img alt="Forks" src="https://img.shields.io/github/forks/IvanSobolev/Neo3dEngine?color=4B95DE&style=flat">
    </a>
    <a href="https://www.gnu.org/licenses/gpl-3.0">
        <img src="https://img.shields.io/badge/license-GPL%20v3-2B6DBE.svg?style=flat">
    </a>
</p>
<h4 align="center"><a href="/CHANGELOG.md">Changelog</a> | <a href="https://github.com/IvanSobolev/Neo3dEngine/wiki">WIKI</a></h4>

## About the Project

Neo 3D is a minimalist 3D engine written in C# that runs entirely inside the system console. The project is built without using third-party graphics libraries. All calculations—ranging from custom vector mathematics to raycasting/raytracing and network code—are executed on the CPU. In the future, we plan to port the most demanding computations from the CPU to the GPU using Vulkan.

The project was created for demonstration and educational purposes for a [YouTube video](https://youtu.be/vhYE882B9dE).

**The default branch contains the stable version of the project. There is also a long-lived `development` branch and short-lived feature branches.**

## Scene Rendering Example (v0.1.1)

<p align="center">
    <img src="https://github.com/user-attachments/assets/bebdf374-fc09-4a35-a1fc-cba1a21e205b">
</p>


## Features

- Renders geometric spheres and `.obj` files using Raytracing.
- Intersection optimization via bounding sphere checks before performing detailed polygon calculations.
- Light attenuation calculation based on distance.
- Lambertian diffuse lighting based on surface normals.
- Shadow rendering by casting shadow rays from the intersection point to the light source.
- Multithreaded rendering using `Parallel.For` to distribute pixel calculations across all CPU cores (with future plans to move these computations to the GPU using Vulkan).
- Brightness mapping to a character gradient `" .:!/r(l1Z4H9W8$@"` for console output.
- Frame buffering and batch rendering of identical colors to minimize slow console system API calls.
- Custom `Vector3`, `Vector2`, `Ray` structures and rotation matrices.
- Implementation of the Möller–Trumbore algorithm for ray-triangle intersection.
- An input system for efficient keyboard polling using system APIs (adapting to the host OS via `IInputProvider`). Supported providers include: `User32.dll` on Windows, `X11` (with Wayland disabled) on Linux, and `DotNet Input` (an inefficient fallback with limitations on simultaneous key presses and difficulties tracking modifier keys like `Shift`, `Ctrl`, `Alt`; used when system access to `User32` or `X11` is unavailable).
- A custom client-server module based on a TCP architecture for game data transmission. We plan to add a stable UDP interface in the future to increase performance in online scenes.
- Custom binary packet serialization, automatic routing via stable type-hash IDs, and an event subscription system.
- Examples of a working multiplayer lobby featuring player position synchronization and text chat.
- An example scene demonstrating how to import and render a `.obj` model.
- Automatic console size detection and scaling to fit the maximum terminal size (determined at startup).

## Run Requirements

When running on Linux, Wayland must be disabled for the terminal window (since Wayland does not grant the terminal direct access to X11). There are many ways to disable it. If Wayland is active, a warning will be displayed when starting a scene.

The easiest way to launch a terminal without Wayland is to use an independent terminal emulator (such as `xterm`) forced to run in X11 compatibility mode (via XWayland):
```bash
apt install xterm
WAYLAND_DISPLAY= xterm
```

In the resulting `xterm` window, navigate to your project directory and run the build command.


## Build Instructions

Neo 3D does not rely on third-party libraries. To run it, you only need the **.NET 8.0 SDK** or newer.
1. Download or clone the repository from GitHub.
2. Navigate to the `SampleGame` folder inside the project using your terminal and compile: `dotnet run --configuration Release`
3. If configured correctly, you will be able to control the active scene. Use `W`, `A`, `S`, `D` to move; `Space`, `Shift` (or `Shift + any key` when using the `dotnet input` provider) to control height; and the arrow keys to rotate the camera. The `+` and `-` keys increase and decrease the scene's light intensity. In the scene with the 3D object, you can hold down the `Ctrl` key to move the light source.

## Example of Creating a Custom Scene (C#) (Interacting with the Engine API)

```csharp
using _3dEngine;
using _3dEngine.AbstractClass;
using _3dEngine.Implementation;
using _3dEngine.Interfaces;
using _3dEngine.Shape;

public class PreviewScene : Scene
{
    private Camera _camera;
    private Sphere _sphere;
    private Light _light;

    public PreviewScene(IDisplaysManagerAsync manager) : base(manager) { }

    public override void Start()
    {
        // Create a camera
        _camera = new Camera(new Vector3(-10, 0, 0), Vector3.Zero);
        SetMainCamera(_camera);

        // Create a red sphere at the origin
        _sphere = new Sphere(Vector3.Zero, Vector3.Zero, r: 1.5f);
        _sphere.Color = ConsoleColor.Red;
        AddDisplaysObject(_sphere);

        // Add a light source
        _light = new Light(new Vector3(-4, 3, -2), lightPower: 150f);
        AddLight(_light);
    }

    public override void Update()
    {
        // Rotate or move objects every frame using GameTime.GetDeltaTime()
    }
}
```

To run your custom scene, specify it in your `Program.cs` entry point when instantiating the `Frame`:

```csharp
using _3dEngine;
using _3dEngine.Implementation;

class Program
{
    static void Main()
    {
        new Frame(new MyCustomScene(new DisplayManagerAsync()), new ConsoleScreenAsync()).MainLoop();
    }
}
```


## Contributing

Neo 3D welcomes most pull requests, provided they comply with the [Contributing Guidelines](/.github/CONTRIBUTING.md).

However, requests for new features and deep architectural changes are accepted less frequently. To learn more, please read the [Why Are These Features Missing?](https://github.com/OxygenCobalt/Auxio/wiki/Why-Are-These-Features-Missing%3F) section.


## License

[![GNU GPLv3 Image](https://www.gnu.org/graphics/gplv3-127x51.png)](http://www.gnu.org/licenses/gpl-3.0.en.html)

Neo 3D is free software: you can use, study, distribute, build games with, and improve it as you see fit. Specifically, you can distribute and/or modify it under the terms of the GNU General Public License (GPL) as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

For more information, please see [here](https://github.com/IvanSobolev/Neo3dEngine?tab=GPL-3.0-1-ov-file).