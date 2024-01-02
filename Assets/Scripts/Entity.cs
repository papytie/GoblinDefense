using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class Entity : MonoBehaviour
{
    public Vector3 AimingPoint { get => aimingPoint; set { aimingPoint = value;}}
    public bool IsDead { get => isDead; set { isDead = value;}} 
    public bool CanMove { get => canMove; set { canMove = value;}} 

    [Header("References")]
    [SerializeField] PathManager pathManager = null;
    [SerializeField] Spawner spawner = null;
    [SerializeField] EntityAnimation entityAnimation = null;

    [Header("Settings")]
    [SerializeField] float healthPoints = 10;
    [SerializeField] float moveSpeed = 5;
    [SerializeField] float despawnTime = 5;
    [SerializeField] float maxRandDist = 2;
    [SerializeField] float hitAnimDuration = .2f;
    [SerializeField] float reachPointDistance = .1f;
    [SerializeField] float aimingPointHeight = 1;
    
    bool canMove = true;
    Vector3 targetPoint = Vector3.zero;
    Vector3 aimingPoint = Vector3.zero;
    bool isDead = false;
    int currentIndex = 1;

    // Event used for Entity reactions behaviours
    event Action OnDamageTaken = null;
    event Action OnDeath = null;

    void Start()
    {
        InitEntity();
    }

    void Update()
    {
        FollowPath();
        AimingPoint = transform.position + Vector3.up * aimingPointHeight; //Set an aiming point for projectiles
    }

    void InitEntity()
    {
        EnemyManager.Instance.AddEntity(this); // Add itself to Manager list

        entityAnimation = GetComponent<EntityAnimation>();
        targetPoint = RandomizedPointPos();

        OnDamageTaken += () =>
        {
            entityAnimation.UpdateIsHitTriggerParam();
            CanMove = false;
            Invoke(nameof(SetCanMoveTrue), hitAnimDuration);
        };

        OnDeath += () =>
        {
            entityAnimation.UpdateIsDeadTriggerParam();
            healthPoints = 0;
            IsDead = true;
            Invoke(nameof(DestroyEntity), despawnTime);
        };
    } //add Functions to essentials events

    void FollowPath()
    {
        if(isDead || !canMove) return;

        transform.position += (targetPoint - transform.position).normalized * moveSpeed * Time.deltaTime;
        transform.LookAt(targetPoint);
        
        float _dist = Vector3.Distance(transform.position, targetPoint);
        if (_dist < reachPointDistance)
        {
            currentIndex++;
            if (currentIndex > pathManager.Path.Count - 1)
            {
                DestroyEntity();
                return;
            }
            targetPoint = RandomizedPointPos();
        }
    } //Move to next path point and is destroy if last point

    Vector3 RandomizedPointPos()
    {
        GameObject _nextPoint = pathManager.Path[currentIndex];
        Vector3 _randomPos = Random.insideUnitSphere * maxRandDist;
        Vector3 _pathPointPos = _nextPoint.transform.position;
        Vector3 _randomizedPos = new Vector3(_pathPointPos.x + _randomPos.x, _pathPointPos.y, _pathPointPos.z + _randomPos.z);
        return _randomizedPos;
    } //Get random destination around next path point

    void SetCanMoveTrue()
    {
        canMove = true;
    } //Simple set used in an Invoke to delay canMove re enable

    public void TakeDamage(int _dmg)
    {
        healthPoints -= _dmg;
        if (healthPoints <= 0)
        {
            OnDeath?.Invoke();
            return;
        }
        OnDamageTaken?.Invoke();   
    } //Called by Projectiles

    public void DestroyEntity()
    {
        Destroy(gameObject);
    } //Destroy method used out or to delay despawn with an Invoke

    public void SetEntityRef(PathManager _pathManagerRef, Spawner _spawnerRef)
    {
        pathManager = _pathManagerRef;
        spawner = _spawnerRef;
    } //Used by Spawner to set refs

    private void OnDestroy()
    {
        EnemyManager.Instance.RemoveEntity(this);
    } //Remove object of manager list
}
