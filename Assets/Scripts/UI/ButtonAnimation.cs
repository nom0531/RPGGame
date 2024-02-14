using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �{�^���̃X�e�[�g
/// </summary>
public enum ButtonState
{
    enIdle,     // �ҋ@���
    enPush,     // �{�^���������ꂽ
    enNotPush,  // �{�^���������Ȃ�����
}

public class ButtonAnimation : MonoBehaviour
{
    private Animator m_animator;

    // Start is called before the first frame update
    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    /// <summary>
    /// �{�^�����������Ƃ��̏���
    /// </summary>
    public void ButtonDown()
    {
        PlayAnimation(ButtonState.enPush);
    }

    /// <summary>
    /// �A�j���[�V�������Đ�����
    /// </summary>
    /// <param name="buttonState">�{�^���̃X�e�[�g</param>
    private void PlayAnimation(ButtonState buttonState)
    {
        switch (buttonState)
        {
            case ButtonState.enIdle:
                break;
            case ButtonState.enPush:
                m_animator.SetTrigger("Push");
                break;
            case ButtonState.enNotPush:
                m_animator.SetTrigger("NotPush");
                break;
        }
    }
}
