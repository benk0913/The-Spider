using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RavenEntity : MonoBehaviour
{
    public static RavenEntity Instance;

    public Animator Raven;

    System.Action randomizeRavenIdle;

    [SerializeField]
    Animator EntityAnimator;

    [SerializeField]
    GameObject LetterTarget;

    [SerializeField]
    GameObject RavenCollider;

    [SerializeField]
    SoundEntity RavenSoundEntity;

    public GameObject ScrollOnBird;

    bool LetterIsIdle = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        randomizeRavenIdle = () => 
        {
            if(RavenEntity.Instance == null || !RavenEntity.Instance.gameObject.activeInHierarchy)
            {
                return;
            }

            Raven.SetInteger("RandomIdle", Random.Range(0, 9));

            CORE.Instance.DelayedInvokation(Random.Range(1f, 6f), randomizeRavenIdle);
        };

        CORE.Instance.DelayedInvokation(Random.Range(1f, 6f), randomizeRavenIdle);
    }

    public void InteractWithRaven()
    {
        if (LetterIsIdle)
        {
            LetterTarget.gameObject.SetActive(true);
            ScrollOnBird.gameObject.SetActive(false);
            EntityAnimator.SetBool("IsRavenHere", false);
            LetterIsIdle = false;
            RavenCollider.gameObject.SetActive(false);
        }
    }

    public void ReceiveLetter()
    {
        LetterTarget.gameObject.SetActive(false);
        EntityAnimator.SetBool("IsRavenHere", true);
        LetterIsIdle = false;
        ScrollOnBird.SetActive(true);
    }
    
    public void LetterIsNowIdle()
    {
        LetterIsIdle = true;
        RavenCollider.gameObject.SetActive(true);
    }

    public void SetRavenFlying()
    {
        Raven.SetTrigger("ResetIdle");
        Raven.SetBool("OnGround", false);
        Raven.SetBool("GlideClosed", false);
        Raven.SetBool("Glide", false);
    }

    public void SetRavenGliding()
    {
        if (Random.Range(0, 2) == 0)
        {
            Raven.SetBool("Glide", true);
            Raven.SetBool("GlideClosed", false);
        }
        else
        {
            Raven.SetBool("Glide", false);
            Raven.SetBool("GlideClosed", true);
        }
    }

    public void SetRavenTurnLeft()
    {
        Raven.SetTrigger("TurnLeft");
    }

    public void SetRavenTurnRight()
    {
        Raven.SetTrigger("TurnRight");
    }

    public void SetRavenGround()
    {
        Raven.SetBool("OnGround", true);
    }

    public void RavenPlaySound(string key)
    {
        RavenSoundEntity.PlaySound(key);
    }
}
