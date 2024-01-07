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
    //[SerializeField] bool isRandomWave;
}

public class Spawner : MonoBehaviour 
{
    public float CurrentWaveTime => currentWaveTime;
    public float CurrentWaveTotalTime => currentWaveTotalTime;
    public string UIWaveIndex => uIWaveIndex;
    public bool IsWaveSpawning => isWaveSpawning;

    [Header("References")]
    [SerializeField] PathManager pathManager = null;

    [Header("Spawner Settings")]
    [SerializeField] float timeBetweenWave = 30; //TODO: create an option to begin next wave only when all entities are dead
    [SerializeField] float startTime = 5;
        
    [Header("Waves Settings")]
    [SerializeField] List<FWave> wavesToSpawn = new();

    FGroup currentGroup;
    FWave currentWave;
    int currentWaveIndex = 0;
    int currentGroupIndex = 0;
    int entityCount = 0;
    float currentWaveTime = 0;
    float currentWaveTotalTime = 0;
    bool isWaveSpawning = false;
    string uIWaveIndex = "";

    //bool allWavesAreSpawned = false; //TODO: switch for end game

    event Action OnNextWave = null;
    event Action OnNextGroup = null;
    event Action OnEntitySpawn = null;

    void Start()
    {
        InitSpawner();
        Invoke(nameof(WaveManagement), startTime);
        //InitiateWaveTimer();
    }

    private void Update()
    {
        currentWaveTime = UpdateWaveTimerValue();
        //Debug.Log("current Timer value is : " +  currentWaveTime);
    }

    void InitSpawner()
    {
        pathManager = FindObjectOfType<PathManager>();
        currentWaveTotalTime = startTime;
        Invoke(nameof(SetIsWaveStarted), startTime);

        OnNextWave += () =>
        {
            Invoke(nameof(WaveManagement), timeBetweenWave);
        };

        OnNextGroup += () => Invoke(nameof(GroupManagement), currentWave.TimeBetweenGroups);
        OnEntitySpawn += () => Invoke(nameof(EntityManagement), currentGroup.SpawnRate);

    } //Initialise ref & events

    void WaveManagement()
    {
        if (currentWaveIndex > wavesToSpawn.Count -1)
        {
            isWaveSpawning = false;
            return;
        }

        currentWave = wavesToSpawn[currentWaveIndex];
        InitiateWaveTimer();
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

    float UpdateWaveTimerValue()
    {
        currentWaveTime += Time.deltaTime;
        if (currentWaveTime >= currentWaveTotalTime) 
            return currentWaveTotalTime;
            
        else return currentWaveTime;
    } //Custom timer with totalWaveTime as maximum, used to show Wave Start Timer in HUD

    float WaveTotalTime(FWave _currentWave)
    {
        float _totalTime = 0;
        foreach(FGroup _group in _currentWave.AllGroupsInWave)
        {
            _totalTime += (_currentWave.TimeBetweenGroups + GroupTotalTime(_group));

        }
        return _totalTime + timeBetweenWave;
    } //Add all waiting time between groups to get wave duration in seconds

    float GroupTotalTime(FGroup _group)
    {
        float _totalTime = 0;
        int _size = _group.NumberOfEntity;
        for (int i = 0; i < _size; i++)
        {
            _totalTime += _group.SpawnRate;
        }
        return _totalTime;
    } //Add all spawn rate time to get total spawn duration in seconds

    void InitiateWaveTimer()
    {
        currentWaveTime = 0;
        currentWaveTotalTime = WaveTotalTime(currentWave); //Set new totalTime
        uIWaveIndex = "Wave " + (currentWaveIndex + 1).ToString();
        Debug.Log("Wave total time is : " + currentWaveTotalTime);
    } //Reset wave Timer

    void SetIsWaveStarted()
    {
        isWaveSpawning = true;
    } //Used with Invoke to delay boolean switch to match start
}
