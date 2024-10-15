using System.Collections.Generic;
using UnityEngine;
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

/// <summary>
/// Handle differnts gamemodes, difficulty, plane, airport aso... all the global game parameters
/// </summary>
public class GameplayManager : MonoBehaviour
{
    /// <summary>
    /// The UI spawned when pausing
    /// </summary>
    public GameObject PauseUIObject;
    GameObject spawnedPauseUIObject;

    // The current enabled gamemode
    GameObject gamemode;

    bool isInMenu = false;

    private static GameplayManager _singleton;
    public static GameplayManager Singleton { get { return _singleton; } }

    private void OnEnable() => _singleton = this;
    private void OnDisable() => _singleton = null;


    public AK.Wwise.Event PlayMenuMusic;
    public AK.Wwise.Event StopMenuMusic;
    public AK.Wwise.RTPC MenuMusicLevel;
    float AudioLevel = 0;
    bool isMenuMusicPlaying = false;

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

    /// <summary>
    /// A list of enabled planes and mode
    /// </summary>
    public List<GameObject> AvailableAirplanes = new List<GameObject>();
    public List<GameObject> AvailableGamemodes = new List<GameObject>();

    /// <summary>
    /// The game config, and the next one that will be used when starting the next game
    /// </summary>
    public GameSettings CurrentSettings;
    public GameSettings NextSettings;

    private void Update()
    {
        float desiredLevel = PlayerManager.LocalPlayer.controlledPlane ? 0 : 100;
        AudioLevel = AudioLevel + Mathf.Clamp(desiredLevel - AudioLevel, -Time.deltaTime * 50, Time.deltaTime * 50);

        if (isMenuMusicPlaying && AudioLevel < 1)
        {
            isMenuMusicPlaying = false;
            StopMenuMusic.Post(gameObject);
        }
        if (!isMenuMusicPlaying && AudioLevel > 1)
        {
            isMenuMusicPlaying = true;
            PlayMenuMusic.Post(gameObject);
        }
        MenuMusicLevel.SetGlobalValue(AudioLevel);
    }

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

    /// <summary>
    /// Stop the current game, clear planes and gamemode
    /// </summary>
    void ClearGame()
    {
        if (gamemode)
            GameObject.Destroy(gamemode);

        for (int i = PlaneActor.PlaneList.Count - 1; i >= 0; --i)
        {
            GameObject.Destroy(PlaneActor.PlaneList[i].gameObject);
        }
    }

    /// <summary>
    /// Menu handling : make the UI appear if paused and vice versa
    /// </summary>
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

                // Disable play inputs
                if (PlayerManager.LocalPlayer)
                    PlayerManager.LocalPlayer.disableInputs = GameplayManager.Singleton.Menu;

                if (isInMenu)
                {
                    // Hide the cursor
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    if (PauseUIObject && !spawnedPauseUIObject)
                        spawnedPauseUIObject = GameObject.Instantiate(PauseUIObject);

                    // Open menu widget
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
