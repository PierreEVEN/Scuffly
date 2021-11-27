using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayWidget : MonoBehaviour
{
    public GameObject RedTeamPlanes;
    public GameObject BlueTeamPlanes;

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
        int redPlane = 0;
        foreach (var plane in PlaneActor.PlaneList)
        {
            if (plane.planeTeam == PlaneTeam.Red)
                redPlane++;
        }

        if (redPlane >= 3)
            return;

        AirportActor foundAirport = AirportActor.GetClosestAirport(PlaneTeam.Red, new Vector3(0, 0, 0));
        if (!foundAirport)
            return;

        foreach (var spawnpoint in foundAirport.GatherSpawnpoints())
        {
            if (spawnpoint.useForAI)
            {
                GameObject spawnedPlane = spawnpoint.SpawnPlane(true, 0);
                spawnedPlane.GetComponent<PlaneActor>().planeTeam = PlaneTeam.Red;
            }
        }
    }
    public void SpawnBlue()
    {
        int bluePlanes = 0;
        foreach (var plane in PlaneActor.PlaneList)
        {
            if (plane.planeTeam == PlaneTeam.Blue)
                bluePlanes++;
        }

        if (bluePlanes >= 3)
            return;

        AirportActor foundAirport = AirportActor.GetClosestAirport(PlaneTeam.Blue, new Vector3(0, 0, 0));
        if (!foundAirport)
            return;

        foreach (var spawnpoint in foundAirport.GatherSpawnpoints())
        {
            if (spawnpoint.useForAI)
            {
                GameObject spawnedPlane = spawnpoint.SpawnPlane(true, 0);
                spawnedPlane.GetComponent<PlaneActor>().planeTeam = PlaneTeam.Blue;
            }
        }
    }

    private void Update()
    {
        int bluePlanes = 0;
        int redPlanes = 0;
        foreach (var plane in PlaneActor.PlaneList)
        {
            if (plane.planeTeam == PlaneTeam.Blue)
                bluePlanes++;
            if (plane.planeTeam == PlaneTeam.Red)
                redPlanes++;
        }

        RedTeamPlanes.GetComponent<Text>().text = redPlanes+ "/3";
        BlueTeamPlanes.GetComponent<Text>().text = bluePlanes + "/3";
    }
}
