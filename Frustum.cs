using OpenTK.Mathematics;

namespace Rasterization;

// Partially based on https://learnopengl.com/Guest-Articles/2021/Scene/Frustum-Culling

internal class Frustum
{
    public readonly float NearDepth;
    public readonly float FarDepth;

    public readonly Plane Near;
    public readonly Plane Far;

    public readonly Plane Left;
    public readonly Plane Right;

    public readonly Plane Top;
    public readonly Plane Bottom;

    public Frustum(Vector3 eye, Vector3 lookAt, Vector3 up, float nearDepth, float farDepth)
    {
        NearDepth = nearDepth;
        FarDepth = farDepth;
        
        lookAt.Normalize();
        Near = new Plane(lookAt, nearDepth + Vector3.Dot(eye, lookAt));
        Far = new Plane(-lookAt, farDepth + Vector3.Dot(eye, -lookAt));
        
        up.Normalize();
        Top = new Plane(-up, eye);
        Bottom = new Plane(up, eye);
        
        var right = Vector3.Cross(lookAt, up);
        Left = new Plane(right, eye);
        Right = new Plane(-right, eye);
    }
}

internal readonly struct Plane
{
    public readonly Vector3 Normal;
    public readonly float DistanceToOrigin;

    /// <param name="normal">Should be unit length.</param>
    /// <param name="distanceToOrigin">Signed distance between plane and the world origin.</param>
    public Plane(Vector3 normal, float distanceToOrigin)
    {
        Normal = normal;
        DistanceToOrigin = distanceToOrigin;
    }

    /// <param name="normal">Should be unit length.</param>
    /// <param name="pointOnPlane">Any point on the plane in world space.</param>
    public Plane(Vector3 normal, Vector3 pointOnPlane)
    {
        Normal = normal;
        DistanceToOrigin = Vector3.Dot(Normal, pointOnPlane);
    }
}