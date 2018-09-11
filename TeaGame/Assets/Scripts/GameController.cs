using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    assigned,
    notAssigned
}

public class GameController : MonoBehaviour
{
    [SerializeField]
    private State myState;
    public int targetValue;
    public int[] possibleTargetValues;

    internal void SetState(State stateToSet)
    {
        myState = stateToSet;
    }

    private void SetValue(int valueToSet)
    {
        targetValue = valueToSet;
    }
}
