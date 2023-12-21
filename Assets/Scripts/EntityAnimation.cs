using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimation : MonoBehaviour
{

    [SerializeField] Animator animator = null;

    void Start()
    {
        InitEntityAnimation();
    }

    void Update()
    {
        
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
    
    public void UpdateIsHitTriggerParam()
    {
        if (!animator) return;
        animator.SetTrigger("isHit");
    }
    
    public void UpdateCanMoveBoolParam(bool _value)
    {
        if (!animator) return;
        animator.SetBool("canMove", _value);
    }

}
