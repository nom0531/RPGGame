using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    /// <summary>
    /// アニメーションのステート
    /// </summary>
    private enum AnimationState
    {
        enActive,
        enNotActive,
    }

    [SerializeField]
    private GameObject AnimatorGameObject;
    private Animator m_animator;

    private void Start()
    {
        m_animator = AnimatorGameObject.GetComponent<Animator>();   // Animatorを参照
    }

    /// <summary>
    /// ボタンを押したときの処理
    /// </summary>
    public void ButtonDown_Active()
    {
        PlayAnimation(AnimationState.enActive);     // 表示する
    }

    /// <summary>
    /// ボタンを押したときの処理
    /// </summary>
    public void ButtonDown_NotActive()
    {
        PlayAnimation(AnimationState.enNotActive);  // 非表示にする
    }

    /// <summary>
    /// アニメーションを再生する
    /// </summary>
    /// <param name="animationState">アニメーションのステート</param>
    private void PlayAnimation(AnimationState animationState)
    {
        switch (animationState)
        {
            case AnimationState.enActive:
                m_animator.SetTrigger("Active");
                break;
            case AnimationState.enNotActive:
                m_animator.SetTrigger("NotActive");
                break;
            default:
                break;
        }
    }
}
