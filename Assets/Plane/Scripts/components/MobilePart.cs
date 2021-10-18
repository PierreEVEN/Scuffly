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

    // Start is called before the first frame update
    void Start()
    {
        intNeutralRotation.eulerAngles = neutralRotation;
        intMinRotation.eulerAngles = minRotation;
        intMaxRotation.eulerAngles = maxRotation;
        setInput(0);
    }

    public void setInput(float inputValue)
    {
        Quaternion finalRotation;

        inputValue = Mathf.Clamp(inputValue, -1, 1);

        if (inputValue > 0.0f)
            finalRotation = Quaternion.Lerp(intNeutralRotation, intMaxRotation, inputValue);
        else
            finalRotation = Quaternion.Lerp(intNeutralRotation, intMinRotation, -inputValue);

        gameObject.transform.rotation = gameObject.transform.parent.rotation * finalRotation;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
