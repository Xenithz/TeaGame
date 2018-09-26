using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowController : MonoBehaviour
{
    public float currentSwitchTime;
    public float switchTargetTime;

    public float emMultiplier;

    public float maximumRange;

    private PlayerControls myPlayerControls;
    public float multiply;

    private void Start()
    {
        myPlayerControls = GetComponent<PlayerControls>();
    }

    private void Update()
    {
        if (GameController.instance.IsSomeoneCurrentlyPouring() == true)
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
            float random = Random.Range(0, maximumRange);
            float emToSet = random * emMultiplier;
            GameController.instance.pourMultiplier = random * multiply;

            // if(!PhotonNetwork.connected)
            //     ParticleController.instance.SetEm(emToSet);
                
            currentSwitchTime = 0;
        }
    }
}
