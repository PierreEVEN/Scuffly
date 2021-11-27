using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLayoutManager : MonoBehaviour
{
    public GameObject OptionWidget;
    public GameObject PlayWidget;
    public GameObject AreYouSureToExitWidget;
    public GameObject AreYouSureToGoToMenuWidget;

    public GameObject WidgetContainer;

    public bool isInLobby = true;

    public void SetDisplayedWidget(GameObject widgetClass)
    {
        for (int i = 0; i < WidgetContainer.transform.childCount; ++i)
        {
            Destroy(WidgetContainer.transform.GetChild(i).gameObject);
        }
        if (widgetClass)
            Instantiate(widgetClass, WidgetContainer.transform);
    }

    public void PressHomeButton()
    {
        if (isInLobby)
        {
            SetDisplayedWidget(null);
        }
        else
        {
            SetDisplayedWidget(AreYouSureToGoToMenuWidget);
        }
    }

    public void PressOptionButton()
    {
        SetDisplayedWidget(OptionWidget);
    }

    public void PressPlayButton()
    {
        SetDisplayedWidget(PlayWidget);
    }

    public void PressBackButton()
    {
        SetDisplayedWidget(null);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
