using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class QuestButton : MonoBehaviour
{
    private const int STRING_MAX = 5; // 文字の最大表示数

    private QuestStatusSystem m_questStatusSystem;
    private int m_myNumber = -1;     // クエストの番号

    /// <summary>
    /// 初期化用の関数。クエストを登録する
    /// </summary>
    /// <param name="number">クエストの番号</param>
    /// <param name="questName">クエストの名前</param>
    /// <param name="fontColor">文字の色</param>
    public void SetQuestStatus(int number,string questName,Color fontColor,QuestStatusSystem questStatusSystem)
    {
        // 文字が一定以上なら
        if (questName.Length >= STRING_MAX)
        {
            // 一部の文字を省略する
            questName = questName.Substring(0, STRING_MAX);
            questName = $"『{questName}…";
        }
        else
        {
            questName = $"『{questName}』";
        }

        m_myNumber = number;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = questName;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = fontColor;
        // 登録
        m_questStatusSystem = questStatusSystem;
    }

    private void Awake()
    {
        m_questStatusSystem = 
            GameObject.FindGameObjectWithTag("SceneManager").GetComponent<QuestStatusSystem>();
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// </summary>
    public void QuestNameButtonDown()
    {
        m_questStatusSystem.DisplaySetSValue(m_myNumber);
    }
}
