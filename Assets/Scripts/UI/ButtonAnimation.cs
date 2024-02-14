using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボタンのステート
/// </summary>
public enum ButtonState
{
    enIdle,     // 待機状態
    enPush,     // ボタンが押された
    enNotPush,  // ボタンが押せなかった
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
