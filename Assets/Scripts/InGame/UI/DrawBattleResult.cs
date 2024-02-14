using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrawBattleResult : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト")]
    private GameObject Canvas;
    [SerializeField,]
    private GameObject ResultText;

    private GetEP m_getEP;  // 獲得EPの記録
    private int m_EP;       // EP

    public int EP
    {
        get => m_EP;
        set => m_EP = value;
    }

    private void Start()
    {
        m_getEP = GetComponent<GetEP>();
        Canvas.SetActive(false);
    }

    /// <summary>
    /// ゲームクリア演出
    /// </summary>
    public void GameClearStaging()
    {
        Canvas.SetActive(true);
        DrawEP();
        ResultText.GetComponent<TextMeshProUGUI>().text = "WIN!";
    }

    /// <summary>
    /// ゲームオーバー演出
    /// </summary>
    public void GameOverStaging()
    {
        Canvas.SetActive(true);
        DrawEP();
        ResultText.GetComponent<TextMeshProUGUI>().text = "LOSE…";
    }

    /// <summary>
    /// 獲得EPの表示
    /// </summary>
    private void DrawEP()
    {
        m_getEP.SaveDropEP(EP);
        m_getEP.DrawText();
    }
}
