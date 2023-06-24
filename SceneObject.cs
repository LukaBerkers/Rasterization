using OpenTK.Mathematics;

namespace Rasterization;

public interface ISceneObject
{
    public Mesh Mesh { get; }
    public Texture Texture { get; }
    public Matrix4 ObjectTransformation { get; }
    public void Update(float frameDuration);
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
}

public class Floor : ISceneObject
{
    public Floor(Mesh mesh, Texture texture)
    {
        Mesh = mesh;
        Texture = texture;
    }

    public Mesh Mesh { get; }
    public Texture Texture { get; }
    public Matrix4 ObjectTransformation => Matrix4.CreateScale(4.0f);

    public void Update(float frameDuration)
    {
    }
}