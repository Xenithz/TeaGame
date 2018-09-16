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
    public int targetValue;
    public int[] possibleTargetValues;

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

    private void SetValue(int valueToSet)
    {
        targetValue = valueToSet;
    }
}
