using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButton : MonoBehaviour
{
    [SerializeField]
    private GameObject Data_Sprite;

    private const int MAX_NUM = 2;  // プレイヤーの最大値

    private PlayerStatusSystem m_playerStatus;
    private StatusAnimation m_statusAnimation;
    private bool m_isDown = false;
    private int m_playerNumber = -1;

    public bool ButtonDownFlag
    {
        get => m_isDown;
    }

    private void Start()
    {
        m_playerStatus = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<PlayerStatusSystem>();
        m_playerNumber = GameManager.Instance.PlayerNumber;
        m_statusAnimation = Data_Sprite.GetComponent<StatusAnimation>();
    }

    /// <summary>
    /// 左ボタンが押された時の処理
    /// </summary>
    public void ButtonDownLeft()
    {
        m_isDown = true;
        m_playerNumber--;
        m_statusAnimation.SpriteAnimaiton(SpriteState.enActiveL);
        if (m_playerNumber < 0)
        {
            m_playerNumber = MAX_NUM;
        }
        m_playerStatus.DisplaySetValue(m_playerNumber);
    }

    /// <summary>
    /// 右ボタンが押された時の処理
    /// </summary>
    public void ButtonDownRight()
    {
        m_isDown = true;
        m_playerNumber++;
        m_statusAnimation.SpriteAnimaiton(SpriteState.enActiveR);
        if (m_playerNumber > MAX_NUM)
        {
            m_playerNumber = 0;
        }        m_statusAnimation.SpriteAnimaiton(SpriteState.enActiveR);
        m_playerStatus.DisplaySetValue(m_playerNumber);
    }

    /// <summary>
    /// フラグをリセットする
    /// </summary>
    public void ResetButtonDownFlag()
    {
        m_isDown = false;
    }
}
