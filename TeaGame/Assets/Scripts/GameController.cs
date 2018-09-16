using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    prep,
    ongoing,
    done
}

public class GameController : MonoBehaviour
{
    [SerializeField]
    private State myState;
    public float targetValue;
    public float[] possibleTargetValues;

    [SerializeField]
    private float savedTime;
    
    public static GameController instance;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if(instance != this)
        {
            Destroy(this);
        }
    }

    internal void SetState(State stateToSet)
    {
        myState = stateToSet;
    }

    private void SetValue(float valueToSet)
    {
        targetValue = valueToSet;
    }

    internal void SaveTime(float timeToSave)
    {
        savedTime = timeToSave;
    }
}
