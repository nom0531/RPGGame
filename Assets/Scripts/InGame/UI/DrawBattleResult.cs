using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrawBattleResult : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject Canvas;
    [SerializeField,]
    private GameObject ResultText;

    private void Start()
    {
        Canvas.SetActive(false);
    }

    /// <summary>
    /// �Q�[���N���A���o
    /// </summary>
    public void GameClearStaging()
    {
        Canvas.SetActive(true);
        ResultText.GetComponent<TextMeshProUGUI>().text = "WIN!";
    }

    /// <summary>
    /// �Q�[���I�[�o�[���o
    /// </summary>
    public void GameOverStaging()
    {
        Canvas.SetActive(true);
        ResultText.GetComponent<TextMeshProUGUI>().text = "LOSE�c";
    }
}
