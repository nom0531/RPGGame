using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    /// <summary>
    /// �{�^���̃X�e�[�g
    /// </summary>
    private enum ButtonState
    {
        enPush,     // �{�^���������ꂽ
    }

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
            case ButtonState.enPush:
                m_animator.SetTrigger("Push");
                break;
            default:
                break;
        }
    }
}
