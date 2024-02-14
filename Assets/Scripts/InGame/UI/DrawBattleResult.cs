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

    private GetEP m_getEP;  // �l��EP�̋L�^
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
    /// �Q�[���N���A���o
    /// </summary>
    public void GameClearStaging()
    {
        Canvas.SetActive(true);
        DrawEP();
        ResultText.GetComponent<TextMeshProUGUI>().text = "WIN!";
    }

    /// <summary>
    /// �Q�[���I�[�o�[���o
    /// </summary>
    public void GameOverStaging()
    {
        Canvas.SetActive(true);
        DrawEP();
        ResultText.GetComponent<TextMeshProUGUI>().text = "LOSE�c";
    }

    /// <summary>
    /// �l��EP�̕\��
    /// </summary>
    private void DrawEP()
    {
        m_getEP.SaveDropEP(EP);
        m_getEP.DrawText();
    }
}
