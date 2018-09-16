using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private Transform flaskTransform;
    [SerializeField]
    private Quaternion targeteRotation;

    private void Start()
    {
        flaskTransform = this.transform;
    }

    private void Update()
    {
        for(int i = 0; i < Input.touchCount; i++)
        {
            if(Input.GetTouch(i).phase == TouchPhase.Began)
            {

            }
        }
    }

    private void StartPour()
    {

    }
}
