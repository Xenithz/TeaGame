using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowController : MonoBehaviour
{
    public float currentTime;
    public float targetTime;
    public bool shouldTime;
    public float[] emValues;
    private ParticleController myController;

    private void Start()
    {
        myController = FindObjectOfType<ParticleController>();
    }

    private void Update()
    {
        if (shouldTime == true)
        {
            Timer();
        }
    }

    private void Timer()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= targetTime)
        {
            Debug.Log("Target time reached");
            int random = Random.Range(0, emValues.Length);
            float emToSet = emValues[random];
            myController.SetEm(emToSet);
        }
    }
}
