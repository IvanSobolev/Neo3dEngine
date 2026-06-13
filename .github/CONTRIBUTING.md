# Contributing to Neo 3d

Thank you for your interest in contributing to Neo 3d! We are excited to have you. Neo 3d aims to be a fast, minimalist, and educational 3D console engine in C# built without heavy external graphics libraries.

Please take a moment to review these guidelines before submitting issues or creating pull requests.

---

## Crashes & Bugs

If you find a bug in rendering, the networking stack, or the input system, please log it in the [Issues](https://github.com/IvanSobolev/Neo3dEngine/issues) tab.

Before reporting a bug, please make sure:
*   **Has it been reported?** Check the open issues to see if it is already being tracked.
*   **Has it already been fixed?** Check if the bug is still present in the **`development`** branch, as it might have already been resolved there.
*   **Is it reproducible?** Make sure you are using the latest version of the .NET SDK (8.0 or newer) and a clean build.

When submitting a bug report, please provide:
1.  **A clear description** of the problem and the expected behavior.
2.  **Step-by-step instructions** to reproduce the issue.
3.  **Your environment details** (this is highly critical for input/rendering issues):
    *   Operating System (e.g., Windows 11, Ubuntu 24.04).
    *   Terminal Emulator (e.g., standard Windows conhost, Windows Terminal, xterm, gnome-terminal).
    *   Display Server (for Linux: X11 or Wayland).
4.  **A stack trace or logs** if the engine crashed.

---

## Feature Requests

If you have ideas for improving the rendering pipeline, optimizing 3D math, or extending network features, feel free to open a feature request in the [Issues](https://github.com/IvanSobolev/Neo3dEngine/issues) tab.

Please keep the engine's core philosophy in mind:
*   **High Performance:** Whether computations run on the CPU or transition to the GPU (Vulkan) in future updates, code paths must remain highly optimized.
*   **Zero External Dependencies:** The project is built from scratch without external graphics or windowing NuGet packages. Proposals requiring heavy third-party dependencies will likely be declined.

When requesting a feature, please describe:
1.  What exactly you would like to see added.
2.  What problem this feature solves.
3.  How it might impact the engine's overall performance.

---

## Code Contributions

If you want to fix a bug or implement a new feature yourself, we welcome your Pull Requests!

### Rules & Workflow:

1.  **Target Branch:** All pull requests must target the **`development`** branch. The `main` branch is reserved for stable releases only.
2.  **Discuss Before Working:** For major architectural changes, math refactoring, or networking/input redesigns, please **open an issue first** to discuss your ideas. This ensures your work aligns with the project's direction before you invest too much time.
3.  **No Third-Party Packages:** Do not introduce external NuGet packages. All features should be self-contained or rely strictly on the standard .NET library.
4.  **Cross-Platform Compatibility:** The input system (`IInputProvider`) and console buffers are tailored for different platforms. If you modify these areas, please try to test your changes on both Windows (Win32 API) and Linux (X11) where possible.
5.  **Performance Focus:** Code changes in hot paths (rendering loops, vector/matrix math, buffer updates) should not cause performance regressions. Benchmarking critical code paths is highly recommended.
6.  **Coding Style:** Follow standard C# formatting and coding conventions (Microsoft guidelines). Keep code clean, readable, and add comments for complex mathematical or low-level sections.
7.  **Testing:** Thoroughly test your changes locally before submitting a PR. Untested code or changes that cause instability will not be merged.

Thank you for helping us make Neo 3d better!
