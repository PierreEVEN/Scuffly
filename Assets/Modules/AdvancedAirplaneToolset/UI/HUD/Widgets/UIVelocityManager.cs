
using UnityEngine;
using UnityEngine.UI;

//@TODO fix and improve velocity widget
[RequireComponent(typeof(Text))]
public class UIVelocityManager : HUDComponent
{
    Text text;
    GameObject container;
    GameObject containerElem;
    // Start is called before the first frame update
    void OnEnable()
    {
        text = GetComponent<Text>();

        container = GetComponentInChildren<RectMask2D>().gameObject;
        containerElem = new GameObject("containerChildren");
        containerElem.transform.parent = container.transform;
        containerElem.transform.localPosition = new Vector3(0, 0, 0);
        containerElem.transform.localScale = new Vector3(1, 1, 1);
        containerElem.hideFlags = HideFlags.DontSave;
        for (int i = 0; i < 1000; ++i)
        {
            GameObject child = new GameObject("containerChildren22");
            child.transform.parent = containerElem.transform;
            child.hideFlags = HideFlags.DontSave;
            child.transform.localPosition = new Vector3(i % 5 == 0  ? 25 : 30, i * 10, 0);
            child.transform.localScale = new Vector3(i % 5 == 0 ? 0.2f : 0.15f, 0.02f, 1);
            child.AddComponent<Image>();
            if (i % 10 == 0)
            {
                GameObject child2 = new GameObject("containerChildren223");
                child2.transform.parent = containerElem.transform;
                child2.hideFlags = HideFlags.DontSave;
                child2.transform.localPosition = new Vector3(-15, i * 10, 0);
                child2.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                Text txt = child2.AddComponent<Text>();
                txt.text = i.ToString();
                txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                txt.fontSize = 70;
                txt.alignment = TextAnchor.MiddleRight;

            }
        }


    }

    private void OnDisable()
    {
        DestroyImmediate(containerElem);
    }

    // Update is called once per frame
    void Update()
    {
        text.text = string.Format("{0}", (int)Plane.GetSpeedInNautics());

        containerElem.transform.localPosition = new Vector3(0, -Plane.GetSpeedInNautics() * 10, 0); ;

    }
}
