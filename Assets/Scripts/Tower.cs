using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TowerAnimation), typeof(Animator))]

public class Tower : MonoBehaviour
{
    public bool IsAttacking
    {
        get => isAttacking;
        set
        {
            isAttacking = value;
            towerAnimation.UpdateIsAttackingBoolParam(value); // Update Anim value
        }
    }
    public Entity MainTarget => mainTarget;

    [Header("Tower Settings")]

    [Header("Stats")]
    [SerializeField] float towerRange = 20; // Entity detection range
    [SerializeField] float fireRate = 1; // time between each projectile spawn
    
    [Header("Projectile")]
    [SerializeField] Projectiles projToSpawn = null;
    [SerializeField] float upProjOffset = 0; // offset position of projectile when spawned
    [SerializeField] float fwdProjOffset = 0; // offset position of projectile when spawned
    [SerializeField] float rgtProjOffset = 0; // offset position of projectile when spawned

    [Header("Parameters")]
    [SerializeField] float refreshRate = .1f; // refresh LookAt rotation, CheckTarget and Detection
    [SerializeField] bool showDebugGizmos = true; // show debug

    TowerAnimation towerAnimation = null; // ref to animation script
    List<Entity> entitiesInRange = new(); // detection list
    Entity mainTarget = null; // selected Target
    Quaternion baseRotation = Quaternion.identity; // used to save default rotation
    bool isAttacking = false; // switch to prevent multi attacking on a target

    // The tower attack behaviour use a serie of events : (see below)
    event Action OnEnemiesInRange = null;
    event Action OnTargetAcquired = null;
    event Action OnTargetExpired = null;

    void Start()
    {
        InitTower();
    }

    void InitTower()
    {
        towerAnimation = GetComponent<TowerAnimation>();
        towerAnimation.UpdateFireRateFloatValue(fireRate);

        baseRotation = transform.rotation; // Save default rotation

        EnableDetection(); // Step 0 : check for Entities in range

        OnEnemiesInRange += () => // Step 1 : Choose a target & disable detection
        {
            PickTarget(); //=> to Step 2 
            DisableDetection();
        };

        OnTargetAcquired += () => // Step 2 : Switch isAttacking and begin animation cycle
        {
            InvokeRepeating(nameof(FollowTarget), 0, refreshRate);
            InvokeRepeating(nameof(CheckTarget), 0, refreshRate);
            IsAttacking = true;
        };

        OnTargetExpired += () => // CANCELATION : reset to Step 0
        {
            mainTarget = null;
            IsAttacking = false;
            transform.rotation = baseRotation; // Reset to default rotation
            EnableDetection();
        };

        PlayerStats.Instance.OnGameOver += () =>
        {
            entitiesInRange.Clear();
            mainTarget = null;
            IsAttacking = false;
            towerAnimation.ToggleDeathTriggerParam();
        };

    }// Add Functions to essentials events

    void EntityDetection()
    {
        // cancel check if list is empty
        if (EnemyManager.Instance.AllEntitiesInScene.Count == 0) return;

        entitiesInRange.Clear(); //To remove missing objects when an entity is destroyed

        if (mainTarget == null)
        {
            towerAnimation.ToggleDetectionTriggerParam();
            foreach (Entity _entity in EnemyManager.Instance.AllEntitiesInScene) // check all Entity in list
            {
                if (_entity.IsDead || entitiesInRange.Contains(_entity)) continue;

                float _dist = Vector3.Distance(transform.position, _entity.transform.position);
                if (_dist < towerRange)
                    entitiesInRange.Add(_entity); // If in range add to list
            }

            if (entitiesInRange.Count > 0)
            {
                OnEnemiesInRange?.Invoke(); // STEP 1
            }
        }
    }// Used to check entities in range, create list and begin attack sequence

    void PickTarget()
    {
        if (entitiesInRange.Count == 0) return;

        mainTarget = entitiesInRange[0];
        OnTargetAcquired?.Invoke();
    }// Take the first element of the list as mainTarget

    void SpawnProjectile()
    {
        if (mainTarget == null) return;

        Projectiles _proj = Instantiate(projToSpawn, 
                                        transform.position + 
                                        (transform.up * upProjOffset) + 
                                        (transform.forward * fwdProjOffset) + 
                                        (transform.right * rgtProjOffset), 
                                        transform.rotation);
        _proj.SetTowerTargetRef(this);
    }// Called in Attack animation clip used

    void CheckTarget()
    {
        if (mainTarget == null) return;
        if (Vector3.Distance(transform.position, mainTarget.transform.position) > towerRange || mainTarget.IsDead)
        {
            CancelInvoke(nameof(CheckTarget));
            OnTargetExpired?.Invoke(); // CANCELATION
        }
    }// Check if mainTarget is alive and in range or cancel attack cycle

    void FollowTarget()
    {
        if (!mainTarget)
        {
            CancelInvoke(nameof(FollowTarget));
            return;
        }
        transform.rotation = Quaternion.LookRotation(mainTarget.transform.position - transform.position); //Look to target
        transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w); //Constrain X and Z rotations to 0
        
    }// LookAt Target or cancel own Invoke

    void EnableDetection()
    {
        InvokeRepeating(nameof(EntityDetection), 0, refreshRate);
    }

    void DisableDetection()
    {
        CancelInvoke(nameof(EntityDetection));
    }

    public void DestroyTower()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        
    }

    // -------- DEBUG ---------
    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, towerRange);
        Gizmos.color = Color.white;
        Gizmos.color = Color.red;
        if (mainTarget == null) return;
        Gizmos.DrawWireCube(mainTarget.AimingPoint, new Vector3(1, 1, 1));
    }
}
