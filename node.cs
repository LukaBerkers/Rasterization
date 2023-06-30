using OpenTK.Mathematics;

namespace Rasterization;

public class Node
{
    private readonly string _debugName;
    private readonly ISceneObject? _obj;
    private readonly Shader _shader;

    public Node(ISceneObject? obj, Shader shader, List<Node>? children = null, string? name = null)
    {
        _obj = obj;
        _shader = shader;
        Children = children ?? new List<Node>();
        _debugName = name ?? _obj?.Mesh.Filename ?? "Unknown object";
    }

    public List<Node> Children { get; }

    public void Update(float frameDuration)
    {
        _obj?.Update(frameDuration);
        foreach (var child in Children) child.Update(frameDuration);
    }

    public void Render(Matrix4 worldToScreen, Matrix4 parentToWorld, Frustum frustum)
    {
        Matrix4 objectToWorld;

        if (_obj is not null)
        {
            objectToWorld = _obj.ObjectTransformation * parentToWorld;

            var objectToScreen = objectToWorld * worldToScreen;
            Console.Error.WriteLine($"Render: {_debugName}");
            _obj.Mesh.Render(_shader, objectToScreen, objectToWorld, _obj.Texture);
        }
        else
        {
            objectToWorld = parentToWorld;
        }

        foreach (var child in Children) child.Render(worldToScreen, objectToWorld, frustum);
    }
}