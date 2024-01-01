using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct FGroup
{
    public Entity TypeOfEntity => typeOfEntity;
    public int NumberOfEntity => numberOfEntity;
    public float SpawnRate => spawnRate;
    public bool IsRandomWave => isRandomWave;

    [Header("Group Settings")]
    [SerializeField] Entity typeOfEntity;
    [SerializeField] int numberOfEntity;
    [SerializeField] float spawnRate;
    [SerializeField] bool isRandomWave;
}

[Serializable]
public struct FWave
{
    public List<FGroup> AllGroupsInWave => allGroupsInWave;
    public float TimeBetweenGroups => timeBetweenGroups;

    [Header("Wave Settings")]
    [SerializeField] float timeBetweenGroups;
    [SerializeField] List<FGroup> allGroupsInWave;
}

public class Spawner : MonoBehaviour //TODO : Redo all the logic of spawn, broken at this time
{
    [Header("References")]
    [SerializeField] PathManager pathManager = null;

    [Header("Spawner Settings")]
    [SerializeField] float timeBetweenWave = 30;
    [SerializeField] float startTime = 5;
        
    [SerializeField] int currentWaveIndex = 0;
    [SerializeField] int currentGroupIndex = 0;
    [SerializeField] int entityCount = 0;
    [SerializeField] FGroup currentGroup;
    [SerializeField] FWave currentWave;
    
    [SerializeField] bool allWavesAreSpawned = false;
    [SerializeField] bool waveIsOver = false;
    [SerializeField] bool groupIsOver = false;

    [SerializeField] List<FWave> wavesToSpawn = new();

    event Action OnWaveBegin = null;
    event Action OnWaveEnd = null;

    void Start()
    {
        InitSpawner();
    }

    void Update()
    {

    }

    void InitSpawner()
    {
        pathManager = FindObjectOfType<PathManager>();
        Invoke(nameof(WaveManagement), startTime);

    }

    void WaveManagement()
    {
        if (waveIsOver) // Select and launch next wave if switch true
        {
            currentWaveIndex++;
            waveIsOver = false;
            if (currentWaveIndex > wavesToSpawn.Count -1)
            {
                allWavesAreSpawned = true;
                return;
            }
        }
        currentWave = wavesToSpawn[currentWaveIndex];
        GroupManagement();
    }

    void GroupManagement()
    {
        if (groupIsOver)
        {
            currentGroupIndex++;
            groupIsOver = false;
            if (currentGroupIndex > currentWave.AllGroupsInWave.Count -1)
            {
                currentGroupIndex = 0;
                waveIsOver = true;
                Invoke(nameof(WaveManagement), timeBetweenWave);
                return;
            }
        }
        currentGroup = currentWave.AllGroupsInWave[currentGroupIndex];
        EntityManagement();
    }

    void EntityManagement()
    {
        entityCount++;
        if(entityCount > currentGroup.NumberOfEntity)
        {
            groupIsOver = true;
            entityCount = 0;
            Invoke(nameof(GroupManagement), currentWave.TimeBetweenGroups);
            return;
        }
        SpawnEntity(currentGroup.TypeOfEntity);
        Invoke(nameof(EntityManagement), currentGroup.SpawnRate);
    }

    void SpawnEntity(Entity _toSpawn)
    {
        Entity _entity = Instantiate(_toSpawn, pathManager.Path[0].transform.position, pathManager.Path[0].transform.rotation);
        _entity.SetEntityRef(pathManager, this);
    }

}
