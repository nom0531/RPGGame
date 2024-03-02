using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アニメーションステート
/// </summary>
public enum AnimationState
{
    enIdle,
    enAttack,
    enSkillAttack,
    enGurad,
    enDamage,
    enWin,
    enLose,
}

public class PlayerAnimation : MonoBehaviour
{
    private Animator m_animator;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    /// <summary>
    /// アニメーションを再生
    /// </summary>
    /// <param name="animationState">アニメーションステート</param>
    public void PlayAnimation(AnimationState animationState)
    {
        switch (animationState)
        {
            case AnimationState.enIdle:
                break;
            case AnimationState.enAttack:
                m_animator.SetTrigger("Attack");
                break;
            case AnimationState.enSkillAttack:
                m_animator.SetTrigger("SkillAttack");
                break;
            case AnimationState.enGurad:
                break;
            case AnimationState.enDamage:
                m_animator.SetTrigger("Damage");
                break;
            case AnimationState.enWin:
                m_animator.SetTrigger("Win");
                break;
            case AnimationState.enLose:
                m_animator.SetTrigger("Lose");
                break;
        }
    }
}
