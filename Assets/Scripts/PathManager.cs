using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager : Singleton<PathManager>
{
    public List<GameObject> Path => path;

    [Header("Path Settings")]
    [SerializeField] List<GameObject> path = new();
    [SerializeField] float debugRadius = 1;
    [SerializeField] bool showDebugGizmos = true;

    void Start()
    {
        InitPath();
    }

    void InitPath()
    {
        path = FindPathPoints();
    }

    List<GameObject> FindPathPoints()
    {
        List<GameObject> _allPoints = new();
        foreach (Transform _child in transform)
        {
            _allPoints.Add(_child.gameObject);
        }
        return _allPoints;
    } //get all gameObjects that is a child of this gameObject by Transform

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        if (!Application.IsPlaying(this))
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.GetChild(i).position, debugRadius);
                Gizmos.color = Color.white;

                if (i + 1 < transform.childCount)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine((transform.GetChild(i).position + transform.up * debugRadius), (transform.GetChild(i + 1).position + transform.up * debugRadius));
                    Gizmos.color = Color.white;
                }
            }
            return;
        }
        if (path.Count < 1) return;
        for (int i = 0; i < path.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(path[i].transform.position, debugRadius);
            Gizmos.color = Color.white;

            if (i + 1 < path.Count)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(path[i].transform.position + transform.up * debugRadius, path[i + 1].transform.position + transform.up * debugRadius);
                Gizmos.color = Color.white;
            }
        }
    } //Debug
    
}
