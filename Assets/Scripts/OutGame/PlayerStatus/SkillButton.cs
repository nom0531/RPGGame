using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillButton : MonoBehaviour
{
    private PlayerStatusSystem m_playerStatusSystem;
    private int m_myNumber = -1;                // 自身の番号
    private int m_myNumberInPlayerData = -1;    // プレイヤーデータ内での自身の番号
    private bool m_isGetData = false;           // データを獲得する

    public int MyNumber
    {
        get => m_myNumber;
        set => m_myNumber = value;
    }

    public int MyNumberInPlayerData
    {
        get => m_myNumberInPlayerData;
        set => m_myNumberInPlayerData = value;
    }

    private void Start()
    {
        m_playerStatusSystem = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<PlayerStatusSystem>();
    }

    private void FixedUpdate()
    {
        if(m_isGetData == false)
        {
            return;
        }
        m_playerStatusSystem.GetData(MyNumber, MyNumberInPlayerData);
        m_isGetData = false;
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// </summary>
    public void ButtonDown()
    {
        m_isGetData = true;
        m_playerStatusSystem.SetActiveTrue();
        m_playerStatusSystem.DrawData(MyNumber);
    }

    /// <summary>
    /// データを更新する
    /// </summary>
    public void DataUpDate()
    {
        m_playerStatusSystem.ChangeName(gameObject, MyNumber);
    }

    /// <summary>
    /// スキルを開放する
    /// </summary>
    public void OKButtonDown()
    {
        m_isGetData = true;
        m_playerStatusSystem.SaveReleaseSkillData(MyNumber);
    }
}
