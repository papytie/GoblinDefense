using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public event Action OnEnemiesInRange = null; // STEP 1
    public event Action OnTargetSelection = null; // STEP 2
    public event Action OnTargetAcquired = null; // STEP 3
    public event Action OnProjectileSpawn = null; // STEP 4
    public event Action OnTargetExpired = null; // CANCELATION

    [SerializeField] Projectiles projToSpawn = null;
    [SerializeField] Entity mainTarget = null;
    [SerializeField] TowerAnimation towerAnimation = null;

    [SerializeField, HideInInspector] List<Entity> entitiesInRange = new();
    [SerializeField] float towerRange = 20; // Entity detection range
    [SerializeField] float fireRate = 1; // time between each projectile spawn
    [SerializeField] float animationDuration = 1.66f; // set animation legth to proper synchro with fire rate
    [SerializeField] float detectionRate = 1f; // time between each detection loop
    [SerializeField] bool isAttacking = false; // switch to prevent multi attacking on a target
    public bool IsAttacking => isAttacking;
    public Entity MainTarget => mainTarget;

    void Start()
    {
        Init();
    }

    void Init()
    {
        towerAnimation = GetComponent<TowerAnimation>();
        towerAnimation.UpdateFireRateFloatValue(fireRate);
        
        EnableDetection(); // Step 0 : check for Entities in range

        OnEnemiesInRange += () => // Step 1 : Choose a target & disable detection
        {
            PickTarget();
            DisableDetection();
        };
        OnTargetSelection += () => // Step 2 : Check main Target if isDead || !InRange
        {
            CheckTarget();
            InvokeRepeating(nameof(FollowTarget), 0, .1f);
        };
        OnTargetExpired += () => // CANCELATION : Basically reset to Step 0
        {
            mainTarget = null;
            UpdateIsAttackingValue(false);
            EnableDetection();
        };
        OnTargetAcquired += () => // Step 3 : Begin Draw Arrow Animation & switch isAttacking
        {
            BeginFire();
            towerAnimation.UpdateAttackTriggerParam();
            UpdateIsAttackingValue(true);
        };
        OnProjectileSpawn += () => // Step 4 : switch isAttacking and resume to Step 2
        {
            UpdateIsAttackingValue(false);
            CheckTarget();
        };
    }

    void EntityDetection()
    {
        // cancel check if list is empty
        if (EnemyManager.Instance.AllEntitiesInScene.Count == 0) return; 

        // Clear EntityInRange to remove missing objects when an entity is destroyed
        entitiesInRange.Clear(); 
        
        if (mainTarget == null)
        {
            // check all Entity in list
            foreach (Entity _entity in EnemyManager.Instance.AllEntitiesInScene) 
            {
                // If in range add to list
                if (InRange(TowerPos(this), 
                    EntityPos(_entity), 
                    towerRange) 
                    && !_entity.IsDead) 
                        AddToList(_entity); 
            }
            if (entitiesInRange.Count == 0) return;
            OnEnemiesInRange?.Invoke(); // STEP 1
        }  
    }

    void PickTarget() 
    {
        if (entitiesInRange.Count == 0) return;
        mainTarget = entitiesInRange[0];
        OnTargetSelection?.Invoke(); // STEP 2
    }

    void CheckTarget()
    {
        if (!InRange(TowerPos(this), 
            EntityPos(mainTarget), 
            towerRange) 
            || mainTarget.IsDead)
            {
                OnTargetExpired?.Invoke(); // CANCELATION
                return;
            }
        OnTargetAcquired?.Invoke(); // STEP 3
    }

    void BeginFire()
    {
        if (!mainTarget || isAttacking) return;
        Invoke(nameof(SpawnArrow), animationDuration / fireRate);
    }

    void SpawnArrow()
    {
        Projectiles _proj = Instantiate(projToSpawn, 
            transform.position + transform.up * 2, 
            transform.rotation);
        OnProjectileSpawn?.Invoke(); // STEP 4
        _proj.SetTowerTargetRef(this);
    }

    #region Utilities
    void EnableDetection() // Run EntityDetection each .x sec instead of Update, less performance used
    {
        InvokeRepeating(nameof(EntityDetection), 0, detectionRate);
    }

    void DisableDetection()
    {
        CancelInvoke(nameof(EntityDetection));
    }

    bool InList(Entity _entityToCheck) // simple check
    {
        return entitiesInRange.Contains(_entityToCheck); 
    }

    void AddToList(Entity _entityToAdd) // Check and Add if not already in list
    {
        if(!InList(_entityToAdd))
        entitiesInRange.Add(_entityToAdd);
    }

    void RemoveFromList(Entity _entityToAdd) // Check and Remove if in list
    {
        if(InList(_entityToAdd))
        entitiesInRange.Remove(_entityToAdd);
    }

    bool InRange(Vector3 _from, Vector3 _to, float _range)
    {
        if (Vector3.Distance(_from, _to) < _range) return true;
        else return false;
    }

    Vector3 EntityPos(Entity _entity)
    {
        return _entity.transform.position;
    }

    Vector3 TowerPos(Tower _tower)
    {
        return _tower.transform.position;
    }

    void FollowTarget()
    {
        if (!mainTarget)
        {
            CancelInvoke(nameof(FollowTarget));
            return;
        }
        transform.LookAt(mainTarget.transform.position, transform.up);
    }

    void UpdateIsAttackingValue(bool _value)
    {
        isAttacking = _value;
        towerAnimation.UpdateIsAttackingBoolParam(isAttacking); // Update Anim value
    }
    #endregion Utilities

    // -------- DEBUG ---------
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, towerRange);
        Gizmos.color = Color.white;
        Gizmos.color = Color.red;
        if (mainTarget == null) return;
        Gizmos.DrawWireCube(mainTarget.AimingPoint, new Vector3(1, 1, 1));
    }
}
