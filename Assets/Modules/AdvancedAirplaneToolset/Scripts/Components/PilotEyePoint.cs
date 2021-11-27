using UnityEngine;

// Point de vue du pilot / emplacement que prend la camera en vue à la premiere personne
[RequireComponent(typeof(PhysicsFeedbackAtPoint))]
public class PilotEyePoint : MonoBehaviour
{    
    PhysicsFeedbackAtPoint physics;
    float GOffsetIntensity = 0.0004f;
    float GForceValue = 0;


    // Start is called before the first frame update
    void OnEnable()
    {
        physics = GetComponent<PhysicsFeedbackAtPoint>();
    }

    // Calcule la position de la camera en appliquant un offset en fonction de l'acceleration que subit le pilote pour un effet sympas :)
    public Vector3 GetCameraLocation()
    {
        return transform.position +
            (transform.right * physics.Acceleration.x * 4.5f + transform.up * physics.Acceleration.y * 1.5f + transform.forward * physics.Acceleration.z * 0.1f) 
            * -1 * GOffsetIntensity;
    }

    private void Update()
    {
        // En fonction de la force de G perçue par le pilote, met a jour la variable GForceValue indiquant le niveau de voil sur les yeux du pilote (positif ou negatif)
        float inputForce = physics.GForce.y / 2.5f; // Resistance du pilote. Plus le coefficient est grand, moins il est soumis aux facteurs de charges
        float newForce = Mathf.Max(0,Mathf.Abs(inputForce) - 1);
        GForceValue = GForceValue + Mathf.Clamp(newForce * Mathf.Sign(inputForce) - GForceValue, -Time.deltaTime * 0.2f, Time.deltaTime * 0.2f); // Facteur de charge lissé
        if (newForce > Mathf.Abs(GForceValue))
            GForceValue = newForce * Mathf.Sign(inputForce);
        if (Mathf.Abs(GForceValue) > 1 && Mathf.Abs(GForceValue) <= 1)
            GForceValue *= 2;
    }

    public float GetGforceEffect()
    {
        return GForceValue;
    }

}
