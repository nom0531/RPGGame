using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerSkillButton : MonoBehaviour
{
    private PlayerSkill m_playerSkill;
    // �X�L���̔ԍ�
    private int m_skillNumber = 0;

    public int SkillNumber
    {
        set => m_skillNumber = value;
    }

    /// <summary>
    /// �������p�̊֐��B�X�L����o�^����
    /// </summary>
    /// <param name="number">�X�L���̔ԍ�</param>
    /// <param name="skillName">�X�L���̖��O</param>
    /// <param name="fontColor">�����̐F</param>
    public void SetPlayerSkill(int number, string skillName, PlayerSkill playerSkill)
    {
        // ���ꂼ��̒l��o�^����
        m_skillNumber = number;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = skillName;
        // �}�ӃV�X�e����o�^����
        m_playerSkill = playerSkill;
    }

    private void Awake()
    {
        m_playerSkill =
            GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<PlayerSkill>();
    }

    /// <summary>
    /// �{�^���������ꂽ���̏���
    /// </summary>
    public void ButtonDown()
    {
        // �ڍׂ�\������
        m_playerSkill.DisplaySetPlayerSkill(m_skillNumber);
    }
}
