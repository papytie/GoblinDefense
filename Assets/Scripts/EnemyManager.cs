using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField]List<Entity> allEntitiesInScene = new();
    public List<Entity> AllEntitiesInScene => allEntitiesInScene;

    public void AddEntity(Entity _entity)
    {
        if (!_entity || InList(_entity)) return; // Check if valid || Already in list
        allEntitiesInScene.Add(_entity);
    }

    public void RemoveEntity(Entity _entity) 
    {
        if (!_entity || !InList(_entity)) return; // Check if valid || NOT Already in list
        allEntitiesInScene.Remove(_entity);
    }

    public void RemoveEntity(int _entityIndex) // Overload to remove entity with Index instead of direct Ref
    {
        if(!InList(_entityIndex)) return;
        allEntitiesInScene.RemoveAt(_entityIndex);
    }

    public void DestroyAllEntities() // NUKE // Destroy all entities in list and clear list
    {
        foreach (Entity _entity in allEntitiesInScene)
        {
            Destroy(_entity.gameObject);
        }
        allEntitiesInScene.Clear();
    }

    public bool InList(Entity _entityToCheck) 
    {
        return allEntitiesInScene.Contains(_entityToCheck); // Return True if Entity is in List, else return False
    }
    
    public bool InList(int _entityIndex) // Overload to check Entity by index
    {
        if(_entityIndex >= allEntitiesInScene.Count || _entityIndex < 0) return false; // return false if Index is out of bounds
        return InList(allEntitiesInScene[_entityIndex]); // Return True if Entity is in List, else return False
    }

    public int GetEnemyCount()
    {
        return AllEntitiesInScene.Count;
    }
   
}
