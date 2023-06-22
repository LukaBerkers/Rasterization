using OpenTK.Mathematics;

namespace Rasterization;

public class Node
{
    // Node world;
    private readonly List<Node> _children;
    private readonly Mesh? _mesh;
    private readonly Matrix4 _objectToParent;
    private readonly Shader _shade;
    private readonly Texture? _texture;

    public Node(Matrix4 objectToParent, Mesh? mesh, Shader shade, Texture? texture, List<Node> children)
    {
        _objectToParent = objectToParent;
        _mesh = mesh;
        _shade = shade;
        _texture = texture;
        _children = children;
    }

    public void Render(Matrix4 worldToScreen, Matrix4 parentToWorld)
    {
        var objectToWorld = _objectToParent * parentToWorld;
        var objectToScreen = objectToWorld * worldToScreen;
        if (_mesh != null || _texture != null) // null error
            _mesh.Render(_shade, objectToScreen, objectToWorld, _texture);

        foreach (var child in _children) child.Render(objectToScreen, objectToWorld);
    }
}