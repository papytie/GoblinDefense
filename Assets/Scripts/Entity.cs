using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class Entity : MonoBehaviour
{
    public event Action OnDamageTaken = null;
    public event Action OnDeath = null;

    [SerializeField] PathManager pathManager = null;
    [SerializeField] Spawner spawner = null;
    [SerializeField] EntityAnimation entityAnimation = null;

    [SerializeField] float moveSpeed = 5;
    [SerializeField] float maxRandDist = 2;
    [SerializeField] float healthPoints = 10;
    [SerializeField] float hitAnimDuration = .2f;
    [SerializeField] int currentIndex = 1;
    [SerializeField] bool isDead = false;
    [SerializeField] bool canMove = true;
    [SerializeField] Vector3 targetPoint = Vector3.zero;
    [SerializeField] Vector3 aimingPoint = Vector3.zero;
    public Vector3 AimingPoint => aimingPoint;

    public bool IsDead => isDead;
    void Start()
    {
        InitEntity();
    }

    void Update()
    {
        FollowPath();
        SetAimingPoint();
    }
    void InitEntity()
    {
        EnemyManager.Instance.AddEntity(this); // Add itself to Manager list
        entityAnimation = GetComponent<EntityAnimation>();
        targetPoint = RandomizedPointPos();
        OnDamageTaken += entityAnimation.UpdateIsHitTriggerParam;
        OnDeath += entityAnimation.UpdateIsDeadTriggerParam;
        
    }
    void FollowPath()
    {
        if(isDead || !canMove) return;
        transform.position += (targetPoint - transform.position).normalized * moveSpeed * Time.deltaTime;
        transform.LookAt(targetPoint);
        
        float _dist = Vector3.Distance(transform.position, targetPoint);
        if (_dist < 0.01)
        {
            currentIndex++;
            if (currentIndex > pathManager.Path.Count - 1)
            {
                DestroyEntity();
                return;
            }
            targetPoint = RandomizedPointPos();
        }
    }
    Vector3 RandomizedPointPos()
    {
        Pathpoint _nextPoint = pathManager.Path[currentIndex];
        Vector3 _randomPos = Random.insideUnitSphere * maxRandDist;
        Vector3 _pathPointPos = _nextPoint.transform.position;
        Vector3 _randomizedPos = new Vector3(_pathPointPos.x + _randomPos.x, _pathPointPos.y, _pathPointPos.z + _randomPos.z);
        return _randomizedPos;
    }
    public void TakeDamage(int _dmg)
    {
        canMove = false;
        healthPoints -= _dmg;
        if (healthPoints <= 0)
        {
            healthPoints = 0;
            isDead = true;
            OnDeath?.Invoke();
            Invoke(nameof(DestroyEntity), 3f);
            return;
        }
        OnDamageTaken?.Invoke();
        Invoke(nameof(MoveSwitch), hitAnimDuration);
    }

    void MoveSwitch()
    {
        canMove = true;
    }
    public void DestroyEntity()
    {
        Destroy(gameObject);
    }

    public void SetEntityRef(PathManager _pathManagerRef, Spawner _spawnerRef)
    {
        pathManager = _pathManagerRef;
        spawner = _spawnerRef;
    }
    void SetAimingPoint()
    {
        aimingPoint = transform.position + Vector3.up * 1;
    }
    private void OnDestroy()
    {
        EnemyManager.Instance.RemoveEntity(this);
    }
}
