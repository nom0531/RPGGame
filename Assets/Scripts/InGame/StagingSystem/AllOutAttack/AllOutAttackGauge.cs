using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AddState
{
    enAttack,
    enWeak,
    enResist,
    enDamage,
    enGurad,
}

public class AllOutAttackGauge : MonoBehaviour
{
    [SerializeField, Header("�f�[�^")]
    private Image Gauge;
    [SerializeField]
    private Image Top;
    [SerializeField]
    private Color BaseColor,MulColor;
    [SerializeField, Header("��l")]
    private float DefaultPoint = 25.0f;

    private const float MAX_POINT = 100.0f;
    private const float MIN_POINT = 0.0f;

    private AllOutAttackSystem m_allOutAttackSystem;
    private Image m_image;

    private float m_nowPoint = 0.0f;

    private void Start()
    {
        m_allOutAttackSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<AllOutAttackSystem>();
        m_image = Gauge.GetComponent<Image>();
        SetAmount();
    }

    private void SetColor()
    {
        // ���U�����ł���Ȃ�
        if (m_allOutAttackSystem.CanStartFlag == true)
        {
            Top.GetComponent<Image>().color = BaseColor;
            return;
        }
        Top.GetComponent<Image>().color = MulColor;
    }

    /// <summary>
    /// �{�^����������悤�ɂ���
    /// </summary>
    private void ButtonActive()
    {
        GetComponent<Animator>().SetTrigger("Active");
    }

    /// <summary>
    /// �|�C���g�����Z����
    /// </summary>
    /// <param name="point">���Z��</param>
    /// <param name="addState">�v�Z���@</param>
    public void AddPoint(AddState addState)
    {
        var addValue = 0.0f;
        switch (addState)
        {
            case AddState.enAttack:
                addValue = DefaultPoint;
                break;
            case AddState.enWeak:
                addValue = DefaultPoint * 4.0f;
                break;
            case AddState.enResist:
                addValue = DefaultPoint / 4.0f;
                break;
            case AddState.enGurad:
                addValue = DefaultPoint / 2.0f;
                break;
        }
        m_nowPoint += addValue;
        if (m_nowPoint >= MAX_POINT)
        {
            m_nowPoint = MAX_POINT;
        }
        SetAmount();
    }

    /// <summary>
    /// �|�C���g�����Z�b�g����
    /// </summary>
    public void ResetPoint()
    {
        m_nowPoint = 0.0f;
        SetAmount();
    }

    /// <summary>
    /// ������ݒ肷��
    /// </summary>
    private void SetAmount()
    {
        m_image.fillAmount = m_nowPoint / MAX_POINT;
        if(m_image.fillAmount == 1.0f)
        {
            m_allOutAttackSystem.CanStartFlag = true;
            ButtonActive();
        }
        SetColor();
    }
}
