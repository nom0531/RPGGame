using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButton : MonoBehaviour
{
    private const int MAX_NUM = 3;  // プレイヤーの最大値

    private PlayerStatusSystem m_playerStatus;
    private int m_playerNumber = -1;

    private void Start()
    {
        m_playerStatus = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<PlayerStatusSystem>();
        m_playerNumber = GameManager.Instance.PlayerNumber;
    }

    /// <summary>
    /// 左ボタンが押された時の処理
    /// </summary>
    public void ButtonDownLeft()
    {
        m_playerNumber--;
        if(m_playerNumber <= 0)
        {
            m_playerNumber = MAX_NUM - 1;   // 補正
        }
        m_playerStatus.DisplaySetValue(m_playerNumber);
    }

    /// <summary>
    /// 右ボタンが押された時の処理
    /// </summary>
    public void ButtonDownRight()
    {
        m_playerNumber++;
        if (m_playerNumber >= MAX_NUM)
        {
            m_playerNumber = 0;   // 補正
        }
        m_playerStatus.DisplaySetValue(m_playerNumber);
    }
}
