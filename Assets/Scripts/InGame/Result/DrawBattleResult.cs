using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrawBattleResult : MonoBehaviour
{
    [SerializeField]
    private GameObject ResultText;

    private UIAnimation m_uIAnimation;
    private SE m_se;
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
        m_se = GetComponent<SE>();
        m_uIAnimation = GetComponent<UIAnimation>();
    }

    /// <summary>
    /// �Q�[���N���A���o
    /// </summary>
    public void GameClearStaging()
    {
        SetSE(true);
        m_uIAnimation.ButtonDown_Active();
        DrawEP();
        ResultText.GetComponent<TextMeshProUGUI>().text = "WIN!";
    }

    /// <summary>
    /// �Q�[���I�[�o�[���o
    /// </summary>
    public void GameOverStaging()
    {
        SetSE(false);
        m_uIAnimation.ButtonDown_Active();
        DrawEP();
        ResultText.GetComponent<TextMeshProUGUI>().text = "LOSE�c";
    }

    /// <summary>
    /// SE��ݒ肷��
    /// </summary>
    private void SetSE(bool isWin)
    {
        if(isWin == true)
        {
            m_se.Number = SENumber.enWin;
        }
        else
        {
            m_se.Number = SENumber.enLose;
        }
        m_se.PlaySE();
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
