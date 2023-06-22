using System.Diagnostics;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Rasterization;

internal class MyApplication
{
    private readonly Camera _camera;
    private readonly Stopwatch _timer = new(); // timer for measuring frame duration
    private readonly bool _useRenderTarget = true; // required for post processing
    private float _a; // teapot rotation angle
    private Shader? _postProc; // shader to use for post processing

    private ScreenQuad? _quad; // screen filling quad for post processing
    private Shader? _shader; // shader to use for rendering
    private RenderTarget? _target; // intermediate render target
    private Mesh? _teapot, _teapot2, _floor; // meshes to draw using OpenGL
    private Texture? _wood; // texture to use for rendering

    private Node World; // world node with no mesh

    // private Node TPOT2;

    private Node TPOT;
    private Node FLOOR;

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
        _teapot = new Mesh("../../../assets/teapot.obj");
        _teapot2 = new Mesh("../../../assets/teapot.obj");
        _floor = new Mesh("../../../assets/floor.obj");
        // initialize stopwatch
        _timer.Reset();
        _timer.Start();
        // create shaders
        _shader = new Shader("../../../shaders/vs.glsl", "../../../shaders/fs.glsl");
        _postProc = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_post.glsl");
        // load a texture
        _wood = new Texture("../../../assets/wood.jpg");
        // create the render target
        if (_useRenderTarget) _target = new RenderTarget(Screen.Width, Screen.Height);
        _quad = new ScreenQuad();
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

        // prepare matrix for vertex shader
        var angle90degrees = MathF.PI / 2;
        var teapotObjectToWorld = Matrix4.CreateScale(0.5f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), _a);
        var teapot2ObjectToWorld = Matrix4.CreateScale(0.5f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 4, 0), _a);
        var floorObjectToWorld = Matrix4.CreateScale(4.0f); //* Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), _a);
        var worldToCamera = _camera.Transformation();
        var cameraToScreen = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f),
            (float)Screen.Width / Screen.Height, .1f, 1000);

        // TPOT2 = new Node(teapot2ObjectToWorld, _teapot2, _shader, _wood, null);
        TPOT = new Node(teapotObjectToWorld, _teapot, _shader, _wood, null);
        FLOOR = new Node(floorObjectToWorld, _floor, _shader, _wood, null);
        var children = new List<Node> { TPOT, FLOOR };
        World = new Node(Matrix4.Identity, null, _shader, null, children);
        World.Render(worldToCamera, Matrix4.Identity);

        // update rotation
        _a += 0.001f * frameDuration;
        if (_a > 2 * MathF.PI) _a -= 2 * MathF.PI;

        if (_useRenderTarget && _target != null && _quad != null)
        {
            // enable render target
            _target.Bind();

            // render scene to render target
            if (_shader != null && _wood != null)
            {
                // _teapot?.Render(_shader, teapotObjectToWorld * worldToCamera * cameraToScreen, teapotObjectToWorld,
                //      _wood);
                TPOT.Render(worldToCamera * cameraToScreen, Matrix4.Identity);
                //  TPOT2.render(worldToCamera * cameraToScreen, Matrix4.Identity);
                // _floor?.Render(_shader, floorObjectToWorld * worldToCamera * cameraToScreen, floorObjectToWorld, _wood);
                FLOOR.Render(worldToCamera * cameraToScreen, Matrix4.Identity);
            }

            // render quad
            _target.Unbind();
            if (_postProc != null)
                _quad.Render(_postProc, _target.GetTextureId());
        }
        else
        {
            // render scene directly to the screen
            if (_shader != null && _wood != null)
            {
                // _teapot?.Render(_shader, teapotObjectToWorld * worldToCamera * cameraToScreen, teapotObjectToWorld,
                //     _wood);
                TPOT.Render(worldToCamera * cameraToScreen, Matrix4.Identity);
                // TPOT2.render(worldToCamera * cameraToScreen, Matrix4.Identity);
                // _floor?.Render(_shader, floorObjectToWorld * worldToCamera * cameraToScreen, floorObjectToWorld, _wood);
                FLOOR.Render(worldToCamera * cameraToScreen, Matrix4.Identity);
            }
        }
    }
}