using OpenTK.Mathematics;

namespace Rasterization;

internal class Camera
{
    public Camera(Vector3 position, Vector3 lookAt)
    {
        Position = position;
        LookAt = lookAt;
    }

    public Vector3 Position { get; }
    public Vector3 LookAt { get; }

    private static readonly Matrix4 YToNegZ = Matrix4.CreateRotationX(-MathHelper.PiOver2);

    public Matrix4 Transformation()
    {
        var translation = Matrix4.CreateTranslation(-Position);
        // We want to rotate `LookAt` to -z-hat ((0, 0, -1)).
        // To avoid getting a zero-vector when `LookAt` is parallel to -z-hat, we instead first rotate it to y-hat, and
        // then rotate to -z-hat.
        // The rotation axis is the cross product of `LookAt` and the goal.
        // The angle to rotate is the angle between them.
        var rotation =
            YToNegZ
            * Matrix4.CreateFromAxisAngle
            (
                Vector3.Cross(LookAt, Vector3.UnitY),
                Vector3.CalculateAngle(LookAt, Vector3.UnitY)
            );
        return translation * rotation;
    }
}