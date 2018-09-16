using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem myParticleSystem;
    public List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
    public int amountOfParticlesDetected;

    private void OnParticleTrigger()
    {
        int numEnter = myParticleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        for (int i = 0; i < numEnter; i++)
        {
            amountOfParticlesDetected++;
        }
    }

    internal void SetEm(float floatToSet)
    {
        var emission = myParticleSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = floatToSet;
    }
}
