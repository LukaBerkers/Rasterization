﻿using System.Globalization;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

// The template provides you with a window which displays a 'linear frame buffer', i.e.
// a 1D array of pixels that represents the graphical contents of the window.

// Under the hood, this array is encapsulated in a 'Surface' object, and copied once per
// frame to an OpenGL texture, which is then used to texture 2 triangles that exactly
// cover the window. This is all handled automatically by the template code.

// Before drawing the two triangles, the template calls the Tick method in MyApplication,
// in which you are expected to modify the contents of the linear frame buffer.

// After (or instead of) rendering the triangles you can add your own OpenGL code.

// We will use both the pure pixel rendering as well as straight OpenGL code in the
// tutorial. After the tutorial you can throw away this template code, or modify it at
// will, or maybe it simply suits your needs.

namespace Rasterization;

public class OpenTKApp : GameWindow
{
    public const bool AllowPrehistoricOpenGl = false;

    private static int _screenId; // unique integer identifier of the OpenGL texture
    private static MyApplication? _app; // instance of the application
    private static bool _terminated; // application terminates gracefully when this is true

    private ScreenQuad _quad;
    private Shader _screenShader;

    public OpenTKApp()
        : base(GameWindowSettings.Default, new NativeWindowSettings
        {
            Size = new Vector2i(960, 540),
            Profile = AllowPrehistoricOpenGl
                ? ContextProfile.Compatability
                : ContextProfile.Core, // required for fixed-function, which is probably not supported on MacOS
            Flags =
                (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                    ? ContextFlags.Default
                    : ContextFlags.Debug) // enable error reporting (not supported on MacOS)
                | (AllowPrehistoricOpenGl ? ContextFlags.Default : ContextFlags.ForwardCompatible) // required for MacOS
        })
    {
    }

    public void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length,
        IntPtr message, IntPtr userParam)
    {
        var sourceStrings = new Dictionary<DebugSource, string>
        {
            { DebugSource.DebugSourceApi, "API - A call to the OpenGL API" },
            { DebugSource.DebugSourceWindowSystem, "Window System - A call to a window system API" },
            { DebugSource.DebugSourceShaderCompiler, "Shader Compiler" },
            { DebugSource.DebugSourceThirdParty, "Third Party - A third party application associated with OpenGL" },
            {
                DebugSource.DebugSourceApplication,
                "Application - A call to GL.DebugMessageInsert() in this application"
            },
            { DebugSource.DebugSourceOther, "Other" },
            { DebugSource.DontCare, "Ignored" }
        };
        string? sourceString;
        if (!sourceStrings.TryGetValue(source, out sourceString)) sourceString = "Unknown";
        var typeString = Enum.GetName(type);
        if (typeString != null) typeString = typeString.Substring(9);
        var severityString = Enum.GetName(severity);
        if (severityString != null) severityString = severityString.Substring(13);
        Console.Error.WriteLine("OpenGL Error:\n  Source: " + sourceString + "\n  Type: " + typeString +
                                "\n  Severity: " + severityString
                                + "\n  Message ID: " + id + "\n  Message: " + Marshal.PtrToStringAnsi(message, length) +
                                "\n");
    } // put a breakpoint here and inspect the stack to pinpoint where the error came from

    protected override void OnLoad()
    {
        base.OnLoad();
        // called during application initialization
        Console.WriteLine("OpenGL Version: " + GL.GetString(StringName.Version) + " (" +
                          (Profile == ContextProfile.Compatability ? "Compatibility" : Profile) + " profile)");
        Console.WriteLine("OpenGL Renderer: " + GL.GetString(StringName.Renderer) +
                          (GL.GetString(StringName.Vendor) == "Intel"
                              ? " (read DiscreteGPU.txt if you have another GPU that you would like to use)"
                              : ""));
        // configure debug output (not supported on MacOS)
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            GL.Enable(EnableCap.DebugOutput);
            // disable all debug messages
            GL.DebugMessageControl(DebugSourceControl.DontCare, DebugTypeControl.DontCare,
                DebugSeverityControl.DontCare, 0, new int[0], false);
            // enable selected debug messages based on source, type, and severity
            foreach (var source in new[]
                         { DebugSourceControl.DebugSourceApi, DebugSourceControl.DebugSourceShaderCompiler })
            foreach (var type in new[]
                     {
                         DebugTypeControl.DebugTypeError, DebugTypeControl.DebugTypeDeprecatedBehavior,
                         DebugTypeControl.DebugTypeUndefinedBehavior, DebugTypeControl.DebugTypePortability
                     })
            foreach (var severity in new[] { DebugSeverityControl.DebugSeverityHigh })
                GL.DebugMessageControl(source, type, severity, 0, new int[0], true);
            GL.DebugMessageCallback(DebugCallback, (IntPtr)0);
        }

        // prepare for rendering
        GL.ClearColor(0, 0, 0, 0);
        GL.Disable(EnableCap.DepthTest);
        Surface screen = new(ClientSize.X, ClientSize.Y);
        _app = new MyApplication(screen);
        _screenId = _app.Screen.GenTexture();
        if (AllowPrehistoricOpenGl)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
        }

        _quad = new ScreenQuad();
        _screenShader = new Shader("../../../shaders/screen_vs.glsl", "../../../shaders/screen_fs.glsl");
        CursorGrabbed = true;
        _app.Init();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        // called upon app close
        GL.DeleteTextures(1, ref _screenId);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        // called upon window resize. Note: does not change the size of the pixel buffer.
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        if (AllowPrehistoricOpenGl)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
        }
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        // called once per frame; app logic
        var keyboard = KeyboardState;
        var mouseDelta = MouseState.Delta;
        _app?.Update(keyboard, mouseDelta);
        if (keyboard[Keys.Escape]) _terminated = true;
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        // called once per frame; render
        if (_app != null) _app.Tick();
        if (_terminated)
        {
            Close();
            return;
        }

        // convert MyApplication.screen to OpenGL texture
        if (_app != null)
        {
            GL.ClearColor(Color4.Black);
            GL.Disable(EnableCap.DepthTest);
            GL.BindTexture(TextureTarget.Texture2D, _screenId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                _app.Screen.Width, _app.Screen.Height, 0,
                PixelFormat.Bgra,
                PixelType.UnsignedByte, _app.Screen.Pixels
            );
            if (AllowPrehistoricOpenGl)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.Color3(1.0f, 1.0f, 1.0f);
                // draw screen filling quad
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0.0f, 1.0f);
                GL.Vertex2(-1.0f, -1.0f);
                GL.TexCoord2(1.0f, 1.0f);
                GL.Vertex2(1.0f, -1.0f);
                GL.TexCoord2(1.0f, 0.0f);
                GL.Vertex2(1.0f, 1.0f);
                GL.TexCoord2(0.0f, 0.0f);
                GL.Vertex2(-1.0f, 1.0f);
                GL.End();
                GL.Disable(EnableCap.Texture2D);
            }

            _quad.Render(_screenShader, _screenId);
            // prepare for generic OpenGL rendering
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            // do OpenGL rendering
            _app.RenderGl();
        }

        // tell OpenTK we're done rendering
        SwapBuffers();
    }

    public static void Main()
    {
        // entry point
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        using OpenTKApp app = new();
        app.RenderFrequency = 30.0;
        app.Run();
    }
}