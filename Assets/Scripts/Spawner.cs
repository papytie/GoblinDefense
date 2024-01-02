using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct FWave //Contains all variables & options for a wave
{
    public List<FGroup> AllGroupsInWave => allGroupsInWave;
    public float TimeBetweenGroups => timeBetweenGroups;

    [Header("Wave Settings")]
    [SerializeField] float timeBetweenGroups;
    [SerializeField] List<FGroup> allGroupsInWave;
}

[Serializable]
public struct FGroup //Contains variables & options for each groups of entities in wave
{
    public Entity TypeOfEntity => typeOfEntity;
    public int NumberOfEntity => numberOfEntity;
    public float SpawnRate => spawnRate;
    //public bool IsRandomWave => isRandomWave; //TODO: create an option to generate a random wave with max and min values

    [Header("Group Settings")]
    [SerializeField] Entity typeOfEntity;
    [SerializeField] int numberOfEntity;
    [SerializeField] float spawnRate;
    [SerializeField] bool isRandomWave;
}

public class Spawner : MonoBehaviour 
{
    [Header("References")]
    [SerializeField] PathManager pathManager = null;

    [Header("Spawner Settings")]
    [SerializeField] float timeBetweenWave = 30; //TODO: create an option to begin next wave only when all entities are dead
    [SerializeField] float startTime = 5;
    [SerializeField] float upOffsetSpawn = 2;
        
    [Header("Waves Settings")]
    [SerializeField] List<FWave> wavesToSpawn = new();

    FGroup currentGroup;
    FWave currentWave;
    int currentWaveIndex = 0;
    int currentGroupIndex = 0;
    int entityCount = 0;


    //bool allWavesAreSpawned = false; //TODO: switch for end game

    event Action OnNextWave = null;
    event Action OnNextGroup = null;
    event Action OnEntitySpawn = null;

    void Start()
    {
        InitSpawner();
        Invoke(nameof(WaveManagement), startTime);
    }

    void InitSpawner()
    {
        pathManager = FindObjectOfType<PathManager>();

        OnNextWave += () => Invoke(nameof(WaveManagement), timeBetweenWave);
        OnNextGroup += () => Invoke(nameof(GroupManagement), currentWave.TimeBetweenGroups);
        OnEntitySpawn += () => Invoke(nameof(EntityManagement), currentGroup.SpawnRate);

    } //Initialise ref & events

    void WaveManagement()
    {
        /*if (currentWaveIndex > wavesToSpawn.Count -1)
        {
            allWavesAreSpawned = true;
            return;
        }*/

        currentWave = wavesToSpawn[currentWaveIndex];
        GroupManagement();
    } //Check for waves to spawn, set currentWave ref & launch sequence || Stop sequence

    void GroupManagement()
    {
        if (currentGroupIndex > currentWave.AllGroupsInWave.Count -1)
        {
            currentWaveIndex++;
            currentGroupIndex = 0;
            OnNextWave?.Invoke();
            return;
        }

        currentGroup = currentWave.AllGroupsInWave[currentGroupIndex];
        EntityManagement();
    } //Check for Group to spawn in wave, set currentGroup ref & continue sequence || Go to next Wave

    void EntityManagement()
    {
        if(entityCount >= currentGroup.NumberOfEntity)
        {
            currentGroupIndex++;
            entityCount = 0;
            OnNextGroup?.Invoke();
            return;
        } 

        SpawnEntity(currentGroup.TypeOfEntity);
        entityCount++;
        OnEntitySpawn?.Invoke();
    } //Check if all entities have spawned and call SpawnEntity || Go to next Group

    void SpawnEntity(Entity _toSpawn)
    {
        Entity _entity = Instantiate(_toSpawn, pathManager.Path[0].transform.position, pathManager.Path[0].transform.rotation);
        _entity.SetEntityRef(pathManager, this);
    } //Spawn target entity and give it pathManager and spawner ref

}
