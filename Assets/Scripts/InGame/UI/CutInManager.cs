using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutInManager : MonoBehaviour
{
    [SerializeField, Header("出力する画像"),Tooltip("出力先")]
    private GameObject PlayerTextuerObject;
    [SerializeField]
    private Sprite[] PlayerTextures;
    [SerializeField, Header("背景色"), Tooltip("出力先")]
    private GameObject BackGroundObject;
    [SerializeField]
    private Color[] PlayerColors;

    private BattleManager m_battleManager;
    private UIAnimation m_uIAnimation;
    private bool m_isInit = false;

    /// <summary>
    /// カットイン処理
    /// </summary>
    public void CutIn()
    {
        Init();
        // データを初期化
        PlayerTextuerObject.GetComponent<Image>().sprite = PlayerTextures[(int)m_battleManager.OperatingPlayer];
        BackGroundObject.GetComponent<Image>().color = PlayerColors[(int)m_battleManager.OperatingPlayer];
        m_uIAnimation.ButtonDown_Active();
    }

    private void Init()
    {
        if(m_isInit == true)
        {
            return;
        }
        // 初期化
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        m_uIAnimation = GetComponent<UIAnimation>();
        m_uIAnimation.Animator = GetComponent<Animator>();
        m_isInit = true;
    }
}
