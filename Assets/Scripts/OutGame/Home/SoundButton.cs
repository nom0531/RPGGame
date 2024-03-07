using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundButton : MonoBehaviour
{
    [SerializeField]
    private GameObject Animator;

    private Animator m_animator;

    private void Start()
    {
        m_animator = Animator.GetComponent<Animator>();
    }

    public void PushFlag_True()
    {
        m_animator.SetBool("IsPush", true);
    }
}
