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

    public float difference;

    [SerializeField]
    private float pourTime;

    [SerializeField]
    private Animator myAnimatorController;

    public bool isPouring;

    public float pourMultiplier;
    
    public static GameController instance;

    private void Start()
    {
        instance = this;
        savedTime = float.NaN;
        pourMultiplier = 1f;
        PickTarget();
    }

    private void Update()
    {
        if(instance != this)
        {
            Destroy(this);
        }

        if(savedTime != float.NaN)
        {
            difference = targetValue - savedTime;
        }

         switch(isPouring)
        {
            case true:
            pourTime += Time.deltaTime * pourMultiplier;
            break;
            case false:
            savedTime = pourTime;
            // pourTime = 0f;
            break;
            default:
            break;
        }
    }

    private void PickTarget()
    {
        int randomInt = Random.Range(0, possibleTargetValues.Length);
        targetValue = possibleTargetValues[randomInt];
    }

    /// <summary>
    /// Function to rotate flask and start/stop particle system depending on pouring bool
    /// </summary>
    internal void PourFlask()
    {
        isPouring = !isPouring;
        Debug.Log("isPouring is: " +isPouring);
        myAnimatorController.SetBool("shouldPour", !myAnimatorController.GetBool("shouldPour"));
        
        switch(isPouring)
        {
            case true:
            StartCoroutine(StartParticleSystem());
            break;
            case false:
            ParticleController.instance.myParticleSystem.Stop();
            break;
            default:
            break;
        }
    }

    IEnumerator StartParticleSystem()
    {
        yield return new WaitForSeconds(1.75f);
        ParticleController.instance.myParticleSystem.Play();
    }

    internal void ClearSavedTime()
    {
        savedTime = 0f;
    }
}
