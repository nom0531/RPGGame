using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    /// <summary>
    /// ボタンのステート
    /// </summary>
    private enum ButtonState
    {
        enPush,     // ボタンが押された
    }

    private Animator m_animator;

    // Start is called before the first frame update
    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    /// <summary>
    /// ボタンを押したときの処理
    /// </summary>
    public void ButtonDown()
    {
        PlayAnimation(ButtonState.enPush);
    }

    /// <summary>
    /// アニメーションを再生する
    /// </summary>
    /// <param name="buttonState">ボタンのステート</param>
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
