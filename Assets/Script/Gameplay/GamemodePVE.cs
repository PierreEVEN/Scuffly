using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GamemodePVE : MonoBehaviour
{
    GameObject PlayerPlane;

    public GameObject EnnemyPlane;
    public GameObject GamemodeUI;
    GameObject GamemodeUIInstance;

    List<GameObject> Ennemies = new List<GameObject>();

    public float EnnemiesMinSpawnDelay = 180;
    public float EnnemiesSpawnDelay = 60;
    bool Spawned = false;

    public UnityEvent OnKill = new UnityEvent();
    public UnityEvent OnLost = new UnityEvent();

    public bool calledEnd = false;
    public int killCount = 0;
    public float timeSurvived = 0;

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

        if (PlayerPlane && PlayerPlane.GetComponent<Rigidbody>().velocity.magnitude > 20)
            EnnemiesSpawnDelay -= Time.deltaTime;
        if (EnnemiesSpawnDelay <= 0 && Ennemies.Count < (GameplayManager.Singleton.CurrentSettings.Difficulty == Difficulty.Casual ? 1 : 2))
        {
            SpawnEnnemy();
            EnnemiesSpawnDelay = EnnemiesMinSpawnDelay * Random.Range(0.8f, 1.25f);
        }

        if (!PlayerPlane && !Spawned)
            SpawnPlayerPlane();

        if (!PlayerPlane && Spawned)
        {
            if (!calledEnd)
            {
                calledEnd = true;
                OnLost.Invoke();
            }
            GameplayManager.Singleton.Menu = true;
        }
    }

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

    void OnPlaneDestroyed(PlaneActor destroyedPlane)
    {
        if (Ennemies.Contains(destroyedPlane.gameObject))
            Ennemies.Remove(destroyedPlane.gameObject);

        if (destroyedPlane.LastDamageInstigator == PlayerPlane)
        {
            killCount++;
            OnKill.Invoke();
        }

        Debug.Log(destroyedPlane.gameObject.name + " was destroyed. Damage instigator is " + (destroyedPlane.LastDamageInstigator ? destroyedPlane.LastDamageInstigator.name : " undefined"));
    }

    int enemyNumber = 0;

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
            spawnedPlane.name = "F-16 red - " + enemyNumber++;
            PlaneActor plane = spawnedPlane.GetComponent<PlaneActor>();
            spawnedPlane.transform.position = SpawnPosition + new Vector3(100, 0, 100) * i;
            plane.planeTeam = PlaneTeam.Red;
            if (!spawnedPlane.GetComponent<PlaneAIController>())
                spawnedPlane.AddComponent<PlaneAIController>();
            Ennemies.Add(spawnedPlane);
            plane.OnDestroyed.AddListener(OnPlaneDestroyed);
        }
    }
}
