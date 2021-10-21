using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareModifier : LandscapeModifier
{
    public Vector2 rectSize = new Vector2(20, 20);

    public float SmoothRadius = 2.0f;

    float altitude = 0;


    public override float GetAltitudeAtLocation(float PosX, float PosZ)
    {
        return altitude;
    }

    public override Rect computeBounds()
    {
        altitude = transform.position.y;
        return new Rect(new Vector2(transform.position.x - rectSize.x / 2, transform.position.z - rectSize.y / 2), rectSize);
    }

    private void OnDrawGizmos()
    {
        Vector3 worldScale = new Vector3(rectSize.x, 20, rectSize.y);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, worldScale);
    }

    public override float GetIncidenceAtLocation(float PosX, float PosZ)
    {
        float borderDistance = Mathf.Min(
            Mathf.Min(Mathf.Abs(PosX - worldBounds.xMin), Mathf.Abs(PosX - worldBounds.xMax)),
            Mathf.Min(Mathf.Abs(PosZ - worldBounds.yMin), Mathf.Abs(PosZ - worldBounds.yMax)));

        return Mathf.Clamp((borderDistance - SmoothRadius) / SmoothRadius, 0, 1);
    }
}
