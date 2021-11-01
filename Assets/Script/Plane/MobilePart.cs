using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 *  @Author : Pierre EVEN
 */
public class MobilePart : MonoBehaviour
{
    public Vector3 neutralRotation = new Vector3(0, 0, 0);
    public Vector3 minRotation = new Vector3(0, 0, 0);
    public Vector3 maxRotation = new Vector3(0, 0, 0);

    private Quaternion intNeutralRotation = Quaternion.identity;
    private Quaternion intMinRotation = Quaternion.identity;
    private Quaternion intMaxRotation = Quaternion.identity;
    private float desiredInput = 0;
    private float inputValue = 0;

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

        inputValue += Mathf.Clamp(desiredInput - inputValue, -Time.deltaTime * 2, Time.deltaTime * 2);
        Quaternion finalRotation;

        if (inputValue > 0.0f)
            finalRotation = Quaternion.Lerp(intNeutralRotation, intMaxRotation, inputValue);
        else
            finalRotation = Quaternion.Lerp(intNeutralRotation, intMinRotation, -inputValue);

        gameObject.transform.rotation = gameObject.transform.parent.rotation * finalRotation;
    }
}
