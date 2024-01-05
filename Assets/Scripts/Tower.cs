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

    [Header("References")]
    [SerializeField] Projectiles projToSpawn = null;
    [SerializeField] Entity mainTarget = null;
    [SerializeField] TowerAnimation towerAnimation = null;

    [Header("Settings")]
    [SerializeField] float towerRange = 20; // Entity detection range
    [SerializeField] float fireRate = 1; // time between each projectile spawn
    [SerializeField] float animationDuration = 1.66f; // set animation legth to proper synchro with fire rate
    [SerializeField] float detectionRate = 1f; // time between each detection loop
    [SerializeField] bool showDebugGizmos = true; // show debug

    List<Entity> entitiesInRange = new();
    bool isAttacking = false; // switch to prevent multi attacking on a target
    Quaternion baseRotation = Quaternion.identity; // used to save default rotation

    // The tower attack behaviour use a serie of events : (see below)
    event Action OnEnemiesInRange = null;
    event Action OnTargetSelection = null;
    event Action OnTargetAcquired = null;
    event Action OnProjectileSpawn = null;
    event Action OnTargetExpired = null;

    private void Awake()
    {
        towerAnimation = GetComponent<TowerAnimation>();
        
    }
    void Start()
    {
        InitTower();
    }

    void InitTower()
    {
        towerAnimation.UpdateFireRateFloatValue(fireRate);

        baseRotation = transform.rotation; // Save default rotation

        EnableDetection(); // Step 0 : check for Entities in range

        OnEnemiesInRange += () => // Step 1 : Choose a target & disable detection
        {
            PickTarget();
            DisableDetection();
        };
        OnTargetSelection += () => // Step 2 : Check main Target if isDead || !InRange
        {
            CheckTarget();
        };
        OnTargetExpired += () => // CANCELATION : Basically reset to Step 0
        {
            mainTarget = null;
            IsAttacking = false;
            transform.rotation = baseRotation; // Reset to default rotation
            EnableDetection();
        };
        OnTargetAcquired += () => // Step 3 : Begin Draw Arrow Animation & switch isAttacking
        {
            InvokeRepeating(nameof(FollowTarget), 0, .1f);
            InvokeRepeating(nameof(CheckTarget), 0, .1f);
            IsAttacking = true;
            //BeginFire();
            //towerAnimation.ToggleAttackTriggerParam();
        };
        OnProjectileSpawn += () => // Step 4 : switch isAttacking and resume to Step 2
        {
            IsAttacking = false;
            CheckTarget();
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
        //OnTargetSelection?.Invoke(); // STEP 2
    }// Take the first element of the list as mainTarget


    /*void BeginFire()
    {
        if (!mainTarget || isAttacking) return;
        Invoke(nameof(SpawnArrow), animationDuration / fireRate);
    }// Begin animation and Invoke delayed projectile*/

    void SpawnArrow()
    {
        Projectiles _proj = Instantiate(projToSpawn, transform.position + transform.up * 2, transform.rotation);
        //OnProjectileSpawn?.Invoke(); // STEP 4
        _proj.SetTowerTargetRef(this);
    }// Spawn projectile and end attack sequence

    void CheckTarget()
    {
        if (Vector3.Distance(transform.position, mainTarget.transform.position) > towerRange || mainTarget.IsDead)
        {
            CancelInvoke(nameof(CheckTarget));
            OnTargetExpired?.Invoke(); // CANCELATION
            //return;
        }
        //OnTargetAcquired?.Invoke(); // STEP 3
    }// Check that mainTarget is alive and in range

    void FollowTarget()
    {
        if (!mainTarget)
        {
            CancelInvoke(nameof(FollowTarget));
            return;
        }
        //transform.LookAt(mainTarget.transform.position, transform.up);
        transform.rotation = Quaternion.LookRotation(mainTarget.transform.position - transform.position); //Look to target
        transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w); //Constrain X and Z rotations to 0
        
    }// LookAt Target or cancel own Invoke

    void EnableDetection()
    {
        InvokeRepeating(nameof(EntityDetection), 0, detectionRate);
    }

    void DisableDetection()
    {
        CancelInvoke(nameof(EntityDetection));
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
