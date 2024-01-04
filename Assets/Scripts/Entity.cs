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
    public bool IsWounded { get => isWounded; set { isWounded = value;}} 

    [Header("Entity Settings")]
    
    [Header("References")]
    [SerializeField] PathManager pathManager = null;
    [SerializeField] Spawner spawner = null;
    [SerializeField] EntityAnimation entityAnimation = null;

    [Header("Stats")]
    [SerializeField] int baseHealth = 10;
    [SerializeField] int maxHealth = 20;
    [SerializeField] int moneyReward = 1;
    [SerializeField] int damageToPlayer = 1;
    [SerializeField] float baseMoveSpeed = 5;
    [SerializeField] float woundedMoveSpeed = 5;
    
    [Header("Params")]
    [SerializeField] float despawnTime = 5;
    [SerializeField] float maxRandDist = 2;
    [SerializeField] float hitAnimDuration = .2f;
    [SerializeField] float reachPointDistance = .1f;
    [SerializeField] float aimingPointHeight = 1;
    
    Vector3 targetPoint = Vector3.zero;
    Vector3 aimingPoint = Vector3.zero;
    bool canMove = true;
    bool isDead = false;
    bool isWounded = false;
    int currentHealth = 1;
    int currentIndex = 1;
    float currentMoveSpeed = 1;

    // Event used for Entity reactions behaviours
    event Action OnEntityIsWounded = null;
    event Action OnDeath = null;
    event Action OnPathComplete = null;

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

        currentHealth = baseHealth;
        currentMoveSpeed = baseMoveSpeed;

        OnEntityIsWounded += () =>
        {
            isWounded = true;
            entityAnimation.UpdateIsWoundedBoolParam(true);
            CanMove = false;
            currentMoveSpeed = woundedMoveSpeed;
            Invoke(nameof(SetCanMoveTrue), hitAnimDuration);
        };

        OnDeath += () =>
        {
            PlayerStats.Instance.AddMoney(moneyReward);
            entityAnimation.UpdateIsDeadTriggerParam();
            IsDead = true;
            Invoke(nameof(DestroyEntity), despawnTime);
        };

        OnPathComplete += () =>
        {
            PlayerStats.Instance.RemoveHealth(damageToPlayer);
            DestroyEntity();
        };
    } //add logic to essentials events

    void FollowPath()
    {
        if(isDead || !canMove) return;

        transform.position += (targetPoint - transform.position).normalized * currentMoveSpeed * Time.deltaTime;
        transform.LookAt(targetPoint);
        
        float _dist = Vector3.Distance(transform.position, targetPoint);
        if (_dist < reachPointDistance)
        {
            currentIndex++;
            if (currentIndex > pathManager.Path.Count - 1)
            {
                OnPathComplete?.Invoke();
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
        currentHealth -= _dmg;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnDeath?.Invoke();
            return;
        }
        if (currentHealth < baseHealth/2 && !isWounded) 
        {
            OnEntityIsWounded?.Invoke();
        }
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
