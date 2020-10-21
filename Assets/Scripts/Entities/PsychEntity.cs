using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsychEntity : MonoBehaviour
{
    public List<ParticleSystem> ParticleSystems = new List<ParticleSystem>();
    public Animator Animer;

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("PsychoEffectRefresh", OnRefresh);
    }

    void OnRefresh()
    {
        Animer.SetInteger("PsychRate", CORE.Instance.PsychoEffectRate);
        Animer.SetTrigger("Update");
        if (CORE.Instance.PsychoEffectRate > 5)
        {
            foreach(ParticleSystem particles in ParticleSystems)
            {
                particles.Play();
            }
        }
        else
        {
            foreach (ParticleSystem particles in ParticleSystems)
            {
                particles.Stop();
            }
        }
    }
}
