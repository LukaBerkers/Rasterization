using OpenTK.Mathematics;

namespace Rasterization;

public class Light
{
    public Vector4 LightColor;
    public Vector4 Specular;
    public Vector3 LightPosition;

    public Light(Vector3 position, Vector4 color, Vector4 specular)
    {
        LightPosition = position;
        LightColor = color;
        Specular = specular;
    }
}