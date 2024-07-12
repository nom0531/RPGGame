using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllOutAttackButton : MonoBehaviour
{
    private AllOutAttackSystem m_allOutAttackSystem;

    private void Start()
    {
        m_allOutAttackSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<AllOutAttackSystem>();
    }

    /// <summary>
    /// �{�^���������ꂽ�u�Ԃ̏���
    /// </summary>
    public void PushButton()
    {
        m_allOutAttackSystem.StartAllOutAttack();
    }
}
