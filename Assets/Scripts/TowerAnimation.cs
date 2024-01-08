using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAnimation : MonoBehaviour
{
    Animator animator = null;
    Tower tower = null;

    private void Awake()
    {
        InitTowerAnimation();
        
    }

    void Start()
    {
    }

    void InitTowerAnimation()
    {
        animator = GetComponent<Animator>();
        tower = GetComponent<Tower>();
    }

    public void ToggleAttackTriggerParam()
    {
        if (!animator) return;
        animator.SetTrigger("attackTrigger"); 
    }

    public void ToggleDetectionTriggerParam()
    {
        if (!animator) return;
        animator.SetTrigger("detectionTrigger"); 
    }

    public void ToggleDeathTriggerParam()
    {
        if (!animator) return;
        animator.SetTrigger("deathTrigger"); 
    }

    public void UpdateIsAttackingBoolParam(bool _value)
    {
        if (!animator) return;
        animator.SetBool("isAttacking", _value);
    } //Param used for canceling attack anim

    public void UpdateFireRateFloatValue(float _value)
    {
        if (!animator) return;
        animator.SetFloat("fireRate", _value);
    } //Param used to sync fire rate speed and anim speed
}
