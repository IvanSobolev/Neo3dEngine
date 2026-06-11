# 💻 Neo 3D Console (C#)

<img width="2070" height="1085" alt="Image" src="https://github.com/user-attachments/assets/06934758-90d4-4b63-9f40-8bb44ca2831e" />

A minimalist 3D engine written in C# that runs entirely inside the system console. This project was built from scratch without any external graphical libraries or modern graphics APIs (like OpenGL or DirectX). Everything—from custom vector mathematics to ray casting/tracing and the TCP network stack—is computed entirely on the CPU.

This project was designed for educational and demonstration purposes for a [YouTube video](https://youtu.be/vhYE882B9dE).

---

## 🚀 Key Features

1. **CPU Raycasting / Raytracing**:
    * Renders polygonal meshes (`.obj` format) and geometric spheres.
    * Intersection optimization using Bounding Sphere checks before performing detailed triangle intersection calculations.
2. **Dynamic Lighting and Shadows**:
    * Light intensity attenuation based on distance.
    * Lambertian diffuse shading using surface normals.
    * Real-time shadow rendering via Shadow Rays cast from intersection points back to light sources.
3. **Multi-threaded Rendering**:
    * Leverages `Parallel.For` to distribute pixel rendering calculations across all available CPU cores.
4. **Optimized Console Output**:
    * Map light intensity to a character gradient string (`" .:!/r(l1Z4H9W8$@"`) to simulate shading.
    * Frame buffering with grouped-color output to minimize slow, native OS terminal print API calls.
5. **Custom 3D Mathematics**:
    * Custom implementations of `Vector3`, `Vector2`, `Ray`, and rotation matrices (Euler rotations around X, Y, and Z axes).
    * Manual implementation of the Möller–Trumbore ray-triangle intersection algorithm.
6. **Low-level Window Input**:
    * Asynchronous, non-blocking keyboard polling using the Win32 API (`GetAsyncKeyState`), ensuring key presses are detected only when the console window is active.
7. **Custom Network Multiplayer**:
    * Client-server TCP network manager built from scratch.
    * Custom binary packet serialization, routing via stable hash-based type IDs, and a decoupled event subscription model.
    * Included multiplayer demo featuring real-time player position synchronization and a text-based chat lobby.

---

## 🏗️ Architecture Overview

The engine is decoupled into clear logical layers:

```
├── AbstractClass             # Base classes for GameObjects, Scenes, Screens, and Lights
├── Implementation            # Concrete cameras, rendering managers, and screen renderers
├── Interfaces                # Contracts for loose coupling (ICamera, IScreen, etc.)
├── Shape                     # Geometric primitives (Sphere, Triangle, Object3D)
├── StaticClass               # Helper utilities (ObjLoader, GameTime tracking)
├── Structure                 # Core structs (Vectors, Rays, RenderData payload)
├── UI                        # Overlay system for rendering text on top of the frame buffer
└── Network                   # Networking layer (TCP Manager, Packet abstractions, Serializer)
```

### Core Architecture Components

* **`Frame`**: The heart of the engine loop. It starts the cycle, updates the active scene's logic (`Update()`), triggers the screen renderer (`RenderFrame()`), prints FPS/diagnostics, and tracks delta-time.
* **`Screen` (ConsoleScreenAsync)**: Manages resolution buffers for brightness and color. Its `RenderFrame` implementation parallelizes the conversion of brightness values into gradient characters and presents the entire frame buffer to the console as grouped color chunks.
* **`Scene`**: A container for world elements. It manages the active camera, light lists, UI elements, and renderable objects (`IDisplays`). It computes individual pixel states via `GetPixelData` on demand.
* **`DisplaysManager`**: Calculates intersections, searching for the nearest object intersected by rays projected from the viewport.

---

## 📐 The Rendering Pipeline

Every frame is rendered using the following steps:

1. **Ray Generation**:
   The camera translates the 2D UV screen coordinates into a 3D direction vector in world space, adjusting for the camera's position and orientation (Pitch, Yaw, Roll).
2. **Intersection / Raycast**:
   The ray is evaluated against all active objects in the scene via the `DisplayManager`:
    * **Spheres**: Evaluated analytically using the quadratic formula for ray-sphere intersection.
    * **Polygonal Objects (3D Models)**: First, a quick intersection check is run against the model's Bounding Sphere. If the ray hits the sphere, a detailed loop checks all individual triangles using the Möller–Trumbore algorithm.
3. **Shading & Shadow Calculation**:
   If an intersection is found, the engine calculates the light influence at that point:
    * It determines the direction vector toward each light source.
    * It calculates the dot product between the surface normal and the light direction (diffuse component).
    * It casts a **Shadow Ray** from the intersection point toward the light. If another object intersects this shadow ray, the point is shaded as in shadow (brightness = 0).
4. **Buffering & Output**:
   The final light intensity is mapped to a character from the gradient array, buffered, and written out to the console terminal.

---

## ⌨️ Cross-Platform Input Architecture

The engine uses a decoupled **Strategy Pattern** to handle asynchronous, non-blocking input with native OS focus checks. This prevents "background input leakage" (ensuring the game only processes keypresses when your terminal window is actively focused).

```
[Input (Facade)] ---> [IInputProvider]
                            |
    +-----------------------+-----------------------+
    |                       |                       |                       
[User32 (Windows)]   [LibX11 (Linux)]        [DotNet (Fallback)]
```

*   **Windows (`User32InputProvider`)**:
    *   Direct hardware polling via `GetAsyncKeyState` [1.1.3].
    *   Active window verification: `GetForegroundWindow() == GetActualConsoleWindow()`. It natively resolves parent-owner relationships (`GetWindow` with `GW_OWNER`) [1.2.1, 2.1.2], supporting both legacy `conhost.exe` and modern tabbed **Windows Terminal** on Windows 11.
*   **Linux (`LibX11InputProvider`)**:
    *   Direct hardware polling using `XQueryKeymap` (fetching the full 256-bit keyboard state once per frame) [1, 2].
    *   Dynamic, layout-independent mapping: Translates standard `.NET` `ConsoleKey` values to X11 KeySyms, resolving them to the user's active layout (QWERTY, AZERTY, Cyrillic, etc.) at startup via `XKeysymToKeycode` [2.1.3].
    *   Focus detection via tree-climbing window ID checks, matching the active window against the console's environment `WINDOWID` or parent terminal names [1.2.1, 1.2.2].
*   **Universal Fallback (`DotNetInputProvider`)**:
    *   An asynchronous, thread-safe input queue wrapping `.NET`'s native `Console.ReadKey(true)` [2.1.8].
    *   Implements a **timeout-based key-release emulator** to simulate smooth real-time KeyUp events and multi-key combinations inside standard command-line pipes [2].
    *   Automatically activated in headless servers (SSH), pure Wayland sessions, or sandboxed containers [1].

---

## 🌐 Custom TCP Network Protocol

The networking module is written on raw sockets (`TcpListener`/`TcpClient`) with zero third-party dependencies.

* **Network Packet Layout**:
  Packets are serialized into a sequential byte array containing a 12-byte header followed by the payload data:
  ```
  [ Type ID (4 bytes) ] [ Sender ID (4 bytes) ] [ Payload Length (4 bytes) ] [  Payload Data (N bytes)  ]
  ```
* **Registration & Serialization (`PacketManager`)**:
  Packets inherit `INetworkPacket` and implement custom `Serialize`/`Deserialize` methods using `BinaryWriter`/`BinaryReader`. They are mapped to unique integer IDs using a stable hashing function on the class name string.
* **Event Dispatching**:
  Scripts subscribe to specific packet types asynchronously using the manager: `PacketManager.Subscribe<T>((packet, senderId) => { ... })`.

---

## 🛠️ Getting Started

To build and run this project, make sure you have the **.NET 8.0 SDK** (or newer) installed.

### Step 1: File Setup
Organize the codebase according to the directory structure. Make sure you have a valid `.obj` model file (such as Blender's classic low-poly Suzanne — `monkey.obj`) located in your output execution directory.

### Step 2: Build and Run
Navigate to your project directory and run:
```bash
dotnet run --configuration Release
```
*(Running with the `Release` configuration is highly recommended to ensure maximum parallel performance on your CPU).*

### Step 3: Multiplayer Setup
When the console starts, choose your network role:
1. Press **`S`** to host as a Server. The terminal will display your local IP and port. Share this with a client.
2. On another machine (or another terminal instance), press **`C`** to connect as a Client, input the Server's IP address, and connect.

### Default Controls:
* **`W, A, S, D`** — Move camera (Forward, Left, Backward, Right).
* **`Space` / `Left Shift`** — Fly Up / Down.
* **Arrow Keys** — Look around (Pitch and Yaw rotations).
* **`Ctrl` + `W, A, S, D, Space, Shift`** — Move the active light source.
* **`+` / `-`** — Increase / Decrease light intensity.
* **`T`** — Open multiplayer chat (Type message -> `Enter` to send, `Esc` to cancel).

---

## 📝 Creating a Custom Scene

You can create your own scene by inheriting from the base `Scene` class. Here is a simple example rendering a single red sphere illuminated by a light source:

```csharp
using _3dEngine;
using _3dEngine.AbstractClass;
using _3dEngine.Implementation;
using _3dEngine.Interfaces;
using _3dEngine.Shape;

public class MyCustomScene : Scene
{
    private Camera _camera;
    private Sphere _sphere;
    private Light _light;

    public MyCustomScene(IDisplaysManagerAsync manager) : base(manager) { }

    public override void Start()
    {
        // 1. Initialize and set up the camera
        _camera = new Camera(new Vector3(0, 0, -5), Vector3.Zero);
        SetMainCamera(_camera);

        // 2. Add a red sphere at the origin
        _sphere = new Sphere(Vector3.Zero, Vector3.Zero, r: 1.5f);
        _sphere.Color = ConsoleColor.Red;
        AddDisplaysObject(_sphere);

        // 3. Add a light source pointing at the sphere
        _light = new Light(new Vector3(-2, 3, -4), lightPower: 15f);
        AddLight(_light);
    }

    public override void Update()
    {
        // Add frame update logic here (e.g., rotate objects over time using GameTime.GetDeltaTime())
    }
}
```

[old repository](https://github.com/IvanSobolev/3dEngine)
