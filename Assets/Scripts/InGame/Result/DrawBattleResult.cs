using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrawBattleResult : MonoBehaviour
{
    [SerializeField]
    private GameObject ResultText;

    private UIAnimation m_uIAnimation;
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
        m_uIAnimation = GetComponent<UIAnimation>();
    }

    /// <summary>
    /// �Q�[���N���A���o
    /// </summary>
    public void GameClearStaging()
    {
        m_uIAnimation.ButtonDown_Active();
        DrawEP();
        ResultText.GetComponent<TextMeshProUGUI>().text = "WIN!";
    }

    /// <summary>
    /// �Q�[���I�[�o�[���o
    /// </summary>
    public void GameOverStaging()
    {
        m_uIAnimation.ButtonDown_Active();
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
