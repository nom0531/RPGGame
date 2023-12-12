using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class QuestButton : MonoBehaviour
{
    private const int STRING_MAX = 5; // �����̍ő�\����

    private QuestStatusSystem m_questStatusSystem;
    private int m_myNumber = -1;     // �N�G�X�g�̔ԍ�

    /// <summary>
    /// �������p�̊֐��B�N�G�X�g��o�^����
    /// </summary>
    /// <param name="number">�N�G�X�g�̔ԍ�</param>
    /// <param name="questName">�N�G�X�g�̖��O</param>
    /// <param name="fontColor">�����̐F</param>
    public void SetQuestStatus(int number,string questName,Color fontColor,QuestStatusSystem questStatusSystem)
    {
        // ���������ȏ�Ȃ�
        if (questName.Length >= STRING_MAX)
        {
            // �ꕔ�̕������ȗ�����
            questName = questName.Substring(0, STRING_MAX);
            questName = $"�w{questName}�c";
        }
        else
        {
            questName = $"�w{questName}�x";
        }

        m_myNumber = number;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = questName;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = fontColor;
        // �o�^
        m_questStatusSystem = questStatusSystem;
    }

    private void Awake()
    {
        m_questStatusSystem = 
            GameObject.FindGameObjectWithTag("SceneManager").GetComponent<QuestStatusSystem>();
    }

    /// <summary>
    /// �{�^���������ꂽ���̏���
    /// </summary>
    public void QuestNameButtonDown()
    {
        m_questStatusSystem.DisplaySetSValue(m_myNumber);
    }
}
