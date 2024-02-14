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
    /// OK�{�^���������ꂽ���̏���
    /// </summary>
    public void OKButtonDown()
    {
        m_lockOnSystem.LockOnEnd();
        m_lockOnSystem.ButtonDown = true;
    }

    /// <summary>
    /// Chancel�{�^���������ꂽ���̏���
    /// </summary>
    public void ChancelButtonDown()
    {
        m_lockOnSystem.LockOnEnd();
        m_lockOnSystem.ButtonDown = false;
    }

    /// <summary>
    /// Left�{�^���������ꂽ���̏���
    /// </summary>
    public void LeftButtonDown()
    {
        m_lockOnSystem.LookAtLeftTarget();
    }

    /// <summary>
    /// Right�{�^���������ꂽ���̏���
    /// </summary>
    public void RightButtonDown()
    {
        m_lockOnSystem.LookAtRightTarget();
    }
}
