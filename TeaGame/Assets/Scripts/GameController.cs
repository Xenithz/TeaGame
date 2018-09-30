using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDX.Network;

public class GameController : MonoBehaviour
{

    public float CurrentScore 
    {
        get {return savedYValue; }
    }

    public float targetValue = 9.67f;
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

    public bool isTracking;

    public int inputCount;
    
    public bool isGamePlaying = false;
    [SerializeField]
    Vector3 storedTransform;
    [SerializeField]
    ParticleSystem OverflowParticles;

    private void Start()
    {
        inputCount = 0;
        instance = this;
        savedYValue = float.NaN;
        hasOverflowed = false;
        finalPosition = new Vector3(teaBase.transform.position.x, targetValue / 10f, teaBase.transform.position.z);
        storedTransform = teaBase.transform.localPosition;
    }

    private void Update()
    {
        if(instance != this)
        {
            Destroy(this);
        }

        if(!isGamePlaying)
            return;
        
        UpdateMovables();
        if(!PhotonNetwork.connected)
        {
            // storedTransform = teaBase.transform.localPosition;

            if(isPouring)
            {

                storedTransform.y += Time.deltaTime * pourMultiplier;
                Debug.Log("Stored transform is " + storedTransform.y);

            }
            else
            {

                savedYValue = storedTransform.y;

                Debug.Log("Saved Y value is: " + savedYValue);

            }
        }

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetTeaBasePosition(float y)
    {
        Vector3 teaPos = teaBase.transform.localPosition;
        Vector3 TempVect = new Vector3(teaPos.x,
        y, teaPos.z);
        if(TempVect.y > 1f)
        {
            TempVect.y = 1f;
        }
        teaBase.transform.localPosition = TempVect;

    }

    private void SingleplayerFinishStateUpdate(Vector3 storedTransform)
    {
        // Overflow
        if(storedTransform.y > targetValue / 10f)
        {
            Debug.Log("It's over");
            teaBase.transform.localPosition = finalPosition;
            hasOverflowed = true;

            CanvasManager.instance.ActivatePanel(CanvasManager.instance.LoseScreen);

            if(!PhotonNetwork.connected)
            {
                CanvasManager.instance.SecondaryTextByPanel(CanvasManager.instance.LoseScreen).text = "Reset Game";
                CanvasManager.instance.ActiveButtonByPanel(CanvasManager.instance.LoseScreen, 0).onClick.AddListener(ResetGame);
            }
       }

        // Win with Range
        if(savedYValue >= 0.72f && savedYValue <= targetValue /10f)
        {
            Debug.Log("You win");
            CanvasManager.instance.ActivatePanel(CanvasManager.instance.WinScreen);

            CanvasManager.instance.SecondaryTextByPanel(CanvasManager.instance.WinScreen).text = "Reset Game";
            CanvasManager.instance.ActiveButtonByPanel(CanvasManager.instance.WinScreen, 0).onClick.AddListener(ResetGame);
            
        }

        // Lose with too low of a value
        if(savedYValue < 0.72f && inputCount >= 2)
        {
            CanvasManager.instance.ActivatePanel(CanvasManager.instance.LoseScreen);

            CanvasManager.instance.SecondaryTextByPanel(CanvasManager.instance.LoseScreen).text = "Reset Game";
            CanvasManager.instance.ActiveButtonByPanel(CanvasManager.instance.LoseScreen, 0).onClick.AddListener(ResetGame);
            
        }

    }

    /// <summary>
    /// Function to rotate flask and start/stop particle system depending on pouring bool
    /// </summary>
    public void PourFlask()
    {
        if(isTracking == true && inputCount < 2)
        {
            inputCount++;

            if(inputCount == 1)
                isPouring = true;
            
            Debug.Log("isPouring is: " +isPouring);
            if(inputCount >= 2)
            {
                isGamePlaying = false;
                savedYValue = storedTransform.y;
                
                if(myAnimatorController.GetBool("shouldPour"))
                    myAnimatorController.SetBool("shouldPour", false);
                isPouring = false;

                SingleplayerFinishStateUpdate(storedTransform);
                inputCount++;

            }
            
        }
    }

    public void UpdateMovables()
    {
        if(IsSomeoneCurrentlyPouring())
        {
            // if(!ParticleController.instance.myParticleSystem.isPlaying)
            //     StartCoroutine(StartParticleSystem());
            if(!teaBase.gameObject.activeSelf)
            {
                teaBase.gameObject.SetActive(true);
            }

            
            if(!myAnimatorController.GetBool("shouldPour"))
                myAnimatorController.SetBool("shouldPour", true);
            
            if(GetYValueOfTurner() > targetValue / 10f)
            {
                hasOverflowed = true;
                OverflowParticles.gameObject.SetActive(true);
                OverflowParticles.Play();
            }
            else
            {
                hasOverflowed = false;
                OverflowParticles.gameObject.SetActive(false);
                
                OverflowParticles.Stop();                
            }

            SetTeaBasePosition(GetYValueOfTurner());

        }
        else
        {
            if(GetYValueOfTurner() > ((targetValue / 10f) - 0.01f))
            {
                hasOverflowed = true;
                OverflowParticles.gameObject.SetActive(true);

                OverflowParticles.Play();
            }
            else
            {
                hasOverflowed = false;
                OverflowParticles.gameObject.SetActive(false);

                OverflowParticles.Stop();                
            }


            //ParticleController.instance.myParticleSystem.Stop();

            if(teaBase.gameObject.activeSelf)
            {
                teaBase.gameObject.SetActive(false);
            }
            
            if(myAnimatorController.GetBool("shouldPour"))
                myAnimatorController.SetBool("shouldPour", false);

            SetTeaBasePosition(0);
        }
    }

    public bool IsSomeoneCurrentlyPouring()
    {
        if(PhotonNetwork.connected && isGamePlaying)
        {
            if(NetworkManager.Instance.LocalPlayer.IsPlayerPouring)
            {
                return true;
            }
            else if(NetworkManager.Instance.RemotePlayer.IsPlayerPouring)
            {
                return true;
            }
            else
                return false;
        }
        else if(!PhotonNetwork.connected && isGamePlaying)
        {
            return isPouring;
        }
        else
        {
            return false;
        }
    }

    public float GetYValueOfTurner()
    {
        if(PhotonNetwork.connected)
        {
            if(NetworkManager.Instance.LocalPlayer.IsPlayerPouring)
            {
                return NetworkManager.Instance.LocalPlayer.TeaLevel;
            }
            else if(NetworkManager.Instance.RemotePlayer.IsPlayerPouring)
            {
                return NetworkManager.Instance.RemotePlayer.TeaLevel;

            }
            else
                return 0;
        }
        else
        {
            return storedTransform.y;
        }
    
    }

    // IEnumerator StartParticleSystem()
    // {
    //     yield return new WaitForSeconds(1.75f);

    //     if(!ParticleController.instance.myParticleSystem.isPlaying)
    //         ParticleController.instance.myParticleSystem.Play();
    //     else
    //         ParticleController.instance.myParticleSystem.Stop();
        
    // }

    public void ResetGame()
    {
        inputCount = 0;
        savedYValue = 0f;
        hasOverflowed = false;
        OverflowParticles.gameObject.SetActive(false);
        
        OverflowParticles.Stop();
        storedTransform = Vector3.zero;
        
        teaBase.transform.localPosition = new Vector3(teaBase.transform.localPosition.x, -0.284f, teaBase.transform.localPosition.z);
        // UIController.instance.DisableResultPanel();
        if(PhotonNetwork.connected)
        {

        }
        else
            CanvasManager.instance.ActivateSingleplayerUI();
    }
}
