using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationEntity : MonoBehaviour
{
    [SerializeField]
    float rotationSpeed = 1f;

    [SerializeField]
    List<RotationInstance> Presets = new List<RotationInstance>();

    public void RotateToPreset(string target)
    {
        if(RotationRoutineInstance != null)
        {
            StopCoroutine(RotationRoutineInstance);
        }

        RotationRoutineInstance = StartCoroutine(RotationRoutine(GetPreset(target).targetRotationEuler));
    }

    public void RotateToPresetInstant(string target)
    {
        transform.rotation = Quaternion.Euler(GetPreset(target).targetRotationEuler);
    }

    Coroutine RotationRoutineInstance;
    IEnumerator RotationRoutine(Vector3 targetEuler)
    {
        float t = 0f;
        while(t<1f)
        {
            t += rotationSpeed * Time.deltaTime;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetEuler), t);

            yield return 0;
        }


        RotationRoutineInstance = null;
    }

    public RotationInstance GetPreset(string key)
    {
        for(int i=0;i<Presets.Count;i++)
        {
            if(Presets[i].Key == key)
            {
                return Presets[i];
            }
        }

        return null;
    }

    



    [System.Serializable]
    public class RotationInstance
    {
        public string Key;
        public Vector3 targetRotationEuler;
    }
}
