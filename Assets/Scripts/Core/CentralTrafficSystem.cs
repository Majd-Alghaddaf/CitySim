using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if(generateInitialPopulation) // to not always generate automatically for testing purposes
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

    private int initialPathIndex;
    private int initialWaypointIndex;
    private Path initialPath;
    private Waypoint initialWaypoint;

    private List<Waypoint> usedWaypoints = new List<Waypoint>();

    private void InitialMapPopulation()
    {
        int numOfVehiclesToInstantiate = paths.Count * vehicleSpawningMultiplier;
        int i = 0;

        while(i < numOfVehiclesToInstantiate)
        {
            SetInitialPathAndWaypoint();

            if (VehicleAlreadySpawnedAtInitialWaypoint())
            {
                continue;
            }

            usedWaypoints.Add(initialWaypoint);

            GameObject spawnedVehicle = InstantiateRandomVehiclePrefabAtWaypointPosition();

            VehicleMovement spawnedVehicleMovement = spawnedVehicle.GetComponent<VehicleMovement>();
            spawnedVehicleMovement.gameObject.name = "Vehicle" + spawnedVehicleMovement.gameObject.transform.GetSiblingIndex();
            spawnedVehicleMovement.SetPathAndCurrentWaypointIndex(initialPath, initialWaypointIndex);
            vehicles.Add(spawnedVehicleMovement);

            i++;
        }
    }

    private GameObject InstantiateRandomVehiclePrefabAtWaypointPosition()
    {
        Vector3 initialSpawnPosition = initialWaypoint.transform.position;
        int initialVehiclePrefabToSpawnIndex = UnityEngine.Random.Range(0, vehiclesPrefabs.Count);
        return Instantiate(vehiclesPrefabs[initialVehiclePrefabToSpawnIndex], initialSpawnPosition, Quaternion.identity, vehiclesContainer.transform);
    }

    private bool VehicleAlreadySpawnedAtInitialWaypoint()
    {
        return usedWaypoints.Contains(initialWaypoint);
    }

    private void SetInitialPathAndWaypoint()
    {
        initialPathIndex = UnityEngine.Random.Range(0, paths.Count);
        initialPath = paths[initialPathIndex];
        initialWaypointIndex = UnityEngine.Random.Range(0, initialPath.waypoints.Count - 1); // -1 because we don't want to take the last wp which is an intersection wp
        initialWaypoint = initialPath.waypoints[initialWaypointIndex];
    }

    public GeneratedPathResponse RequestNewPath(EndWaypoint currentEndWaypoint)
    {
        Collider[] hitColliders = Physics.OverlapSphere(currentEndWaypoint.transform.position, settings.endComponentOverlapSphereRadius, LayerMask.GetMask(settings.endComponentMaskName));

        if(hitColliders.Length > 1 || hitColliders.Length == 0)
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
