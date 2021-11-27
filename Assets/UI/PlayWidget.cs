using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayWidget : MonoBehaviour
{
    public void Restart()
    {
        for (int i = PlaneActor.PlaneList.Count - 1; i >= 0; --i)
        {
            Destroy(PlaneActor.PlaneList[i].gameObject);
        }

        PlayerManager.LocalPlayer.RequestPlaneServerRpc();
        PlayerManager.LocalPlayer.GetComponent<UiInputs>().OnPause();
    }

    public void SpawnRed()
    {
        AirportActor foundAirport = AirportActor.GetClosestAirport(PlaneTeam.Red, new Vector3(0, 0, 0));
        if (!foundAirport)
            return;

        foreach (var spawnpoint in foundAirport.GatherSpawnpoints())
        {
            if (spawnpoint.useForAI)
            {
                spawnpoint.SpawnPlane(true, 0);
            }
        }
    }
    public void SpawnBlue()
    {
        AirportActor foundAirport = AirportActor.GetClosestAirport(PlaneTeam.Blue, new Vector3(0, 0, 0));
        if (!foundAirport)
            return;

        foreach (var spawnpoint in foundAirport.GatherSpawnpoints())
        {
            if (spawnpoint.useForAI)
            {
                spawnpoint.SpawnPlane(true, 0);
            }
        }
    }
}
