using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSwitchGalleryUI : MonoBehaviour
{

    [SerializeField]
    Image ImageA;

    [SerializeField]
    Image ImageB;

    [SerializeField]
    float FadeSpeed = 1f;

    [SerializeField]
    float StayDuration = 3f;

    [SerializeField]
    List<Sprite> Gallery = new List<Sprite>();

    private void OnEnable()
    {
        if(EffectRoutineInstance != null)
        {
            StopCoroutine(EffectRoutineInstance);
        }

        EffectRoutineInstance = StartCoroutine(EffectRoutine());
    }

    Coroutine EffectRoutineInstance;

    public IEnumerator EffectRoutine()
    {
        ImageA.color = Color.white;
        ImageB.color = Color.clear;
        float t = 0f;


        ImageA.sprite = Gallery[Random.Range(0, Gallery.Count)];
        ImageB.sprite = Gallery[Random.Range(0, Gallery.Count)];


        while (true)
        {
            yield return 0;

            t = 0f;
            while(t<0.5f)
            {
                t += Time.deltaTime * FadeSpeed;

                ImageA.color = Color.Lerp(Color.white, Color.clear, t);
                ImageB.color = Color.Lerp(Color.clear, Color.white, t);

                yield return 0;
            }

            
            ImageA.sprite = Gallery[Random.Range(0, Gallery.Count)];
            ImageB.sprite = Gallery[Random.Range(0, Gallery.Count)];

            while (t < 1f)
            {
                t += Time.deltaTime * FadeSpeed;

                ImageA.color = Color.Lerp(Color.white, Color.clear, t);
                ImageB.color = Color.Lerp(Color.clear, Color.white, t);

                yield return 0;
            }

            yield return new WaitForSeconds(StayDuration);

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * FadeSpeed;

                ImageB.color = Color.Lerp(Color.white, Color.clear, t);
                ImageA.color = Color.Lerp(Color.clear, Color.white, t);

                yield return 0;
            }

            yield return new WaitForSeconds(StayDuration);
        }
    }
}
