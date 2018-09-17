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
    public float targetValue = 9.67f;
    public float[] possibleTargetValues;

    [SerializeField]
    private float savedYValue;

    public float difference;

    [SerializeField]
    private Animator myAnimatorController;

    public bool isPouring;

    public float pourMultiplier;

    public bool hasOverflowed;

    public Transform teaBase;
    
    public static GameController instance;

    public Vector3 finalPosition;
    

    private void Start()
    {
        instance = this;
        savedYValue = float.NaN;
        hasOverflowed = false;
        finalPosition = new Vector3(teaBase.transform.position.x, targetValue / 10f, teaBase.transform.position.z);
        Debug.Log(targetValue / 10f);
    }

    private void Update()
    {
        if(instance != this)
        {
            Destroy(this);
        }

        if(savedYValue != float.NaN)
        {
            difference = targetValue - savedYValue;
        }
        
        Vector3 storedTransform = teaBase.transform.localPosition;

         switch(isPouring)
        {
            case true:
            storedTransform.y += Time.deltaTime * pourMultiplier;
            teaBase.transform.localPosition = storedTransform;
            break;
            case false:
            savedYValue = storedTransform.y;
            // trackedYValue = 0f;
            break;
            default:
            break;
        }

        if(storedTransform.y > targetValue / 10f)
        {
            Debug.Log("It's over");
            teaBase.transform.localPosition = finalPosition;
            Debug.Log("Lost game");
            hasOverflowed = true;
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

    internal void ResetGame()
    {
        savedYValue = 0f;
        hasOverflowed = true;
    }
}
