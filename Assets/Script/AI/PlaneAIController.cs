using UnityEngine;

public class PlaneAIController : PlaneComponent, GPULandscapePhysicInterface
{
    // Start is called before the first frame update
    void Start()
    {
        Plane.SetThrustInput(1.0f);
    }

    void OnEnable()
    {
        GPULandscapePhysic.Singleton.AddListener(this);
    }
    void OnDisable()
    {
        GPULandscapePhysic.Singleton.RemoveListener(this);
    }

    AirportActor targetAirport;

    float[] frontAltitudes;
    Vector3 aiTarget;
    // Update is called once per frame
    void Update()
    {
        if (!targetAirport)
            targetAirport = AirportActor.GetClosestAirport(Plane.planeTeam, transform.position);
        Debug.Log("target airport : " + targetAirport.name);

        aiTarget = new Vector3(transform.position.x, 2000, transform.position.z) + new Vector3(transform.forward.x, 0, transform.forward.z).normalized * 1000;

        if (targetAirport)
            aiTarget = targetAirport.transform.position;

        AvoidGround(ref aiTarget);


        MoveTowardTarget(aiTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(aiTarget, 10);
        Gizmos.DrawLine(transform.position, aiTarget);
    }


    public void AvoidGround(ref Vector3 TargetPoint)
    {
        float? highestPoint = null;
        if (frontAltitudes == null)
            return;

        foreach (var point in frontAltitudes)
        {
            if (highestPoint == null || point > highestPoint.Value)
                highestPoint = point;
        }

        if (highestPoint != null)
        {
            if ((transform.position + Physics.velocity * 10).y < highestPoint.Value + 40)
            {
                TargetPoint = new Vector3(transform.forward.x, 0, transform.forward.z).normalized * 10 + new Vector3(transform.position.x, highestPoint.Value + 200, transform.position.z);
            }
        }
    }

    void MoveTowardTarget(Vector3 target)
    {
        Vector3 worldDirectionToTarget = (target - transform.position).normalized;

        Vector3 relativeDirectionToTarget = transform.InverseTransformDirection(worldDirectionToTarget);

        Plane.SetYawInput(relativeDirectionToTarget.x * 1);
        Plane.SetRollInput(relativeDirectionToTarget.x * 1);
        Plane.SetPitchInput(relativeDirectionToTarget.y * -1);
    }

    int pointCount = 20;
    float pointSpacing = 30;

    public Vector2[] Collectpoints()
    {
        Vector2[] points = new Vector2[pointCount];
        for (int i = 1; i <= pointCount; ++i)
        {
            Vector3 worldPos = transform.position + new Vector3(Physics.velocity.x, 0, Physics.velocity.z).normalized * pointSpacing * i;
            points[i - 1] = new Vector2(worldPos.x, worldPos.z);
        }
        return points;
    }

    public void OnPointsProcessed(float[] processedPoints)
    {
        frontAltitudes = processedPoints;
    }
}
