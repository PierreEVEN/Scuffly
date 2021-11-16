using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 *  @Author : Pierre EVEN
 *  
 *  Une partie mobile correspond par exemple aux gouvernes d'un avion. Utilisable pour n'importe quelle composant lie aux gouvernes.
 *  
 *  @TODO rendre le fonctionnement des parties mobiles dependantes a l'etat des systemes hydrolique de l'avion
 */
public class MobilePart : PlaneComponent
{
    // Note : les rotations sont parametrees en angle d'euler pour simplifier le parametrage

    // Rotation en position zero
    public Vector3 neutralRotation = new Vector3(0, 0, 0);
    // Rotation en position negative maximale
    public Vector3 minRotation = new Vector3(0, 0, 0);
    // Rotation en position positive maximale
    public Vector3 maxRotation = new Vector3(0, 0, 0);

    // Rotations en Quaternions
    private Quaternion intNeutralRotation = Quaternion.identity;
    private Quaternion intMinRotation = Quaternion.identity;
    private Quaternion intMaxRotation = Quaternion.identity;

    // La rotation est progressive pour simuler le temps de latence des systemes hydrauliques et electroniques.
    private float desiredInput = 0;
    private float inputValue = 0;
    private float InterpolationSpeed = 2;

    public bool RequirePlanePower = true;

    // Start is called before the first frame update
    void Start()
    {
        intNeutralRotation.eulerAngles = neutralRotation;
        intMinRotation.eulerAngles = minRotation;
        intMaxRotation.eulerAngles = maxRotation;
        setInput(0);
    }

    public void setInput(float input)
    {
        desiredInput = Mathf.Clamp(input, -1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        // Necessite de la puissance pour fonctionner
        if (RequirePlanePower && Plane.GetCurrentPower() < 95)
            return;

        // Deplace la partie mobile vers la position desiree
        inputValue += Mathf.Clamp(desiredInput - inputValue, -Time.deltaTime * InterpolationSpeed, Time.deltaTime * InterpolationSpeed);
        Quaternion finalRotation;

        // Applique la rotation
        if (inputValue > 0.0f)
            finalRotation = Quaternion.Lerp(intNeutralRotation, intMaxRotation, inputValue);
        else
            finalRotation = Quaternion.Lerp(intNeutralRotation, intMinRotation, -inputValue);
        if (gameObject.transform.parent)
            gameObject.transform.rotation = gameObject.transform.parent.rotation * finalRotation;
    }
}
