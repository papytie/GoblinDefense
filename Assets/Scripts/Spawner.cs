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

    [Header("Wave Settings")]
    [SerializeField] float timeBetweenGroups;
    [SerializeField] List<FGroup> allGroupsInWave;
}

public class Spawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PathManager pathManager = null;

    [Header("Spawner Settings")]
    [SerializeField] float currentTime = 0;
    [SerializeField] float maxTime = 5;
    //[SerializeField] Entity toSpawn = null;
    //[SerializeField] float currentTimeWave = 0;
    //[SerializeField] float nextWave = 30;
    //[SerializeField] float minSpawnTime = 1;
    //[SerializeField] float maxSpawnTime = 5;
        
    [SerializeField] int currentWaveIndex = 0;
    [SerializeField] int currentGroupIndex = 0;
    [SerializeField] bool allWavesAreSpawned = false;

    [SerializeField] List<FWave> wavesToSpawn = new();

    void Start()
    {
        pathManager = FindObjectOfType<PathManager>();
        
    }

    void Update()
    {
        WaveTimer(maxTime);
    }

    void ManageWaves(FWave _waveToSpawn)
    {
        if (allWavesAreSpawned == true) return;
        if (wavesToSpawn.Count > 0) 
        { 
            foreach(FGroup _group in _waveToSpawn.AllGroupsInWave) 
            {
                SpawnGroup(_group);
                //Debug.Log("group check foreach");
            }
            currentWaveIndex++;
            if (currentWaveIndex == wavesToSpawn.Count)
                allWavesAreSpawned = true;
        }
    }

    void SpawnGroup(FGroup _groupToSpawn) // POURQUOI CA MARCHE ? La boucle for se joue tant que sa condition n'est pas remplie
    {
        int _size = _groupToSpawn.NumberOfEntity;
        for (int i = 0; i < _size; i++)
        {
            SpawnEntity(_groupToSpawn.TypeOfEntity);
            //Debug.Log("current for [i] value is : " + i);
        }
        //Debug.Log("truc");

    }

    void WaveTimer(float _maxTime)
    {
        currentTime += Time.deltaTime;
        if(currentTime >= _maxTime) 
        {
            currentTime = 0;
            ManageWaves(wavesToSpawn[0]);
        }
    }

    void SpawnEntity(Entity _toSpawn)
    {
        Entity _entity = Instantiate(_toSpawn, pathManager.Path[0].transform.position, Quaternion.identity);
        _entity.SetEntityRef(pathManager, this);
        //allEntities.Add(_entity);
    }

    public void RemoveEntityFromList(Entity _entity)
    {
        //allEntities.Remove(_entity);
    }

    /*void WaveSpawner()
    {
        if (wavesToSpawn.Count == 0)
        {
            Debug.Log("No wave to spawn");
            return;
        }
        
        currentWave = wavesToSpawn[currentWaveIndex]; // Set currentWave
        
        currentGroup = currentWave.AllGroupsInWave[currentGroupIndex]; // Set currentGroup
       
    }*/
}
