using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A gamemode where you will fight against waves of enemies
/// </summary>
public class GamemodePVE : MonoBehaviour
{
    /// <summary>
    /// The plane spawned for the player
    /// </summary>
    GameObject PlayerPlane;

    /// <summary>
    /// The type of plane used by the ennemies
    /// </summary>
    public GameObject EnnemyPlane;

    /// <summary>
    /// The gamemode UI class
    /// </summary>
    public GameObject GamemodeUI;
    GameObject GamemodeUIInstance;

    /// <summary>
    /// A list of spawned enemy planes
    /// </summary>
    List<GameObject> Ennemies = new List<GameObject>();

    // Spawn and respawn delay
    public float EnnemiesMinSpawnDelay = 180;
    public float EnnemiesSpawnDelay = 60;

    /// <summary>
    /// has player spawned
    /// </summary>
    bool Spawned = false;

    public UnityEvent OnKill = new UnityEvent();
    public UnityEvent OnDefeat = new UnityEvent();

    /// <summary>
    /// has end been kalled
    /// </summary>
    public bool calledEnd = false;
    public int killCount = 0;
    public float timeSurvived = 0;

    int enemyTagNumber = 0;

    private void OnEnable()
    {
        if (!GamemodeUIInstance)
            GamemodeUIInstance = Instantiate(GamemodeUI);
        GamemodeUIInstance.GetComponent<PVEWidget>().Setup(this);
    }

    private void OnDisable()
    {
        if (GamemodeUIInstance)
            Destroy(GamemodeUIInstance);
    }

    void Update()
    {
        timeSurvived += Time.deltaTime;

        // Don't spawn enemies while the player is stopped on ground
        if (PlayerPlane && PlayerPlane.GetComponent<Rigidbody>().velocity.magnitude > 20)
            EnnemiesSpawnDelay -= Time.deltaTime;

        // Spawn the next waves if the previous one was destroyed
        if (EnnemiesSpawnDelay <= 0 && Ennemies.Count < (GameplayManager.Singleton.CurrentSettings.Difficulty == Difficulty.Casual ? 1 : 2))
        {
            SpawnEnnemy();
            EnnemiesSpawnDelay = EnnemiesMinSpawnDelay * Random.Range(0.8f, 1.25f);
        }

        // Spawn the plane of the player
        if (!PlayerPlane && !Spawned)
            SpawnPlayerPlane();


        // Detect player plane destruction
        //@TODO use event instead
        if (!PlayerPlane && Spawned)
        {
            if (!calledEnd)
            {
                calledEnd = true;
                OnDefeat.Invoke();
            }
            GameplayManager.Singleton.Menu = true;
        }
    }

    /// <summary>
    /// Spawn the plane of the player
    /// </summary>
    void SpawnPlayerPlane()
    {
        AirportActor Airport = GameplayManager.Singleton.CurrentSettings.Airport;
        GameObject PlaneModel = GameplayManager.Singleton.CurrentSettings.PlaneModel;

        if (!PlaneModel || !Airport)
        {
            Debug.LogError("cannot spawn plane");
            return;
        }

        foreach (var spawnPoint in Airport.GatherSpawnpoints())
        {
            PlayerPlane = GameObject.Instantiate(PlaneModel, spawnPoint.transform.position, spawnPoint.transform.rotation);
            if (!PlayerPlane)
            {
                GameManager.Singleton.GoToMenu();
                return;
            }
            PlayerManager.LocalPlayer.PossessPlane(PlayerPlane.GetComponent<PlaneActor>());

            PlayerPlane.GetComponent<PlaneActor>().OnDestroyed.AddListener(OnPlaneDestroyed);
            Spawned = true;

            break;
        }
    }

    /// <summary>
    /// Event called when a plane is destroyed
    /// </summary>
    /// <param name="destroyedPlane"></param>
    void OnPlaneDestroyed(PlaneActor destroyedPlane)
    {
        // Remove the plane from the enemy list
        if (Ennemies.Contains(destroyedPlane.gameObject))
            Ennemies.Remove(destroyedPlane.gameObject);

        // Add to kill count if the instigator for the kill is the player
        if (destroyedPlane.LastDamageInstigator == PlayerPlane)
        {
            killCount++;
            OnKill.Invoke();
        }

        Debug.Log(destroyedPlane.gameObject.name + " was destroyed. Damage instigator is " + (destroyedPlane.LastDamageInstigator ? destroyedPlane.LastDamageInstigator.name : " undefined"));
    }

    /// <summary>
    /// Spawn an enemy in a random circle around the player
    /// </summary>
    void SpawnEnnemy()
    {
        int num = (int)Random.Range(1, GameplayManager.Singleton.CurrentSettings.Difficulty == Difficulty.Casual ? 1 : 3);
        float spawnDist = Random.Range(15000.0f, 30000.0f);
        Vector3 Spawnoffset = new Vector3(PlayerPlane.transform.position.x, Random.Range(2000.0f, 5000.0f), PlayerPlane.transform.position.z);
        Vector3 SpawnDir = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)).normalized;
        Vector3 SpawnPosition = Spawnoffset + spawnDist * SpawnDir;

        for (int i = 0; i < num; ++i)
        {
            Debug.Log("spawn ennemy " + spawnDist + " / " + Spawnoffset + " / " + SpawnDir);
            GameObject spawnedPlane = Instantiate(EnnemyPlane);
            spawnedPlane.name = "F-16 red - " + enemyTagNumber++;
            PlaneActor plane = spawnedPlane.GetComponent<PlaneActor>();
            spawnedPlane.transform.position = SpawnPosition + new Vector3(100, 0, 100) * i;
            plane.planeTeam = PlaneTeam.Red;

            // Add an AI controlled to the spawned plane
            if (!spawnedPlane.GetComponent<PlaneAIController>())
                spawnedPlane.AddComponent<PlaneAIController>();
            Ennemies.Add(spawnedPlane);
            plane.OnDestroyed.AddListener(OnPlaneDestroyed);
        }
    }
}
