using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutInManager : MonoBehaviour
{
    [SerializeField, Header("�o�͂���摜"),Tooltip("�o�͐�")]
    private GameObject PlayerTextuerObject;
    [SerializeField]
    private Sprite[] PlayerTextures;
    [SerializeField, Header("�w�i�F"), Tooltip("�o�͐�")]
    private GameObject BackGroundObject;
    [SerializeField]
    private Color[] PlayerColors;

    private BattleManager m_battleManager;
    private UIAnimation m_uIAnimation;
    private bool m_isInit = false;

    /// <summary>
    /// �J�b�g�C������
    /// </summary>
    public void CutIn()
    {
        Init();
        // �f�[�^��������
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
        // ������
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        m_uIAnimation = GetComponent<UIAnimation>();
        m_uIAnimation.Animator = GetComponent<Animator>();
        m_isInit = true;
    }
}
