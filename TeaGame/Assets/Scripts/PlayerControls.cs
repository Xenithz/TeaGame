using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControls : MonoBehaviour
{
    private Transform flaskTransform;

    private bool isPouring;

    private int touchCount;

    private void Start()
    {
        flaskTransform = this.transform;
    }

    private void Update()
    {
        //Mobile
        for(int i = 0; i < Input.touchCount; i++)
        {
            if(Input.GetTouch(i).phase == TouchPhase.Began)
            {
                GameController.instance.PourFlask();
            }
        }
        
        //PC Test
        if(Input.GetKeyDown(KeyCode.A))
        {
            GameController.instance.PourFlask();
        }
    }
}
