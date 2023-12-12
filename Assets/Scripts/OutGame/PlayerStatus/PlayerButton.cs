using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerButton : MonoBehaviour
{
    // プレイヤーの番号
    private int m_playerNumber = -1;
    // 図鑑システム
    private PlayerStatusSystem m_playerStatus;
    private PlayerEnhancementSystem m_playerEnhancement;

    /// <summary>
    /// 初期化用の関数。プレイヤーを登録する
    /// </summary>
    /// <param name="number">プレイヤーの番号</param>
    /// <param name="playerImage">プレイヤーの画像</param>
    public void SetPlayerStatus(int number, Sprite playerImage, PlayerStatusSystem playerStatus)
    {
        // それぞれの値を登録する
        m_playerNumber = number;
        GetComponent<Image>().sprite = playerImage;
        // 図鑑システムを登録する
        m_playerStatus = playerStatus;
    }

    /// <summary>
    /// 初期化用の関数。プレイヤーを登録する
    /// </summary>
    /// <param name="number">プレイヤーの番号</param>
    /// <param name="enemyImage">プレイヤーの画像</param>
    public void SetPlayerEnhancement(int number, Sprite playerImage, PlayerEnhancementSystem playerEnhancement)
    {
        // それぞれの値を登録する
        m_playerNumber = number;
        GetComponent<Image>().sprite = playerImage;
        // 図鑑システムを登録する
        m_playerEnhancement = playerEnhancement;
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// </summary>
    public void PlayerStatusButtoonDown()
    {
        m_playerStatus.DisplaySetValue(m_playerNumber);
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// </summary>
    public void PlayerEnhancementButtoonDown()
    {
        m_playerEnhancement.DisplaySetValue(m_playerNumber);
    }
}
