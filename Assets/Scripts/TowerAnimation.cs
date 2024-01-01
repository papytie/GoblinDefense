using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAnimation : MonoBehaviour
{
    Animator animator = null;
    Tower tower = null;

    void Start()
    {
        InitTowerAnimation();
    }

    void InitTowerAnimation()
    {
        animator = GetComponent<Animator>();
        tower = GetComponent<Tower>();
    }

    public void UpdateAttackTriggerParam()
    {
        if (!animator) return;
        animator.SetTrigger("attackTrigger"); 
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
