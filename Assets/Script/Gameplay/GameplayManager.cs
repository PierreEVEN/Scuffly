using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.Rendering.HighDefinition.VolumetricClouds;

public enum Weather
{
    Sunny,
    Overcast,
    Stormy
}
public enum Difficulty
{
    Casual,
    Advanced,
    Realistic
}

public struct GameSettings
{
    public GameObject GamemodeInstance;
    public GameObject PlaneModel;
    public float DayTime;
    public Weather Weather;
    public Difficulty Difficulty;
    public AirportActor Airport;
}

public class GameplayManager : MonoBehaviour
{
    public GameObject PauseUIObject;
    GameObject spawnedPauseUIObject;

    GameObject gamemode;

    private static GameplayManager _singleton;
    public static GameplayManager Singleton
    {
        get { return _singleton; }
    }

    private void OnEnable()
    {
        _singleton = this;
    }

    private void Start()
    {
        Menu = true;

        NextSettings.GamemodeInstance = AvailableGamemodes[0];
        NextSettings.PlaneModel = AvailableAirplanes[0];
        NextSettings.DayTime = 90;
        NextSettings.Airport = GetAvailableAirports()[0];
        NextSettings.Weather = Weather.Overcast;
        NextSettings.Difficulty = Difficulty.Advanced;
    }

    public AirportActor[] GetAvailableAirports()
    {
        return AirportActor.AirportList.ToArray();
    }

    public List<GameObject> AvailableAirplanes = new List<GameObject>();
    public List<GameObject> AvailableGamemodes = new List<GameObject>();

    private void OnDisable()
    {
        _singleton = null;
    }

    public GameSettings CurrentSettings;
    public GameSettings NextSettings;

    public void StartGame()
    {
        ClearGame();
        CurrentSettings = NextSettings;


        switch (CurrentSettings.Weather)
        {
            case Weather.Sunny:
                OptionWidget.GetVolumeComponent<VolumetricClouds>().cloudPreset = new CloudPresetsParameter(CloudPresets.Sparse);
                break;
            case Weather.Overcast:
                OptionWidget.GetVolumeComponent<VolumetricClouds>().cloudPreset = new CloudPresetsParameter(CloudPresets.Overcast);
                break;
            case Weather.Stormy:
                OptionWidget.GetVolumeComponent<VolumetricClouds>().cloudPreset = new CloudPresetsParameter(CloudPresets.Stormy);
                break;
        }

        GameObject.Find("Lighting").GetComponent<DayNightCycle>().SetRotation(CurrentSettings.DayTime);

        Menu = false;

        if (CurrentSettings.GamemodeInstance)
            gamemode = GameObject.Instantiate(CurrentSettings.GamemodeInstance);
        else
        {
            Debug.LogWarning("no gamemode selected");
            Menu = true;
        }
    }

    void ClearGame()
    {
        if (gamemode)
            GameObject.Destroy(gamemode);

        for (int i = PlaneActor.PlaneList.Count - 1; i >= 0; --i)
        {
            GameObject.Destroy(PlaneActor.PlaneList[i].gameObject);
        }
    }

    bool isInMenu = false;
    public bool Menu
    {
        get
        {
            return isInMenu;
        }
        set
        {
            if (isInMenu != value)
            {
                isInMenu = value;

                if (PlayerManager.LocalPlayer)
                    PlayerManager.LocalPlayer.disableInputs = GameplayManager.Singleton.Menu;
                if (isInMenu)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    if (PauseUIObject && !spawnedPauseUIObject)
                        spawnedPauseUIObject = GameObject.Instantiate(PauseUIObject);

                    if (spawnedPauseUIObject && spawnedPauseUIObject.GetComponent<MenuLayoutManager>())
                        spawnedPauseUIObject.GetComponent<MenuLayoutManager>().Open(true);
                }
                else
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;

                    if (spawnedPauseUIObject && spawnedPauseUIObject.GetComponent<MenuLayoutManager>())
                        spawnedPauseUIObject.GetComponent<MenuLayoutManager>().Open(false);
                }
            }
        }
    }
}
