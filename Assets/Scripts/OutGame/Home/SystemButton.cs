using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemButton : MonoBehaviour
{
    [SerializeField,Tooltip("0は自身のAnimator,1は相手のAnimator")]
    private GameObject[] Animators;

    public void PushFlag_True()
    {
        Animators[0].GetComponent<Animator>().SetBool("IsPush", true);
    }

    public void PushFlag_False()
    {
        Animators[1].GetComponent<Animator>().SetBool("IsPush", false);
    }
}
