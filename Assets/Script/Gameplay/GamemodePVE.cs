using System.Collections.Generic;
using UnityEngine;

public class GamemodePVE : MonoBehaviour
{
    GameObject PlayerPlane;

    public GameObject EnnemyPlane;

    List<GameObject> Ennemies = new List<GameObject>();

    public float EnnemiesMinSpawnDelay = 180;

    public float EnnemiesSpawnDelay = 60;

    bool Spawned = false;

    int killCount = 0;

    void Update()
    {
        EnnemiesSpawnDelay -= Time.deltaTime;
        if (EnnemiesSpawnDelay <= 0 && Ennemies.Count < (GameplayManager.Singleton.CurrentSettings.Difficulty == Difficulty.Casual ? 1 : 2))
        {
            SpawnEnnemy();
            EnnemiesSpawnDelay = EnnemiesMinSpawnDelay;
        }

        if (!PlayerPlane && !Spawned)
            SpawnPlayerPlane();

        if (!PlayerPlane && Spawned)
        {
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
            Debug.Log("et paf !");
        }

        Debug.Log(destroyedPlane.gameObject.name + " was destroyed. Damage instigator is " + (destroyedPlane.LastDamageInstigator ? destroyedPlane.LastDamageInstigator.name : " undefined"));
    }

    int enemyNumber = 0;

    void SpawnEnnemy()
    {
        int num = (int)Random.Range(1, GameplayManager.Singleton.CurrentSettings.Difficulty == Difficulty.Casual ? 1 : 3);
        Vector3 SpawnPosition = new Vector3(PlayerPlane.transform.position.x, Random.Range(2000, 5000), PlayerPlane.transform.position.z) + Random.Range(25000, 50000) * new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)).normalized;

        for (int i = 0; i < num; ++i)
        {
            Debug.Log("spawn ennemy");
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
