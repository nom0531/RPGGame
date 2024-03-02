using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スプライトの表示ステート
/// </summary>
public enum SpriteState
{
    enActiveR,
    enNotActiveR,
    enActiveL,
    enNotActiveL,
}

public class StatusAnimation : MonoBehaviour
{
    private Animator m_animator;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    public void SpriteAnimaiton(SpriteState spriteState)
    {
        switch (spriteState)
        {
            case SpriteState.enActiveR:
                m_animator.SetTrigger("ActiveR");
                break;
            case SpriteState.enActiveL:
                m_animator.SetTrigger("ActiveL");
                break;
        }
    }
}
