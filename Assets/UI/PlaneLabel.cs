using UnityEngine;
using UnityEngine.UI;

public class PlaneLabel : MonoBehaviour
{
    [HideInInspector]
    public GameObject target;

    public GameObject textObject;

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            Vector3 ScreenPos = Camera.main.WorldToScreenPoint(target.transform.position);
            transform.localPosition = new Vector3(ScreenPos.x, ScreenPos.y, ScreenPos.z < 0 ? -100000 : 0);
            textObject.GetComponent<Text>().text = target.name;
        }
    }
}
