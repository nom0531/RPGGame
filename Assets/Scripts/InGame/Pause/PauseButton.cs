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

    private PauseManager m_pauseManager;

    private void Start()
    {
        m_pauseManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<PauseManager>();
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// </summary>
    public void ButtonDown()
    {
        m_pauseManager.PauseFlag = true;
        PauseButtonObjct.GetComponent<Image>().sprite = PauseAfterImage;
    }
}