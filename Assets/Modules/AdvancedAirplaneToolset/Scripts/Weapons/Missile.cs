using UnityEngine;
using UnityEngine.VFX;

/*
 * @Autor : Leo
 * 
 * Armement de type missile attachable a un pod d'armement
 * 
 */

public class Missile : PodItem
{
    public float ThrustPower = 100;
    public float Endurance = 5;

    VisualEffect thrustFx;

    Vector3 LastTargetVelocity = Vector3.zero;

    private void FixedUpdate()
    {
        if (physics)
        {
            if (Endurance > 0)
            {
                Endurance -= Time.fixedDeltaTime;
                physics.velocity += transform.forward * ThrustPower * Time.fixedDeltaTime;
            }
            else if (thrustFx)
                thrustFx.Stop();

            if (!target)
                return;

            if (owner && Vector3.Distance(owner.transform.position, transform.position) < 10)
                return;

            // Rotate toward target
            Vector3 targetPosition = target.transform.position;

            float distanceToTarget = (targetPosition - transform.position).magnitude;
            float timeBeforeImpact = distanceToTarget / physics.velocity.magnitude;

            Vector3 targetVelocity = Vector3.zero;
            var targetRb = target.GetComponent<Rigidbody>();
            if (targetRb)
            {
                targetVelocity = targetRb.velocity;
            }

            Vector3 targetAcceleration = ((targetRb.velocity - LastTargetVelocity) / Time.fixedDeltaTime);

            LastTargetVelocity = targetRb.velocity;

            Vector3 correctedTargetPosition = targetPosition + targetVelocity * timeBeforeImpact + targetAcceleration * timeBeforeImpact;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((correctedTargetPosition - transform.position).normalized, new Vector3(0, 1, 0)), 50 * Time.fixedDeltaTime);
        }
    }

    public override void Shoot(GameObject objectOwner, Vector3 initialSpeed, Vector3 upVector, GameObject target)
    {
        base.Shoot(objectOwner, initialSpeed, upVector, target);

        thrustFx = GetComponentInChildren<VisualEffect>();
        if (thrustFx)
        {
            thrustFx.initialEventName = "OnPlay";
            thrustFx.Play();
        }
    }
}
