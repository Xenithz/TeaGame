using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public static ParticleController instance;

    public ParticleSystem myParticleSystem;

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

    internal void SetEm(float floatToSet)
    {
        var emission = myParticleSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = floatToSet;
    }
}
