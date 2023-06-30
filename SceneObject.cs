using OpenTK.Mathematics;

namespace Rasterization;

public interface ISceneObject
{
    public Mesh Mesh { get; }
    public Texture Texture { get; }
    public Matrix4 ObjectTransformation { get; }
    public void Update(float frameDuration);
    public bool IsBehindPlane(Plane plane);
    public bool IsOutsideFrustum(Frustum frustum);
}

public class Teapot : ISceneObject
{
    private readonly Matrix4 _baseRotation;
    private readonly Vector3 _position;
    private readonly bool _rotate;
    private readonly float _scale;

    private float _angle;

    public Teapot(Mesh mesh, Texture texture, float scale, Vector3 up, Vector3 position, bool rotate = true)
    {
        Mesh = mesh;
        Texture = texture;
        _scale = scale;
        _position = position;
        _rotate = rotate;
        var baseRotationAxis = Vector3.Cross(up, Vector3.UnitY);
        if (baseRotationAxis == Vector3.Zero)
        {
            _baseRotation = Matrix4.Identity;
        }
        else
        {
            var baseRotationAngle = -Vector3.CalculateAngle(up, Vector3.UnitY);
            _baseRotation = Matrix4.CreateFromAxisAngle(baseRotationAxis, baseRotationAngle);
        }
    }

    // Assumes the specific teapot texture
    private float Radius => 11.5f * _scale;

    public Mesh Mesh { get; }
    public Texture Texture { get; }

    public Matrix4 ObjectTransformation =>
        Matrix4.CreateScale(_scale)
        * Matrix4.CreateRotationY(_angle)
        * _baseRotation
        * Matrix4.CreateTranslation(_position);

    public void Update(float frameDuration)
    {
        if (!_rotate) return;
        _angle += 0.001f * frameDuration;
        if (_angle > 2 * MathF.PI) _angle -= 2 * MathF.PI;
    }

    public bool IsBehindPlane(Plane plane)
    {
        return plane.SignedDistanceToPoint(_position) < -Radius;
    }

    public bool IsOutsideFrustum(Frustum frustum)
    {
        return
            IsBehindPlane(frustum.Near);
        // || IsBehindPlane(frustum.Left)
        // || IsBehindPlane(frustum.Right)
        // || IsBehindPlane(frustum.Bottom)
        // || IsBehindPlane(frustum.Top)
        // || IsBehindPlane(frustum.Far);
    }
}

public class Floor : ISceneObject
{
    // Needs to be changed if the floor can be at different positions
    private const float Radius = 11.5f * Scale;
    private const float Scale = 4.0f;

    public Floor(Mesh mesh, Texture texture)
    {
        Mesh = mesh;
        Texture = texture;
    }

    private static Vector3 Position => Vector3.Zero;

    public Mesh Mesh { get; }
    public Texture Texture { get; }
    public Matrix4 ObjectTransformation => Matrix4.CreateScale(Scale);

    public void Update(float frameDuration)
    {
    }

    public bool IsBehindPlane(Plane plane)
    {
        return plane.SignedDistanceToPoint(Position) < -Radius;
    }

    public bool IsOutsideFrustum(Frustum frustum)
    {
        return
            IsBehindPlane(frustum.Near)
            || IsBehindPlane(frustum.Left)
            || IsBehindPlane(frustum.Right)
            || IsBehindPlane(frustum.Bottom)
            || IsBehindPlane(frustum.Top);
        // || IsBehindPlane(frustum.Far);
    }
}