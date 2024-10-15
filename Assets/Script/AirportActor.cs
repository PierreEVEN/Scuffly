using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controlleur d'aeroport : regroupe les differents points de spawn.
public class AirportActor : MonoBehaviour
{
    public PlaneTeam AirportTeam = PlaneTeam.Blue;

    public string AirportName = "No name";

    // Liste des aeroports activés
    public static List<AirportActor> AirportList = new List<AirportActor>();

    void OnEnable()
    {
        AirportList.Add(this);
    }

    void OnDisable()
    {
        AirportList.Remove(this);
    }

    // Retourne la liste des points de spawn disponibles
    public PlaneSpawnpoint[] GatherSpawnpoints()
    {
        return GetComponentsInChildren<PlaneSpawnpoint>();
    }

    // Retourne l'aeroport appartenant a une équipe le plus proche d'un point
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
