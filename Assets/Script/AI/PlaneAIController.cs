using UnityEngine;


enum EAiMode
{
    Unknown,
    TakeOff,
    Flying,
}

// WIP : AI controller => joueur IA a attacher a un avion pour le rendre autonome (l'avion n'est plus controllable par la suite)
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
    Vector3 TargetOffset = Vector3.zero;
    float targetDelay = 0;

    EAiMode AICurrentMode = EAiMode.Unknown;

    float[] nextAltitudes;
    Vector3 aiTargetDirection;
    Vector3 pathDirection;
    Vector3 groundAzimuthPosition;
    // Update is called once per frame
    void Update()
    {
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

        UpdateAIMode();
        aiTargetDirection = SelectTargetDirection();
        pathDirection = AvoidGround(aiTargetDirection);
        MoveTowardDirection(pathDirection);
    }

    void UpdateAIMode()
    {
        if (AICurrentMode == EAiMode.Unknown)
        {
            AICurrentMode = Physics.velocity.magnitude < 20 ? EAiMode.TakeOff : EAiMode.Flying;
            return;
        }
        if (AICurrentMode == EAiMode.TakeOff && nextAltitudes != null)
        {
            if (nextAltitudes[0] < transform.position.y - 500)
            {
                Plane.RetractGear = true;
                AICurrentMode = EAiMode.Flying;
            }
        }
    }

    public static Vector3 OverrideYAxis(Vector3 inVector, float yAxis)
    {
        return new Vector3(inVector.x, yAxis, inVector.z);
    }

    float shootTimer = 0;
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

                bool found = false;
                foreach (var plane in PlaneActor.PlaneList)
                {
                    if (plane.planeTeam != Plane.planeTeam)
                    {
                        direction = plane.transform.position - transform.position;
                        found = true;

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
                if (!found)
                {
                    if (!targetAirport)
                    {
                        targetAirport = AirportActor.GetClosestAirport(Plane.planeTeam, transform.position);
                    }
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

    int pointCount = 10;
    float pointSpacing = 120;
    float degree = 12;
    float safeAltitude = 200;
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
        // Si la cible est "devant" : applique une correction qui compense la gravité (applique un effet miroir sur la vitesse par rapport a la direction de la cible, et l'ajoute a la direction courrante)
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

    public Vector2[] Collectpoints()
    {
        Vector3[] points3D = GetNextPoints();
        Vector2[] points2D = new Vector2[points3D.Length];
        for (int i = 0; i < points3D.Length; ++i)
            points2D[i] = new Vector2(points3D[i].x, points3D[i].z);
        return points2D;
    }

    public void OnPointsProcessed(float[] processedPoints)
    {
        nextAltitudes = processedPoints;
    }
}
