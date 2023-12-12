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

    private void Start()
    {
        Canvas.SetActive(false);
    }

    /// <summary>
    /// ゲームクリア演出
    /// </summary>
    public void GameClearStaging()
    {
        Canvas.SetActive(true);
        ResultText.GetComponent<TextMeshProUGUI>().text = "WIN!";
    }

    /// <summary>
    /// ゲームオーバー演出
    /// </summary>
    public void GameOverStaging()
    {
        Canvas.SetActive(true);
        ResultText.GetComponent<TextMeshProUGUI>().text = "LOSE…";
    }
}
