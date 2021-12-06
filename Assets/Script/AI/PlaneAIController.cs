using UnityEngine;

enum EAiMode
{
    Unknown,
    TakeOff,
    Flying,
}

/// <summary>
/// An AI component that can controll a plane it is attached to.
/// Basically it's like a simulated player that will act on the plane inputs.
/// At the moment, it can take off, navigate, and use the aim9 weapons
/// 
/// The AI Will automatically attack planes that are not in it's team
/// 
/// //@TODO : use splines and curves to get better paths prediction
/// </summary>
public class PlaneAIController : PlaneComponent, GPULandscapePhysicInterface
{
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

    /// <summary>
    /// The airport the AI is going to
    /// </summary>
    AirportActor targetAirport;

    /// <summary>
    /// If going to an airport, it will move around to a random point
    /// </summary>
    Vector3 TargetOffset = Vector3.zero;

    /// <summary>
    /// And change target position every x seconds
    /// </summary>
    float targetDelay = 0;

    EAiMode AICurrentMode = EAiMode.Unknown;

    /// <summary>
    /// The altitude of the point the plane is supposed to go to (used to avoid landscape)
    /// </summary>
    float[] nextAltitudes;
    Vector3 aiTargetDirection;
    Vector3 pathDirection;
    Vector3 groundAzimuthPosition;


    // Update is called once per frame
    void Update()
    {
        // Force the input of the plane
        Plane.ParkingBrakes = false;
        Plane.MainPower = true;
        Plane.OpenCanopy = false;
        Plane.LandingLights = true;
        Plane.PositionLight = 1;
        Plane.CockpitFloodLights = 1;
        if (Plane.GetCurrentPower() < 100)
            Plane.EnableAPU = true;
        Plane.ThrottleNotch = true;
        WeaponSystem.IsToggledOn = true;
        if (WeaponSystem.CurrentWeaponMode != WeaponMode.Pod_Air)
            WeaponSystem.AirAirMode();
        if (AICurrentMode == EAiMode.Flying)
            Plane.RetractGear = true;

        // Update AI state
        UpdateAIMode();
        // Select the target direction depending on the state
        aiTargetDirection = SelectTargetDirection();
        // Ensure we will not hit the ground
        pathDirection = AvoidGround(aiTargetDirection);
        // The make the input match the desired direction
        MoveTowardDirection(pathDirection);
    }

    void UpdateAIMode()
    {
        // If the velocity is bellow 20, we can considere we are alligned on a runway and we can take off
        if (AICurrentMode == EAiMode.Unknown)
        {
            AICurrentMode = Physics.velocity.magnitude < 20 ? EAiMode.TakeOff : EAiMode.Flying;
            return;
        }
        // Else we are in flight, so we can retract gear and start the mission
        if (AICurrentMode == EAiMode.TakeOff && nextAltitudes != null)
        {
            if (nextAltitudes[0] < transform.position.y - 500)
            {
                Plane.RetractGear = true;
                AICurrentMode = EAiMode.Flying;
            }
        }
    }

    static Vector3 OverrideYAxis(Vector3 inVector, float yAxis)
    {
        return new Vector3(inVector.x, yAxis, inVector.z);
    }

    /// <summary>
    /// Timer used to avoid a missile spam
    /// </summary>
    float shootTimer = 0;

    /// <summary>
    /// Find the direction to take depending on the mode
    /// </summary>
    /// <returns></returns>
    Vector3 SelectTargetDirection()
    {
        Vector3 direction = OverrideYAxis(transform.forward, 0).normalized;
        switch (AICurrentMode)
        {
            case EAiMode.Unknown:
                break;
            case EAiMode.TakeOff:
                direction = new Vector3(-1, 0.5f, 0).normalized;
                break;
            case EAiMode.Flying:
                // Search the nearest available enemy
                //@TODO use radar instead
                bool found = false;
                foreach (var plane in PlaneActor.PlaneList)
                {
                    if (plane.planeTeam != Plane.planeTeam)
                    {
                        // Make the desired position match the target position
                        direction = plane.transform.position - transform.position;
                        found = true;
                        // If the target is locked and in range, Fire !
                        if (Vector3.Distance(plane.transform.position, transform.position) < 1500 && Vector3.Dot(transform.forward, direction.normalized) > 0.98f && IrDetectorComponent.acquiredTarget && IrDetectorComponent.acquiredTarget.GetComponent<PlaneActor>().planeTeam != Plane.planeTeam)
                        {
                            if (shootTimer > 10)
                            {
                                shootTimer = 0;
                                Plane.Shoot(plane.gameObject);
                            }
                        }

                        break;
                    }
                }
                // If no target is found, go to the nearest airport
                if (!found)
                {
                    if (!targetAirport)
                        targetAirport = AirportActor.GetClosestAirport(Plane.planeTeam, transform.position);
                    if (targetAirport)
                        direction = (targetAirport.transform.position + TargetOffset) - transform.position;

                    if (targetDelay <= 0)
                    {
                        targetDelay = 60;
                        TargetOffset = new Vector3(Random.Range(-10000, 10000), Random.Range(200, 1200), Random.Range(-10000, 10000));
                    }
                }
                break;
        }
        targetDelay -= Time.deltaTime;
        shootTimer += Time.deltaTime;

        return direction.normalized;
    }


    private void OnDrawGizmos()
    {
        // Draw target direction
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + aiTargetDirection * 200, 10);
        Gizmos.DrawLine(transform.position, transform.position + pathDirection * 200);

