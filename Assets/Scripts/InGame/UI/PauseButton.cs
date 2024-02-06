using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    private BattleManager m_battleManager;

    private void Start()
    {
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
    }

    /// <summary>
    /// �{�^���������ꂽ���̏���
    /// </summary>
    public void ButtonDown()
    {
        m_battleManager.PauseFlag = true;
    }

    /// <summary>
    /// �Q�[���ɖ߂�{�^���������ꂽ���̏���
    /// </summary>
    public void ReturnGameButtonDown()
    {
        m_battleManager.PauseFlag = false;
    }
}