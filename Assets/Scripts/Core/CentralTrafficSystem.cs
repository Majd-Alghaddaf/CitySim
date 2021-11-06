using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WaypointInformation
{
    public Path path;
    public Waypoint waypoint;
    public int index;

    public WaypointInformation(Path path, Waypoint waypoint, int index)
    {
        this.path = path;
        this.waypoint = waypoint;
        this.index = index;
    }
}

public class CentralTrafficSystem : MonoBehaviour
{
    private static CentralTrafficSystem _instance;
    public static CentralTrafficSystem Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        if (generateInitialPopulation) // to not always generate automatically for testing purposes
        {
            InitialMapPopulation();
        }
    }

    [Header("Configuration")]
    [SerializeField] private Transform pathsContainer;
    [SerializeField] private Transform vehiclesContainer;
    [SerializeField] private Transform endComponentsContainer;
    [SerializeField] private int vehicleSpawningMultiplier = 2;
    [SerializeField] private Settings settings;
    [SerializeField] private List<GameObject> vehiclesPrefabs = new List<GameObject>();

    [Header("Debugging")]
    [SerializeField] private bool generateInitialPopulation = false;
    [SerializeField] private List<Path> paths = new List<Path>();
    [SerializeField] private List<VehicleMovement> vehicles = new List<VehicleMovement>();
    [SerializeField] private List<EndComponent> endComponents = new List<EndComponent>();

    private void InitialMapPopulation()
    {
        int numOfVehiclesToInstantiate = paths.Count * vehicleSpawningMultiplier;

        List<WaypointInformation> availableWaypointsInformation = GetAllAvailableWaypointsInformation();

        int i = 0;
        while (i < numOfVehiclesToInstantiate && availableWaypointsInformation.Count > 0)
        {
            WaypointInformation randomAvailableWaypointInformation = GetRandomAvailableWaypointInformation(availableWaypointsInformation);
            GameObject spawnedVehicle = InstantiateVehiclesAndSetInitialConditions(randomAvailableWaypointInformation);

            VehicleMovement spawnedVehicleMovement = spawnedVehicle.GetComponent<VehicleMovement>();
            spawnedVehicleMovement.SetPathAndCurrentWaypointIndex(randomAvailableWaypointInformation.path, randomAvailableWaypointInformation.index);
            vehicles.Add(spawnedVehicleMovement);

            i++;
        }
    }
    private List<WaypointInformation> GetAllAvailableWaypointsInformation()
    {
        int i = 0;
        List<WaypointInformation> availableWaypointsInformation = new List<WaypointInformation>();
        foreach (Path path in paths)
        {
            for (i = 0; i < path.waypoints.Count; i++)
            {
                WaypointInformation waypointInformation = new WaypointInformation(path, path.waypoints[i], i);
                availableWaypointsInformation.Add(waypointInformation);
            }
        }

        return availableWaypointsInformation;
    }

    private WaypointInformation GetRandomAvailableWaypointInformation(List<WaypointInformation> availableWaypointsInformation)
    {
        int randomAvailableWaypointIndex = UnityEngine.Random.Range(0, availableWaypointsInformation.Count);
        WaypointInformation randomAvailableWaypointInformation = availableWaypointsInformation[randomAvailableWaypointIndex];
        availableWaypointsInformation.Remove(randomAvailableWaypointInformation);
        return randomAvailableWaypointInformation;
    }

    private GameObject InstantiateVehiclesAndSetInitialConditions(WaypointInformation randomAvailableWaypointInformation)
    {
        Vector3 initialSpawnPosition = randomAvailableWaypointInformation.waypoint.transform.position;
        int initialVehiclePrefabToSpawnIndex = UnityEngine.Random.Range(0, vehiclesPrefabs.Count);
        GameObject spawnedVehicle = Instantiate(vehiclesPrefabs[initialVehiclePrefabToSpawnIndex], initialSpawnPosition, Quaternion.identity, vehiclesContainer.transform);
        spawnedVehicle.name = "Vehicle" + spawnedVehicle.transform.GetSiblingIndex();

        Vector3 lookDirection;
        if (randomAvailableWaypointInformation.index == 0)
        {
            lookDirection = randomAvailableWaypointInformation.path.waypoints[randomAvailableWaypointInformation.index + 1].transform.position - randomAvailableWaypointInformation.waypoint.transform.position;
        }
        else
        {
            lookDirection = randomAvailableWaypointInformation.waypoint.transform.position - randomAvailableWaypointInformation.path.waypoints[randomAvailableWaypointInformation.index - 1].transform.position;
        }
        spawnedVehicle.transform.rotation = Quaternion.LookRotation(lookDirection);

        return spawnedVehicle;
    }

    public GeneratedPathResponse RequestNewPath(EndWaypoint currentEndWaypoint)
    {
        Collider[] hitColliders = Physics.OverlapSphere(currentEndWaypoint.transform.position, settings.endComponentOverlapSphereRadius, LayerMask.GetMask(settings.endComponentMaskName));

        if (hitColliders.Length > 1 || hitColliders.Length == 0)
        {
            Debug.LogError($"Found no or more than one end component (traffic light or stop sign) near {currentEndWaypoint} - this should not happen");
        }

        EndComponent nearbyEndComponent = hitColliders[0].gameObject.GetComponent<EndComponent>();
        GeneratedPathResponse nearbyEndComponentResponse = nearbyEndComponent.GenerateResponseForNewPathAtEndComponent(currentEndWaypoint);

        return nearbyEndComponentResponse;
    }

#if UNITY_EDITOR
    #region EDITOR SCRIPTS
    public void PopulatePathsList()
    {
        paths = new List<Path>(pathsContainer.GetComponentsInChildren<Path>());
    }

    public void PopulateVehiclesList()
    {
        vehicles = new List<VehicleMovement>(vehiclesContainer.GetComponentsInChildren<VehicleMovement>());
    }

    public void PopulateEndComponentsList()
    {
        endComponents = new List<EndComponent>(endComponentsContainer.GetComponentsInChildren<EndComponent>());
    }

    public void RenameAndSetupPaths()
    {
        foreach (Path path in paths)
        {
            path.gameObject.name = "P" + path.gameObject.transform.GetSiblingIndex();
            path.PopulateWaypointsList();
            path.RenameWaypoints();
            path.SetupFirstAndEndWaypoint();
        }
    }
    public void RenameAndSetupEndComponents()
    {
        foreach (EndComponent endComponent in endComponents)
        {
            endComponent.DetectNearbyEndWaypoints();
            endComponent.name = "EC" + endComponent.gameObject.transform.GetSiblingIndex();
        }
    }
    #endregion
#endif
}
