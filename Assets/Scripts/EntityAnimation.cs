using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimation : MonoBehaviour
{
    Animator animator = null;

    void Start()
    {
        InitEntityAnimation();
    }

    void InitEntityAnimation()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateIsDeadTriggerParam()
    {
        if (!animator) return;
        animator.SetTrigger("isDead");
    }

    public void UpdateVictoryTriggerParam()
    {
        if (!animator) return;
        animator.SetTrigger("victory");
    }
    
    public void UpdateIsWoundedBoolParam(bool _value)
    {
        if (!animator) return;
        animator.SetBool("isWounded", _value);
    }
    
    public void UpdateCanMoveBoolParam(bool _value)
    {
        if (!animator) return;
        animator.SetBool("canMove", _value);
    }

}
