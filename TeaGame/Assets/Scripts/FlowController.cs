﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowController : MonoBehaviour
{
    public float currentSwitchTime;
    public float switchTargetTime;

    [SerializeField]
    private float[] emValues;
    [SerializeField]
    private float[] multiplierValues;

    private PlayerControls myPlayerControls;
    private void Start()
    {
        myPlayerControls = GetComponent<PlayerControls>();
    }

    private void Update()
    {
        if (GameController.instance.isPouring == true)
        {
            Timer();
        }
    }

    /// <summary>
    /// Change flow randomly based on timer
    /// </summary>
    private void Timer()
    {
        currentSwitchTime += Time.deltaTime;
        if (currentSwitchTime >= switchTargetTime)
        {
            Debug.Log("Target time reached");
            int random = Random.Range(0, emValues.Length);
            Debug.Log("emValues[" + random + ']');
            float emToSet = emValues[random];
            Debug.Log("multiplierValues[" + random + ']');
            GameController.instance.pourMultiplier = multiplierValues[random];
            ParticleController.instance.SetEm(emToSet);
            currentSwitchTime = 0;
        }
    }
}
