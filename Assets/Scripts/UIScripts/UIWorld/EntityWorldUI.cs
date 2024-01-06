using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EntityWorldUI : MonoBehaviour
{
    [SerializeField] Slider healthBar = null;
    [SerializeField] Canvas canvas = null;
    Camera mainCamera = null;
    Entity entityRef = null;

    void Start()
    {
        entityRef = GetComponent<Entity>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        healthBar.value = UpdateHealthBar();
        canvas.transform.LookAt(mainCamera.transform.position); //Always Look at mainCamera
    }

    float UpdateHealthBar()
    {
        float _currentHP = entityRef.CurrentHealth;
        float _baseHP = entityRef.BaseHealth;
        return _currentHP / _baseHP;
    } //Convert int HP in float values
}
