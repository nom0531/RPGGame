using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillButton : MonoBehaviour
{
    private PlayerStatusSystem m_playerStatusSystem;
    private int m_myNumber = -1;        // 自身の番号
    private bool m_isGetData = false;   // データを獲得する

    public int MyNumber
    {
        set => m_myNumber = value;
        get => m_myNumber;
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
        m_playerStatusSystem.GetData(MyNumber);
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
