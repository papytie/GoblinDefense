using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EntityWorldUI : UIWorldElement
{
    [SerializeField] Slider healthBar = null;
    Entity entityRef = null;

    protected override void Start()
    {
        base.Start();
        entityRef = GetComponent<Entity>();
    }

    protected override void Update()
    {
        base.Update();
        healthBar.value = UpdateHealthBar();
    }

    float UpdateHealthBar()
    {
        float _currentHP = entityRef.CurrentHealth;
        float _baseHP = entityRef.BaseHealth;
        return _currentHP / _baseHP;
    } //Convert int HP in float values
}
