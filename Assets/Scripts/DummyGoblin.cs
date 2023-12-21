using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class DummyGoblin : MonoBehaviour
{
    public event Action OnAnimationPlayed = null;
    public event Action OnTimerEnd = null;

    [SerializeField] Animator animator = null;

    [SerializeField] float currentTime = 0;
    [SerializeField] float maxTime = 1;

    [SerializeField] List<string> paramsList = new();
    void Start()
    {
        InitDummyGoblin();
    }

    void Update()
    {
        RandomTimer();
    }

    void InitDummyGoblin()
    {
        animator = GetComponent<Animator>();
        OnTimerEnd += ChangeMaxTime;
    }
    
    void ChangeMaxTime()
    {
        maxTime = Random.Range(2f, 4f);
    }

    void RandomTimer()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= maxTime) 
        {
            currentTime = 0;
            ChooseRandomAnimInList();
            OnTimerEnd?.Invoke();
        }
    }

    void ChooseRandomAnimInList()
    {
        int _randomIndex = Random.Range(0, paramsList.Count);
        UpdateParamTrigger(paramsList[_randomIndex]);
    }

    void UpdateParamTrigger(string _paramName)
    {
        if (!animator) return;
        animator.SetTrigger(_paramName);
    }
}
