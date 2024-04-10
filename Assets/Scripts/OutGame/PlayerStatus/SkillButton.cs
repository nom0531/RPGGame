using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillButton : MonoBehaviour
{
    private PlayerStatusSystem m_playerStatusSystem;
    private GameObject m_skillNameObject;
    private int m_myNumber = -1;                // ���g�̔ԍ�
    private int m_myNumberInPlayerData = -1;    // �v���C���[�f�[�^���ł̎��g�̔ԍ�

    public GameObject SkillNameObject
    {
        set => m_skillNameObject = value;
    }

    public int MyNumber
    {
        get => m_myNumber;
        set => m_myNumber = value;
    }

    public int MyNumberInPlayerData
    {
        get => m_myNumberInPlayerData;
        set => m_myNumberInPlayerData = value;
    }

    private void Start()
    {
        m_playerStatusSystem = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<PlayerStatusSystem>();
    }

    /// <summary>
    /// �{�^���������ꂽ���̏���
    /// </summary>
    public void ButtonDown()
    {
        m_playerStatusSystem.DrawData(MyNumber, MyNumberInPlayerData, gameObject);
        DataUpdate();
    }

    /// <summary>
    /// �X�L�����J������
    /// </summary>
    public void OKButtonDown()
    {
        m_playerStatusSystem.SaveReleaseSkillData(MyNumberInPlayerData);
        DataUpdate();
    }

    private void DataUpdate()
    {
        m_playerStatusSystem.GetData(MyNumber, MyNumberInPlayerData);
        m_playerStatusSystem.ChangeName(m_skillNameObject, MyNumberInPlayerData);
    }
}
