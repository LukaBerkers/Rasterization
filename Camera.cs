using OpenTK.Mathematics;

namespace Rasterization;

internal class Camera
{
    public enum MoveDirection
    {
        Forwards,
        Backwards,
        Left,
        Right
    }

    private static readonly Matrix4 YToNegZ = Matrix4.CreateRotationX(-MathHelper.PiOver2);
    private Vector3 _lookAt;

    public Camera(Vector3 position, Vector3 lookAt)
    {
        Position = position;
        _lookAt = lookAt.Normalized();
        Right = Vector3.Cross(_lookAt, Vector3.UnitY);
    }

    public Vector3 Position { get; private set; }

    public Vector3 LookAt
    {
        get => _lookAt;
        private set
        {
            _lookAt = value.Normalized();
            Right = Vector3.Cross(_lookAt, Vector3.UnitY);
        }
    }

    public Vector3 Right { get; private set; }

    public Matrix4 Transformation()
    {
        // var translation = Matrix4.CreateTranslation(-Position);
        // // We want to rotate `LookAt` to -z-hat ((0, 0, -1)).
        // // To avoid getting a zero-vector when `LookAt` is parallel to -z-hat, we instead first rotate it to y-hat, and
        // // then rotate to -z-hat.
        // // The rotation axis is the cross product of `LookAt` and the goal.
        // // The angle to rotate is the angle between them.
        // var rotation =
        //     YToNegZ
        //     * Matrix4.CreateFromAxisAngle
        //     (
        //         Vector3.Cross(LookAt, Vector3.UnitY),
        //         Vector3.CalculateAngle(LookAt, Vector3.UnitY)
        //     );
        // return translation * rotation;

        return Matrix4.LookAt(Position, Position + LookAt, Vector3.UnitY);
    }

    public void Move(MoveDirection direction, float speed)
    {
        var offset = direction switch
        {
            MoveDirection.Forwards => LookAt,
            MoveDirection.Backwards => -LookAt,
            MoveDirection.Left => -Right,
            MoveDirection.Right => Right,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

        Position += speed * offset;
    }

    public void Pan(float value)
    {
        LookAt = Matrix3.CreateRotationY(value) * LookAt;
    }

    public void Tilt(float value)
    {
        LookAt = Matrix3.CreateFromAxisAngle(Right, value) * LookAt;
    }
}