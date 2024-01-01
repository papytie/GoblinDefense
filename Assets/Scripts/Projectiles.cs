using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] float moveSpeed = 10;
    [SerializeField] float impactDist = 2;
    [SerializeField] int damageValue = 1;

    Entity target = null;
    Tower towerRef = null;

    void Update()
    {
        MovementBehaviour();
    }

    public void SetTowerTargetRef(Tower _tower) 
    {
        towerRef = _tower;
        target = towerRef.MainTarget;
    } // Used in Tower class to give MainTarget Entity

    void MovementBehaviour()
    {
        if (target == null || target.IsDead) 
        {
            Destroy(gameObject);
            return;
        }

        // Using AimingPoint instead of base transform.position to improve projectile trajectory
        transform.position += (target.AimingPoint - transform.position).normalized * moveSpeed * Time.deltaTime; 
        transform.LookAt(target.AimingPoint);
        
        float _dist = Vector3.Distance(target.AimingPoint, transform.position);
        if (_dist < impactDist)
        {
            target.TakeDamage(damageValue);
            Destroy(gameObject);
        }

    } // Destroy this if target is dead

    /*void MoveObjectToEntityPosition(Vector3 _target)
    {
        // Using AimingPoint instead of base transform.position to improve projectile trajectory
        transform.position += (_target - transform.position).normalized * moveSpeed * Time.deltaTime; 
    }
    void CheckDist()
    {
        float _dist = Vector3.Distance(target.AimingPoint, transform.position);
        if (_dist < impactDist)
        {
            target.TakeDamage(damageValue);
            Destroy(gameObject);
        }
    }*/
}
