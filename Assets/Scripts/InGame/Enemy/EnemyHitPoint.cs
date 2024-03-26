using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHitPoint : MonoBehaviour
{
    /// <summary>
    /// HP�̌�������
    /// </summary>
    private enum DecreaseProcess
    {
        enStart,    // �J�n
        enEnd,      // �I��
    }


    /// <summary>
    /// �z��̓Y����
    /// </summary>
    private enum ImageState
    {
        enGreen,
        enRed,
        enNum
    }

    [SerializeField, Header("�Q�ƃf�[�^")]
    private EnemyDataBase EnemyData;
    [SerializeField, Header("HP�Q�[�W")]
    private GameObject Data_HPBarGreen;
    [SerializeField]
    private GameObject Data_HPBarRed;

    private BattleManager m_battleManager;
    private DrawStatusValue m_drawStatusValue;
    private LockOnManager m_lockOnManager;
    private Image[] m_barImages = new Image[(int)ImageState.enNum];
    private DecreaseProcess m_decreaseProcess = DecreaseProcess.enEnd;  // �o�[�̌��������̃X�e�[�g
    private bool m_isInit = false;                                      // �����������s�����Ȃ�true

    // Start is called before the first frame update
    void Start()
    {
        SetFillAmount();
    }

    private void Update()
    {
        Decrease();
    }

    /// <summary>
    /// ������ݒ肷��
    /// </summary>
    public void SetFillAmount()
    {
        Init();
        var number = m_battleManager.EnemyMoveList[m_lockOnManager.TargetNumber].MyNumber;
        for (int i = 0; i < (int)ImageState.enNum; i++)
        {
            // ������ݒ�
            m_barImages[i].fillAmount = m_drawStatusValue.CalculateRate(
                m_battleManager.EnemyMoveList[m_lockOnManager.TargetNumber].EnemyStatus.HP, EnemyData.enemyDataList[number].HP);
        }
        m_decreaseProcess = DecreaseProcess.enStart;
    }

    /// <summary>
    /// ������
    /// </summary>
    private void Init()
    {
        if(m_isInit == true)
        {
            return;
        }
        m_drawStatusValue = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<DrawStatusValue>();
        m_barImages[(int)ImageState.enGreen] = Data_HPBarGreen.GetComponent<Image>();
        m_barImages[(int)ImageState.enRed] = Data_HPBarRed.GetComponent<Image>();
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        m_lockOnManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<LockOnManager>();
        m_isInit = false;
    }

    /// <summary>
    /// HP�̌�������
    /// </summary>
    private void Decrease()
    {
        // �����������J�n���Ȃ��Ȃ���s���Ȃ�
        if (m_decreaseProcess == DecreaseProcess.enEnd)
        {
            return;
        }
        // �v�Z
        var number = m_battleManager.EnemyMoveList[m_lockOnManager.TargetNumber].MyNumber;
        // ������ݒ�
        m_barImages[(int)ImageState.enGreen].fillAmount = m_drawStatusValue.CalculateRate(
            m_battleManager.EnemyMoveList[m_lockOnManager.TargetNumber].EnemyStatus.HP, EnemyData.enemyDataList[number].HP);
        m_barImages[(int)ImageState.enRed].fillAmount = m_drawStatusValue.SetRate(
            m_barImages[(int)ImageState.enRed].fillAmount, m_barImages[(int)ImageState.enGreen].fillAmount);
        m_decreaseProcess = DecreaseProcess.enEnd;
    }
}
