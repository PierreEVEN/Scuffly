using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirportActor : MonoBehaviour
{
    public PlaneTeam AirportTeam = PlaneTeam.Blue;

    public static List<AirportActor> AirportList = new List<AirportActor>();

    void OnEnable()
    {
        AirportList.Add(this);
    }

    void OnDisable()
    {
        AirportList.Remove(this);
    }

    public PlaneSpawnpoint[] GatherSpawnpoints()
    {
        return GetComponentsInChildren<PlaneSpawnpoint>();
    }

    public static AirportActor GetClosestAirport(PlaneTeam team, Vector3 point)
    {
        AirportActor closestAirport = null;
        float closestDistance = 0;

        foreach (var airport in AirportList)
        {
            if (airport.AirportTeam == team)
            {
                if (!closestAirport || Vector3.Distance(airport.transform.position, point) < closestDistance)
                {
                    closestDistance = Vector3.Distance(airport.transform.position, point);
                    closestAirport = airport;
                }
            }
        }
        return closestAirport;
    }
}
