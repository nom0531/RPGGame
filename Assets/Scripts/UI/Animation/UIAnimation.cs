using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    /// <summary>
    /// �A�j���[�V�����̃X�e�[�g
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
        m_animator = AnimatorGameObject.GetComponent<Animator>();   // Animator���Q��
    }

    /// <summary>
    /// �{�^�����������Ƃ��̏���
    /// </summary>
    public void ButtonDown_Active()
    {
        PlayAnimation(AnimationState.enActive);     // �\������
    }

    /// <summary>
    /// �{�^�����������Ƃ��̏���
    /// </summary>
    public void ButtonDown_NotActive()
    {
        PlayAnimation(AnimationState.enNotActive);  // ��\���ɂ���
    }

    /// <summary>
    /// �A�j���[�V�������Đ�����
    /// </summary>
    /// <param name="animationState">�A�j���[�V�����̃X�e�[�g</param>
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
