using UnityEngine;

public class GamemodeFreeFlight : MonoBehaviour
{
    GameObject PlayerPlane;

    bool Spawned = false;

    void Update()
    {
        if (!PlayerPlane && !Spawned)
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
                Spawned = true;

                break;
            }

            if (!Spawned)
                Debug.LogError("failed to spawn plane");
        }

        // Plane was destroyed
        if (!PlayerPlane && Spawned)
        {
            Destroy(gameObject);
            GameplayManager.Singleton.Menu = true;
        }
    }
}
