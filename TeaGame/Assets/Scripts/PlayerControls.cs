using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlaskState
{
    notPouring,
    pouring
}

public class PlayerControls : MonoBehaviour
{
    private Transform flaskTransform;

    [SerializeField]
    private FlaskState currentState;

    private Animator myAnimatorController;

    private int touchCount;

    [SerializeField]
    private float pourTime;

    public float pourMultiplier;

    private void Start()
    {
        flaskTransform = this.transform;
        currentState = FlaskState.notPouring;
        myAnimatorController = GetComponent<Animator>();
        pourMultiplier = 1f;
    }

    private void Update()
    {
        for(int i = 0; i < Input.touchCount; i++)
        {
            if(Input.GetTouch(i).phase == TouchPhase.Began && currentState == FlaskState.notPouring)
            {
                myAnimatorController.SetBool("shouldPour", true);
                currentState = FlaskState.pouring;
            }
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            myAnimatorController.SetBool("shouldPour", !myAnimatorController.GetBool("shouldPour"));
            
            if(currentState == FlaskState.notPouring)
            {
                currentState = FlaskState.pouring;
            }
            else
            {
                currentState = FlaskState.notPouring;
            }
        }

        switch(currentState)
        {
            case FlaskState.pouring:
            pourTime += Time.deltaTime;
            break;
            case FlaskState.notPouring:
            GameController.instance.SaveTime(pourTime);
            pourTime = 0f;
            break;
            default:
            break;
        }
    }
}
