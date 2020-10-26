using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TouchColliderEntity : MonoBehaviour
{
    public AudioSource SourceOne;
    public AudioSource SourceTwo;

    public float FadeSpeed = 1f;

    private void OnCollisionEnter(Collision collision)
    {
        StopAllCoroutines();
        StartCoroutine(SourceSwitch(true));
    }

    private void OnCollisionExit(Collision collision)
    {
        StopAllCoroutines();
        StartCoroutine(SourceSwitch(false));
    }

    IEnumerator SourceSwitch(bool toTwo = true)
    {
        float t = 0f;
        if (toTwo)
        {
            while (t < 1f)
            {
                t += FadeSpeed * Time.deltaTime;


                SourceOne.volume = Mathf.Lerp(SourceOne.volume, 0f,t);
                SourceTwo.volume = Mathf.Lerp(SourceTwo.volume, 1f, t);

                yield return 0;
            }
        }
        else
        {
            while (t < 1f)
            {
                t += FadeSpeed * Time.deltaTime;


                SourceOne.volume = Mathf.Lerp(SourceOne.volume, 1f, t);
                SourceTwo.volume = Mathf.Lerp(SourceTwo.volume, 0f, t);

                yield return 0;
            }
        }
    }


}
