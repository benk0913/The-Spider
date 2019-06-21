using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterDispenserEntity : MonoBehaviour
{
    [SerializeField]
    GameObject LetterPrefab;

    [SerializeField]
    Transform targetPoint;

    [SerializeField]
    float DispensingSpeed = 1f;


    //TEST
    public Letter TEST_LETTER;
    public bool TEST;
    public bool TEST2;

    private void Update()
    {
        if(TEST)
        {
            DispenseLetter(TEST_LETTER);
            TEST = false;
        }

        if (TEST2)
        {
            DispenseLetters(new Letter[]{ TEST_LETTER,TEST_LETTER,TEST_LETTER,TEST_LETTER });
            TEST2 = false;
        }
    }
    ///

    public void DispenseLetters(Letter[] letters)
    {
        for(int i=0;i<letters.Length;i++)
        {
            StartCoroutine(DispenseLetterRoutine(GenerateLetter(letters[i]).transform, i));
        }
    }

    public void DispenseLetter(Letter letter)
    {
        StartCoroutine(DispenseLetterRoutine(GenerateLetter(letter).transform));
    }

    IEnumerator DispenseLetterRoutine(Transform letterTransform, float addedY = 0f)
    {
        yield return 0;

        Quaternion targetRotation = Quaternion.Euler(letterTransform.rotation.x+90f, letterTransform.rotation.y, letterTransform.rotation.z+ Random.Range(0f, 180f));

        float t = 0f;
        while(t<1f)
        {
            t += DispensingSpeed * Time.deltaTime;

            letterTransform.position = Vector3.Lerp(letterTransform.position, targetPoint.position +new Vector3(0f,addedY/100f,0f), t);
            letterTransform.rotation = Quaternion.Lerp(letterTransform.rotation, targetRotation, t);

            yield return 0;
        }
    }

    GameObject GenerateLetter(Letter letter)
    {
        GameObject tempLetter = Instantiate(LetterPrefab);
        tempLetter.transform.position = transform.position;
        tempLetter.transform.rotation = transform.rotation;
        tempLetter.GetComponent<EnvelopeEntity>().SetInfo(letter);

        return tempLetter;
    }


}
