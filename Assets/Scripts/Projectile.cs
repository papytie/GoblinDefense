using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [SerializeField] Entity target = null;
    [SerializeField] Tower towerRef = null;

    [SerializeField] float moveSpeed = 10;
    [SerializeField] float impactDist = 2;
    [SerializeField] int damageValue = 1;

    void Start()
    {
       
    }

    void Update()
    {
        MovementBehaviour();
    }

    public void SetTowerTargetRef(Tower _tower) // Used in Tower class to give MainTarget Entity
    {
        towerRef = _tower;
        target = towerRef.MainTarget;
    }
    void MovementBehaviour()
    {
        if (target == null || target.IsDead) // Destroy this if target is dead
        {
            Destroy(gameObject);
            return;
        }
        transform.position += (target.AimingPoint - transform.position).normalized * moveSpeed * Time.deltaTime; // Using AimingPoint instead of base transform.position to improve projectile trajectory
        transform.LookAt(target.AimingPoint);
        float _dist = Vector3.Distance(target.AimingPoint, transform.position);
        if (_dist < impactDist)
        {
            target.TakeDamage(damageValue);
            Destroy(gameObject);
        }
        
    }

    void MoveObjectToEntityPosition(Vector3 _target)
    {
        // Using AimingPoint instead of base transform.position to improve projectile trajectory
        transform.position += (_target - transform.position).normalized 
                               * moveSpeed 
                               * Time.deltaTime; 
    }
    void CheckDist()
    {
        float _dist = Vector3.Distance(target.AimingPoint, transform.position);
        if (_dist < impactDist)
        {
            target.TakeDamage(damageValue);
            Destroy(gameObject);
        }
    }
}
