using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// A simple missile that can be shoot from a weapon pod
/// </summary>
public class Missile : PodItem
{
    /// <summary>
    /// The thrust force of the engine of the missile
    /// </summary>
    public float ThrustPower = 100;

    /// <summary>
    /// The burst duration of the missile
    /// </summary>
    public float Endurance = 5;

    /// <summary>
    /// Thrust effect
    /// </summary>
    VisualEffect thrustFx;

    Vector3 LastTargetVelocity = Vector3.zero;



    private void FixedUpdate()
    {
        if (physics)
        {
            if (Endurance > 0)
            {
                // While the engine is working, add thrust acceleration
                Endurance -= Time.fixedDeltaTime;
                physics.velocity += transform.forward * ThrustPower * Time.fixedDeltaTime;
            }
            else if (thrustFx)
                thrustFx.Stop();

            if (!target)
                return;

            if (owner && Vector3.Distance(owner.transform.position, transform.position) < 10)
                return;

            // Aim algorithm

            // Rotate toward target
            Vector3 targetPosition = target.transform.position;

            float distanceToTarget = (targetPosition - transform.position).magnitude;
            float timeBeforeImpact = distanceToTarget / physics.velocity.magnitude;

            // Retrieve the target velocity if available
            Vector3 targetVelocity = Vector3.zero;
            var targetRb = target.GetComponent<Rigidbody>();
            if (targetRb) { targetVelocity = targetRb.velocity; }

            Vector3 targetAcceleration = ((targetRb.velocity - LastTargetVelocity) / Time.fixedDeltaTime);
            Vector3 relativeVelocity = transform.InverseTransformDirection(physics.velocity);
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;


            // Predict the target position at impact time. It use the velocity of the missile and the plane to compute the final impact location

            Vector3 Mp = transform.position; // Missile position
            Vector3 Mv = physics.velocity; // Missile velocity
            Vector3 Ap = target.transform.position; // Airplane position
            Vector3 Av = targetVelocity; // Airplane velocity
            // 1) On ajoute a la cible Ap sa vitesse Av, on trace une sphere de centre de centre Ap + Av de rayon |Mv| (vitesse missile)
            Vector3 CircleCenter = Ap + Av;
            float CircleRadius = Mv.magnitude;
            // 2) On cherche l'intersection entre la sphere et une droite entre le missile et la cible

            float intersecCenterDistFromPlane = Vector3.Dot(directionToTarget, Av);
            Vector3 intersectionCenter = Ap + directionToTarget * intersecCenterDistFromPlane;
            float intersectToCenterDist = (intersectionCenter - CircleCenter).magnitude;
            float intersectDist = Mathf.Sqrt(CircleRadius * CircleRadius - intersectToCenterDist * intersectToCenterDist);
            float interInnerDist = intersectDist - intersecCenterDistFromPlane;

            // 3) On calcule la trajectoire que doit prendre le missile
            float distanceRatio = (Ap - Mp).magnitude / interInnerDist;
            //Vector3 predictedTargetPos = Mp + Mv * distanceRatio;

            // We compute the time at impact with correction
            float correctTimeBeforeImpact = ((Mv * distanceRatio).magnitude / (Ap - Mp).magnitude) * timeBeforeImpact;

            // Update the acceleration of the target
            LastTargetVelocity = targetRb.velocity;

            // The final target position is a combination of prediction of the target movement depending on it's position, current velocity, current acceleration, and missile current lateral velocity
            Vector3 correctedTargetPosition = targetPosition + targetVelocity * correctTimeBeforeImpact + targetAcceleration * correctTimeBeforeImpact * 0.2f - transform.TransformDirection(new Vector3(relativeVelocity.x, relativeVelocity.y, 0)) * correctTimeBeforeImpact;

            // debug
            Debug.DrawLine(transform.position, targetPosition + targetVelocity * timeBeforeImpact, Color.red);
            Debug.DrawLine(transform.position, targetPosition + targetVelocity * correctTimeBeforeImpact, Color.yellow);

            // Make the missile look toward it's target
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((correctedTargetPosition - transform.position).normalized, new Vector3(0, 1, 0)), (40 / Mathf.Clamp((relativeVelocity.z / 300), 1, 4) * Time.fixedDeltaTime));
        }
    }


    public override void Shoot(GameObject objectOwner, Vector3 initialSpeed, GameObject target)
    {
        base.Shoot(objectOwner, initialSpeed, target);

        // When shoot, the missile activate it's rocket trail
        thrustFx = GetComponentInChildren<VisualEffect>();
        if (thrustFx)
        {
            thrustFx.initialEventName = "OnPlay";
            thrustFx.Play();
        }
    }
}
