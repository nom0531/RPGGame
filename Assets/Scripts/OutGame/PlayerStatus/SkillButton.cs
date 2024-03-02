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
        m_playerStatusSystem.SetActive(true);
        m_playerStatusSystem.DrawData(MyNumber);
    }

    /// <summary>
    /// 1つ前の画面に戻る
    /// </summary>
    public void ChancelButtonDown()
    {
        m_playerStatusSystem.SetActive(false);
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
