using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField, HideInInspector] List<Entity> entitiesInRange = new();
    [SerializeField] float towerRange = 20; // Entity detection range
    [SerializeField] float fireRate = 1; // time between each projectile spawn
    [SerializeField] float animationDuration = 1.66f; // set animation legth to proper synchro with fire rate
    [SerializeField] float detectionRate = 1f; // time between each detection loop
    [SerializeField] bool isAttacking = false; // switch to prevent multi attacking on a target

    // The tower attack behaviour use a sequence of events : ( below)
    event Action OnEnemiesInRange = null;
    event Action OnTargetSelection = null;
    event Action OnTargetAcquired = null;
    event Action OnProjectileSpawn = null;
    event Action OnTargetExpired = null;

    void Start()
    {
        InitTower();
    }

    void InitTower()
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
            IsAttacking = false;
            EnableDetection();
        };
        OnTargetAcquired += () => // Step 3 : Begin Draw Arrow Animation & switch isAttacking
        {
            BeginFire();
            towerAnimation.UpdateAttackTriggerParam();
            IsAttacking = true;
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
        OnTargetSelection?.Invoke(); // STEP 2
    }// Take the first element of the list as mainTarget

    void CheckTarget()
    {
        if (Vector3.Distance(transform.position, mainTarget.transform.position) > towerRange || mainTarget.IsDead)
        {
            OnTargetExpired?.Invoke(); // CANCELATION
            return;
        }
        OnTargetAcquired?.Invoke(); // STEP 3
    }// Check that mainTarget is alive and in range

    void BeginFire()
    {
        if (!mainTarget || isAttacking) return;
        Invoke(nameof(SpawnArrow), animationDuration / fireRate);
    }// Begin animation and Invoke delayed projectile

    void SpawnArrow()
    {
        Projectiles _proj = Instantiate(projToSpawn, transform.position + transform.up * 2, transform.rotation);
        OnProjectileSpawn?.Invoke(); // STEP 4
        _proj.SetTowerTargetRef(this);
    }// Spawn projectile and end attack sequence

    void FollowTarget()
    {
        if (!mainTarget)
        {
            CancelInvoke(nameof(FollowTarget));
            return;
        }
        transform.LookAt(mainTarget.transform.position, transform.up);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, towerRange);
        Gizmos.color = Color.white;
        Gizmos.color = Color.red;
        if (mainTarget == null) return;
        Gizmos.DrawWireCube(mainTarget.AimingPoint, new Vector3(1, 1, 1));
    }
}