        // Draw actual direction plus safe path
        Gizmos.color = Color.red;
        var nextPoints = GetNextPoints();
        if (nextAltitudes != null)
        {
            for (int i = 0; i < Mathf.Min(nextPoints.Length, nextAltitudes.Length); ++i)
                Gizmos.DrawLine(nextPoints[i], new Vector3(nextPoints[i].x, nextAltitudes[i], nextPoints[i].z));
        }

        // Draw Velocity
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(transform.position, transform.position + Physics.velocity.normalized * pointCount * pointSpacing);
        Vector3 velocityRightVector = Quaternion.AngleAxis(90, new Vector3(0, 1, 0)) * OverrideYAxis(Physics.velocity, 0).normalized;
        Gizmos.DrawLine(transform.position, transform.position + velocityRightVector * 100);

        // Draw safe direction
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, groundAzimuthPosition);
    }

    /// <summary>
    /// Predicted points
    /// </summary>
    int pointCount = 10;
    /// <summary>
    /// Spacing of the next predicted points in m
    /// </summary>
    float pointSpacing = 120;
    /// <summary>
    /// Max estimmed rotation per step
    /// </summary>
    float degree = 12;
    /// <summary>
    /// Min allowed altitude
    /// </summary>
    float safeAltitude = 200;

    /// <summary>
    /// Compute the coordinate of the next predicted passage points of the plane
    /// </summary>
    /// <returns></returns>
    public Vector3[] GetNextPoints()
    {
        Vector3 currentDirection = Physics.velocity.normalized;
        Vector3 currentPosition = transform.position - currentDirection * 0.2f;

        Vector3[] points = new Vector3[pointCount];
        for (int i = 0; i < pointCount; ++i)
        {
            currentDirection = Quaternion.AngleAxis(Mathf.Clamp(Vector3.SignedAngle(OverrideYAxis(currentDirection, 0), OverrideYAxis(aiTargetDirection, 0), new Vector3(0, 1, 0)), -degree, degree), new Vector3(0, 1, 0)) * currentDirection;

            currentDirection.Normalize();
            currentPosition += currentDirection * pointSpacing;
            points[i] = currentPosition;
        }

        return points;
    }

    /// <summary>
    /// From a givent direction, return the direction, or a corrected direction that will make the plane avoid obstacles
    /// </summary>
    /// <param name="aiPathDirection"></param>
    /// <returns></returns>
    public Vector3 AvoidGround(Vector3 aiPathDirection)
    {
        var nextPoints = GetNextPoints();
        float? MaxAngle = null;
        Vector3 velocityRightVector = Quaternion.AngleAxis(90, new Vector3(0, 1, 0)) * new Vector3(Physics.velocity.x, 0, Physics.velocity.z).normalized;

        if (nextAltitudes == null)
            return aiPathDirection;

        // Compute safe point
        for (int i = 0; i < Mathf.Min(nextPoints.Length, nextAltitudes.Length); ++i)
        {
            Vector3 GroundPosition = OverrideYAxis(nextPoints[i], nextAltitudes[i] + safeAltitude);

            float angle = Vector3.SignedAngle(GroundPosition - transform.position, nextPoints[i] - transform.position, velocityRightVector);
            if (MaxAngle == null || angle > MaxAngle.Value)
            {
                MaxAngle = angle;
                groundAzimuthPosition = GroundPosition;
            }
        }

        // Pull UP !!!
        if (Vector3.SignedAngle(groundAzimuthPosition - transform.position, Physics.velocity, velocityRightVector) > 0)
            return (groundAzimuthPosition - transform.position).normalized;
        return aiPathDirection;
    }

    void MoveTowardDirection(Vector3 worldDirectionToPath)
    {
        // If the target is in front of the plane, compensate the gravity effect by overshooting a bit the target
        if (transform.InverseTransformDirection(worldDirectionToPath).z > 0)
        {
            float differentialAngle = Vector3.Angle(Physics.velocity, worldDirectionToPath);
            float CorrectionRatio = 1.8f;
            Vector3 directionWithCorrection = Quaternion.AngleAxis(differentialAngle * CorrectionRatio, Vector3.Cross(Physics.velocity, worldDirectionToPath)) * Physics.velocity;
            worldDirectionToPath = directionWithCorrection;
        }

        Vector3 relativeDirectionToTarget = transform.InverseTransformDirection(worldDirectionToPath).normalized;
        Vector3 RelativeAngularVelocity = transform.InverseTransformDirection(Physics.angularVelocity);
        Plane.SetYawInput(relativeDirectionToTarget.x * 10);
        Plane.SetRollInput(relativeDirectionToTarget.x * 3 + RelativeAngularVelocity.z * 1.0f);
        Plane.SetPitchInput(relativeDirectionToTarget.y * -10 - RelativeAngularVelocity.x * 5.0f);
    }

    // Landscape Physics
    public Vector2[] Collectpoints()
    {
        Vector3[] points3D = GetNextPoints();
        Vector2[] points2D = new Vector2[points3D.Length];
        for (int i = 0; i < points3D.Length; ++i)
            points2D[i] = new Vector2(points3D[i].x, points3D[i].z);
        return points2D;
    }

    // Landscape Physics
    public void OnPointsProcessed(float[] processedPoints)
    {
        nextAltitudes = processedPoints;
    }
}
