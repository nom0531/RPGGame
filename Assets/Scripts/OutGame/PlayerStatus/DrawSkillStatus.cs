using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrawSkillStatus : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private SkillDataBase SkillData;

    private GameObject Data_SkillName, Data_Element, Data_Detil;
    private PlayerStatusSystem m_playerStatusSystem;
    private int m_myNumber = 0;     // ���g�̔ԍ�

    public int MyNumber
    {
        set => m_myNumber = value;
    }

    private void Awake()
    {
        m_playerStatusSystem = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<PlayerStatusSystem>();
    }

    /// <summary>
    /// �f�[�^��ݒ肷��
    /// </summary>
    public void SetObject(GameObject name, GameObject element, GameObject detil)
    {
        Data_SkillName = name;
        Data_Element = element;
        Data_Detil = detil;
    }

    /// <summary>
    /// �f�[�^��ݒ肷��
    /// </summary>
    public void SetData()
    {
        Data_SkillName.GetComponent<TextMeshProUGUI>().text = SkillData.skillDataList[m_myNumber].SkillName;
        Data_Detil.GetComponent<TextMeshProUGUI>().text = SkillData.skillDataList[m_myNumber].SkillDetail;
        Data_Element.GetComponent<TextMeshProUGUI>().text = m_playerStatusSystem.SetElementData(m_myNumber);
    }
}
