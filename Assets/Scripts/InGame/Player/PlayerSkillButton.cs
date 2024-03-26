using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerSkillButton : MonoBehaviour
{
    private PlayerSkill m_playerSkill;
    // スキルの番号
    private int m_skillNumber = 0;

    public int SkillNumber
    {
        set => m_skillNumber = value;
    }

    /// <summary>
    /// 初期化用の関数。スキルを登録する
    /// </summary>
    /// <param name="number">スキルの番号</param>
    /// <param name="skillName">スキルの名前</param>
    /// <param name="fontColor">文字の色</param>
    public void SetPlayerSkill(int number, string skillName, PlayerSkill playerSkill)
    {
        // それぞれの値を登録する
        m_skillNumber = number;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = skillName;
        // 図鑑システムを登録する
        m_playerSkill = playerSkill;
    }

    private void Awake()
    {
        m_playerSkill =
            GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<PlayerSkill>();
    }

    /// <summary>
    /// ボタンを押された時の処理
    /// </summary>
    public void ButtonDown()
    {
        // 詳細を表示する
        m_playerSkill.DisplaySetPlayerSkill(m_skillNumber);
    }
}
