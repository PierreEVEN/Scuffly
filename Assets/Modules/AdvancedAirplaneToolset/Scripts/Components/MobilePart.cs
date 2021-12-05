using UnityEngine;

public enum MobilePartBinding
{
    Custom,
    Roll,
    Pitch,
    Yaw,
    Thrust,
    Canopy
}

/**
 *  @Author : Pierre EVEN
 *  
 *  Une partie mobile correspond par exemple aux gouvernes d'un avion. Utilisable pour n'importe quelle composant lie aux gouvernes.
 *  
 *  @TODO rendre le fonctionnement des parties mobiles dependantes a l'etat des systemes hydraulique de l'avion
 */
[ExecuteInEditMode]
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
    [Range(-1, 1)]
    public float desiredInput = 0; // Valeur par defaut au demarrage
    private float currentInput = 0;
    public float InterpolationSpeed = 2;

    // Assignation automatique de la valeur a une composante de l'avion
    public MobilePartBinding binding = MobilePartBinding.Custom;

    // Est ce que le mouvement necessite de l'energie
    public bool RequirePlanePower = true;

    // Start is called before the first frame update
    void Start()
    {
        intNeutralRotation.eulerAngles = neutralRotation;
        intMinRotation.eulerAngles = minRotation;
        intMaxRotation.eulerAngles = maxRotation;

        if (Application.isPlaying && binding == MobilePartBinding.Custom)
            desiredInput = 0;

        currentInput = desiredInput;
    }

    // Met a jour l'input manuellement si le mode "Custom" est actif
    public void setInput(float input)
    {
        desiredInput = Mathf.Clamp(input, -1, 1);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            intNeutralRotation.eulerAngles = neutralRotation;
            intMinRotation.eulerAngles = minRotation;
            intMaxRotation.eulerAngles = maxRotation;
        }
#endif

        // Verifie si on a assez de puissance pour fonctionner
        if (Plane && (!RequirePlanePower || Plane.GetCurrentPower() > 95))
        {
            // Recupere la valeur
            switch (binding)
            {
                case MobilePartBinding.Custom:
                    break;
                case MobilePartBinding.Roll:
                    desiredInput = Plane.RollInput;
                    break;
                case MobilePartBinding.Pitch:
                    desiredInput = Plane.PitchInput;
                    break;
                case MobilePartBinding.Yaw:
                    desiredInput = Plane.YawInput;
                    break;
                case MobilePartBinding.Thrust:
                    desiredInput = Plane.ThrottleNotch ? Plane.ThrustInput : 0;
                    break;
                case MobilePartBinding.Canopy:
                    desiredInput = Plane.OpenCanopy ? 1 : 0;
                    break;
            }
        }

        SetRotationValue(desiredInput);
    }

    // Applique la rotation en fonction d'une valeur entre 0 et 1
    void SetRotationValue(float value)
    {
        // Deplace la partie mobile vers la position desiree de façon lissee
        float delta = Time.deltaTime * InterpolationSpeed;
        currentInput = Mathf.Clamp(currentInput + Mathf.Clamp(value - currentInput, -delta, delta), -1, 1);

        Quaternion finalRotation = Quaternion.identity;

        // Applique la rotation UP ou DOWN selon la valeur de currentInput
        if (currentInput > 0.0f)
            finalRotation = Quaternion.Lerp(intNeutralRotation, intMaxRotation, currentInput);
        else
            finalRotation = Quaternion.Lerp(intNeutralRotation, intMinRotation, -currentInput);

        transform.localRotation = finalRotation;
    }
}
