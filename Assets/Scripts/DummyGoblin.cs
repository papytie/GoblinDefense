using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class DummyGoblin : MonoBehaviour
{
    [Header("Dummy Goblin Settings")]
    [SerializeField] float maxTime = 1;
    [SerializeField] float minRandTime = 2;
    [SerializeField] float maxRandTime = 4;
    [SerializeField] List<string> paramsList = new();

    Animator animator = null;
    
    float currentTime = 0;

    //event Action OnAnimationPlayed = null; //not used
    event Action OnTimerEnd = null;

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
    } //set refs and event
    
    void ChangeMaxTime()
    {
        maxTime = Random.Range(minRandTime, maxRandTime);
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
    } //Timer with changing random max value

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
