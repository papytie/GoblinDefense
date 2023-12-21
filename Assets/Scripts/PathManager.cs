using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [SerializeField] List<Pathpoint> path = new();

    public List<Pathpoint> Path => path;
    void Start()
    {
        InitPath();
    }

    void Update()
    {
        
    }
    void CreatePath()
    {
        List<Pathpoint> _allPoints = FindObjectsByType<Pathpoint>(FindObjectsSortMode.None).ToList();
        path = _allPoints.OrderBy(_point => _point.name).ToList();
        
    }
    void InitPath()
    {
        CreatePath();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        int _size = path.Count-1;
        for (int i = 0; i < _size; i++)
        {
            Gizmos.DrawLine(path[i].transform.position, path[i + 1].transform.position);
        }
    }
    
}
