using OpenTK.Mathematics;

namespace Rasterization;

public class Node
{
    private readonly ISceneObject? _obj;
    private readonly Shader _shader;

    public Node(ISceneObject? obj, Shader shader, List<Node>? children = null)
    {
        _obj = obj;
        _shader = shader;
        Children = children ?? new List<Node>();
    }

    // Node world;
    public List<Node> Children { get; }

    public void Update(float frameDuration)
    {
        _obj?.Update(frameDuration);
        foreach (var child in Children) child.Update(frameDuration);
    }

    public void Render(Matrix4 worldToScreen, Matrix4 parentToWorld)
    {
        var objectToParent = _obj?.ObjectTransformation ?? Matrix4.Identity;
        var objectToWorld = objectToParent * parentToWorld;
        var objectToScreen = objectToWorld * worldToScreen;
        _obj?.Mesh.Render(_shader, objectToScreen, objectToWorld, _obj.Texture);

        foreach (var child in Children) child.Render(worldToScreen, objectToWorld);
    }
}