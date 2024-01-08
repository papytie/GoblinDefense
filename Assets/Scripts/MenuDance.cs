using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDance : MonoBehaviour
{
    Animator animator = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        animator.SetTrigger("beginDance");
    }

}
