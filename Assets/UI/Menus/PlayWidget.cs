using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayWidget : MonoBehaviour
{
    public GameObject gamemodeDropDown;
    public GameObject difficultyDropDown;
    public GameObject planeDropDown;
    public GameObject airportDropDown;

    public GameObject dayTimeSlider;

    private void OnEnable()
    {
        Dropdown airportDD = airportDropDown.GetComponent<Dropdown>();
        airportDD.options = new List<Dropdown.OptionData>();
        int value = 0;
        for (int i = 0; i < AirportActor.AirportList.Count; ++i)
        {
            if (AirportActor.AirportList[i] == GameplayManager.Singleton.NextSettings.Airport)
            {
                value = i;
            }
            airportDD.options.Add(new Dropdown.OptionData(AirportActor.AirportList[i].AirportName));
        }
        airportDD.value = value;



        Dropdown gamemodeDD = gamemodeDropDown.GetComponent<Dropdown>();
        gamemodeDD.options = new List<Dropdown.OptionData>();
        value = 0;
        for (int i = 0; i < GameplayManager.Singleton.AvailableGamemodes.Count; ++i)
        {
            if (GameplayManager.Singleton.AvailableGamemodes[i] == GameplayManager.Singleton.NextSettings.GamemodeInstance)
            {
                value = i;
            }
            gamemodeDD.options.Add(new Dropdown.OptionData(GameplayManager.Singleton.AvailableGamemodes[i].name));
        }
        gamemodeDD.value = value;


        Dropdown planeDD = planeDropDown.GetComponent<Dropdown>();
        planeDD.options = new List<Dropdown.OptionData>();
        value = 0;
        for (int i = 0; i < GameplayManager.Singleton.AvailableAirplanes.Count; ++i)
        {
            if (GameplayManager.Singleton.AvailableAirplanes[i] == GameplayManager.Singleton.NextSettings.PlaneModel)
            {
                value = i;
            }
            planeDD.options.Add(new Dropdown.OptionData(GameplayManager.Singleton.AvailableAirplanes[i].name));
        }
        planeDD.value = value;

        Dropdown difficultyDD = difficultyDropDown.GetComponent<Dropdown>();
        difficultyDD.options = new List<Dropdown.OptionData>();
        value = 0;
        var difficultyNames = System.Enum.GetNames(typeof(Difficulty));
        for (int i = 0; i < difficultyNames.Length; ++i)
        {
            if (System.Enum.GetName(typeof(Difficulty), GameplayManager.Singleton.NextSettings.Difficulty) == difficultyNames[i])
                value = i;
            difficultyDD.options.Add(new Dropdown.OptionData(difficultyNames[i]));
        }
        difficultyDD.value = value;

        dayTimeSlider.GetComponent<Slider>().value = GameplayManager.Singleton.NextSettings.DayTime / 360;
    }


    public void Restart()
    {
        GameplayManager.Singleton.NextSettings.Airport = AirportActor.AirportList[airportDropDown.GetComponent<Dropdown>().value];
        GameplayManager.Singleton.NextSettings.GamemodeInstance = GameplayManager.Singleton.AvailableGamemodes[gamemodeDropDown.GetComponent<Dropdown>().value];
        GameplayManager.Singleton.NextSettings.PlaneModel = GameplayManager.Singleton.AvailableAirplanes[planeDropDown.GetComponent<Dropdown>().value];
        GameplayManager.Singleton.NextSettings.DayTime = dayTimeSlider.GetComponent<Slider>().value * 360;
        int diffValue = difficultyDropDown.GetComponent<Dropdown>().value;
        GameplayManager.Singleton.NextSettings.Difficulty = diffValue == 0 ? Difficulty.Casual : diffValue == 1 ? Difficulty.Advanced : Difficulty.Realistic;

        GetComponentInParent<MenuLayoutManager>().BlackScreen();


        GameplayManager.Singleton.StartGame();
    }
}
