using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEntity : MonoBehaviour
{
    public PuzzleGate MainGate;

    public void Interact()
    {
        MouseLook.Instance.FocusOnItemInHands();
    }

    public void Release()
    {
        MouseLook.Instance.UnfocusOnItemInHands();
    }

    void PuzzleSolved()
    {
    }
}
