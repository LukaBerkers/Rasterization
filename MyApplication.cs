using System.Diagnostics;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Rasterization;

internal class MyApplication
{
    private readonly Camera _camera;
    private readonly Stopwatch _timer = new(); // timer for measuring frame duration
    private readonly bool _useRenderTarget = true; // required for post processing
    private Shader? _postProc; // shader to use for post processing

    private ScreenQuad? _quad; // screen filling quad for post processing
    private RenderTarget? _target; // intermediate render target

    private Node? _world; // world node with no mesh

    // member variables
    public Surface Screen; // background surface for printing etc.

    // constructor
    public MyApplication(Surface screen)
    {
        Screen = screen;
        _camera = new Camera((0.0f, 6.0f, 8.0f), new Vector3(0.0f, 0.0f, -1.0f));
    }

    // initialize
    public void Init()
    {
        // load teapot
        var teapotMesh = new Mesh("../../../assets/teapot.obj");
        var floorMesh = new Mesh("../../../assets/floor.obj");
        // initialize stopwatch
        _timer.Reset();
        _timer.Start();
        // create shaders
        var shader = new Shader("../../../shaders/vs.glsl", "../../../shaders/fs.glsl");
        _postProc = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_post.glsl");
        // load a texture
        var wood = new Texture("../../../assets/wood.jpg");
        // create the render target
        if (_useRenderTarget) _target = new RenderTarget(Screen.Width, Screen.Height);
        _quad = new ScreenQuad();

        var teapot = new Teapot(teapotMesh, wood, 0.5f, (4.0f, 0.0f, 0.0f));
        var smallTeapot = new Teapot(teapotMesh, wood, 0.5f, (8.0f, 0.0f, 0.0f), false);
        var floor = new Floor(floorMesh, wood);
        _world = new Node(null, shader)
        {
            Children =
            {
                new Node(teapot, shader)
                {
                    Children = { new Node(smallTeapot, shader) }
                },
                new Node(floor, shader)
            }
        };
    }

    // tick for background surface
    public void Tick()
    {
        Screen.Clear(0);
        Screen.Print("hello world", 2, 2, 0xffff00);
    }

    public void Update(KeyboardState keyboardState, Vector2 mouseDelta)
    {
        const float moveSpeed = 1.0f / 4096.0f;
        const float rotateSpeed = 1.0f / 1024.0f;

        Camera.MoveDirection? direction = null;
        if (keyboardState[Keys.W])
            direction = Camera.MoveDirection.Forwards;
        else if (keyboardState[Keys.S])
            direction = Camera.MoveDirection.Backwards;
        else if (keyboardState[Keys.A])
            direction = Camera.MoveDirection.Left;
        else if (keyboardState[Keys.D]) direction = Camera.MoveDirection.Right;

        if (direction.HasValue) _camera.Move((Camera.MoveDirection)direction, moveSpeed);
        _camera.Pan(mouseDelta.X * rotateSpeed);
        _camera.Tilt(mouseDelta.Y * rotateSpeed);
    }

    // tick for OpenGL rendering code
    public void RenderGl()
    {
        // measure frame duration
        float frameDuration = _timer.ElapsedMilliseconds;
        _timer.Reset();
        _timer.Start();

        // Update scene
        _world?.Update(frameDuration);

        // prepare matrix for vertex shader
        var worldToCamera = _camera.Transformation();
        var cameraToScreen = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f),
            (float)Screen.Width / Screen.Height, .1f, 1000);

        if (_useRenderTarget && _target != null && _quad != null)
        {
            // enable render target
            _target.Bind();

            // render scene to render target
            _world?.Render(worldToCamera * cameraToScreen, Matrix4.Identity);

            // render quad
            _target.Unbind();
            if (_postProc != null)
                _quad.Render(_postProc, _target.GetTextureId());
        }
        else
        {
            // render scene directly to the screen
            _world?.Render(worldToCamera * cameraToScreen, Matrix4.Identity);
        }
    }
}