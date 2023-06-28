using OpenTK.Mathematics;

namespace Rasterization;

public class Light
{
    public Vector4 LightColor;
    public Vector3 LightPosition;

    public Light(Vector3 position, Vector4 color)
    {
        LightPosition = position;
        LightColor = color;
    }
}