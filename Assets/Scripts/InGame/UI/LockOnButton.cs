using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnButton : MonoBehaviour
{
    private LockOnSystem m_lockOnSystem;

    private void Awake()
    {
        m_lockOnSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<LockOnSystem>();
    }

    /// <summary>
    /// OKボタンが押された時の処理
    /// </summary>
    public void OKButtonDown()
    {
        m_lockOnSystem.LockOnEnd();
        m_lockOnSystem.ButtonDown = true;
    }

    /// <summary>
    /// Chancelボタンが押された時の処理
    /// </summary>
    public void ChancelButtonDown()
    {
        m_lockOnSystem.LockOnEnd();
        m_lockOnSystem.ButtonDown = false;
    }

    /// <summary>
    /// Leftボタンが押された時の処理
    /// </summary>
    public void LeftButtonDown()
    {
        m_lockOnSystem.LookAtLeftTarget();
    }

    /// <summary>
    /// Rightボタンが押された時の処理
    /// </summary>
    public void RightButtonDown()
    {
        m_lockOnSystem.LookAtRightTarget();
    }
}
