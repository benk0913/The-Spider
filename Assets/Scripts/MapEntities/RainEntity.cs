using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainEntity : MonoBehaviour
{
    [SerializeField]
    List<AudioSource> AudioSources = new List<AudioSource>();

    [SerializeField]
    List<ParticleSystem> ParticleSystems  = new List<ParticleSystem>();

    [SerializeField]
    int DurationTimeMin = 0;

    [SerializeField]
    int DurationTimeMax = 500;

    [SerializeField]
    float AccelerationSpeed = 0.05f;


    public Dictionary<ParticleSystem, float> particleSystemEmissionData = new Dictionary<ParticleSystem, float>();
    
    public float Intensity
    {
        set
        {
            _intensity = Mathf.Clamp01(value);

            ParticleSystems.ForEach((x) =>
            {
                ParticleSystem.EmissionModule emissionModule = x.emission;
                emissionModule.rateOverTime = Mathf.Lerp(0f,particleSystemEmissionData[x], _intensity);
            });
        }
        get
        {
            return _intensity;
        }
    }
    float _intensity;

    private void OnEnable()
    {
        particleSystemEmissionData.Clear();
        ParticleSystems.ForEach((x) => { particleSystemEmissionData.Add(x, x.emission.rateOverTime.constant); });

        if(UpdateRoutineInstance != null)
        {
            StopCoroutine(UpdateRoutineInstance);
        }

        UpdateRoutineInstance = StartCoroutine(UpdateRoutine());
    }

    public void SetIntensity(float value)
    {
        Intensity = value;
    }

    Coroutine UpdateRoutineInstance;
    IEnumerator UpdateRoutine()
    {
        while(true)
        {
            yield return 0;

            Intensity = 0f;

            float randomTime = Random.Range(DurationTimeMin,DurationTimeMax);
            yield return new WaitForSeconds(randomTime);

            float targetIntensity = Random.Range(0.5f, 1f);
            Intensity = 0f;
            while(Intensity < targetIntensity)
            {
                Intensity += AccelerationSpeed * Time.deltaTime;

                yield return 0;
            }

            randomTime = Random.Range(DurationTimeMin, DurationTimeMax);

            yield return new WaitForSeconds(randomTime);

            while (Intensity > 0f)
            {
                Intensity -= AccelerationSpeed * Time.deltaTime;

                yield return 0;
            }


            yield return 0;
        }
    }

    private void LateUpdate()
    {
        AudioSources.ForEach((x) =>
        {
            x.volume = _intensity * AudioControl.Instance.VolumeGroups["Untagged"];
        });
    }


}
