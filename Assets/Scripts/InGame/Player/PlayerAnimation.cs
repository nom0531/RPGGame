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
    enDamage_Down,
    enDamage_Up,
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
        if(m_animator == null)
        {
            m_animator = GetComponent<Animator>();
        }
        // 再生処理
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
            case AnimationState.enDamage_Down:
                m_animator.SetTrigger("Down");
                break;
            case AnimationState.enDamage_Up:
                m_animator.SetTrigger("Up");
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
