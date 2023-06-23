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
    private float _angle;

    public Teapot(Mesh mesh, Texture texture)
    {
        Mesh = mesh;
        Texture = texture;
    }

    public Mesh Mesh { get; }
    public Texture Texture { get; }

    public Matrix4 ObjectTransformation =>
        Matrix4.CreateScale(0.5f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), _angle);

    public void Update(float frameDuration)
    {
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