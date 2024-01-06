using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWorldElement : MonoBehaviour
{
    [SerializeField] protected Canvas canvas = null;
    [SerializeField] protected Camera mainCamera = null;

    protected virtual void Start()
    {
        mainCamera = Camera.main;

    }

    protected virtual void Update()
    {
        canvas.transform.LookAt(mainCamera.transform.position); //Always Look at mainCamera

    }
}
