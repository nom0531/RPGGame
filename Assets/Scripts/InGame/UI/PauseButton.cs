using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト")]
    private GameObject PauseCanvas;

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
        PauseCanvas.SetActive(true);
        m_battleManager.PauseFlag = true;
    }

    /// <summary>
    /// ゲームに戻るボタンが押された時の処理
    /// </summary>
    public void ReturnGameButtonDown()
    {
        PauseCanvas.SetActive(false);
        m_battleManager.PauseFlag = false;
    }
}