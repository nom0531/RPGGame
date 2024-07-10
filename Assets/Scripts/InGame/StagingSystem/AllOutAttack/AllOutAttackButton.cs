using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllOutAttackButton : MonoBehaviour
{
    private AllOutAttackSystem m_allOutAttackSystem;
    private bool m_activeFlag = false;

    private void OnEnable()
    {
        if(m_activeFlag == true)
        {
            return;
        }
        GetComponent<Animator>().SetTrigger("Active");
        m_allOutAttackSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<AllOutAttackSystem>();
        m_activeFlag = true;
    }

    /// <summary>
    /// É{É^ÉìÇ™âüÇ≥ÇÍÇΩèuä‘ÇÃèàóù
    /// </summary>
    public void PushButton()
    {
        m_allOutAttackSystem.StartAllOutAttack();
    }
}
