using OpenTK.Mathematics;

namespace Rasterization;

internal class Camera
{
    public enum MoveDirection
    {
        Forwards,
        Backwards,
        Left,
        Right,
        Up,
        Down
    }

    private static readonly Vector3 WorldUp = Vector3.UnitY;

    private Vector3 _lookAt;

    public Camera(Vector3 position, Vector3 lookAt)
    {
        Position = position;
        _lookAt = lookAt.Normalized();
        Right = Vector3.Cross(_lookAt, WorldUp);
    }

    public Vector3 Position { get; private set; }

    public Vector3 LookAt
    {
        get => _lookAt;
        private set
        {
            _lookAt = value.Normalized();
            Right = Vector3.Cross(_lookAt, WorldUp);
        }
    }

    public Vector3 Right { get; private set; }

    public Matrix4 Transformation => Matrix4.LookAt(Position, Position + LookAt, WorldUp);

    public void Move(MoveDirection direction, float speed)
    {
        var offset = direction switch
        {
            MoveDirection.Forwards => LookAt,
            MoveDirection.Backwards => -LookAt,
            MoveDirection.Left => -Right,
            MoveDirection.Right => Right,
            MoveDirection.Up => WorldUp,
            MoveDirection.Down => -WorldUp,
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