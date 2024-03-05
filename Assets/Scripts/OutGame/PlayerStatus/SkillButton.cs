using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillButton : MonoBehaviour
{
    private PlayerStatusSystem m_playerStatusSystem;
    private int m_myNumber = -1;        // ���g�̔ԍ�
    private bool m_isGetData = false;   // �f�[�^���l������

    public int MyNumber
    {
        set => m_myNumber = value;
        get => m_myNumber;
    }

    private void Start()
    {
        m_playerStatusSystem = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<PlayerStatusSystem>();
    }

    private void FixedUpdate()
    {
        if(m_isGetData == false)
        {
            return;
        }
        m_playerStatusSystem.GetData(MyNumber);
        m_isGetData = false;
    }

    /// <summary>
    /// �{�^���������ꂽ���̏���
    /// </summary>
    public void ButtonDown()
    {
        m_isGetData = true;
        m_playerStatusSystem.SetActiveTrue();
        m_playerStatusSystem.DrawData(MyNumber);
    }

    /// <summary>
    /// �f�[�^���X�V����
    /// </summary>
    public void DataUpDate()
    {
        m_playerStatusSystem.ChangeName(gameObject, MyNumber);
    }

    /// <summary>
    /// �X�L�����J������
    /// </summary>
    public void OKButtonDown()
    {
        m_isGetData = true;
        m_playerStatusSystem.SaveReleaseSkillData(MyNumber);
    }
}
