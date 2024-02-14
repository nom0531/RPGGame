using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [SerializeField, Header("ポーズボタン")]
    private GameObject PauseButtonObjct;
    [SerializeField, Tooltip("変更先の画像")]
    private Sprite PauseAfterImage;

    private BattleManager m_battleManager;

    private void Start()
    {
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// </summary>
    public void ButtonDown()
    {
        m_battleManager.PauseFlag = true;
        PauseButtonObjct.GetComponent<Image>().sprite = PauseAfterImage;
    }

    /// <summary>
    /// ゲームに戻るボタンが押された時の処理
    /// </summary>
    public void ReturnGameButtonDown()
    {
        m_battleManager.PauseFlag = false;
    }
}