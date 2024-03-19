using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

/// <summary>
/// �^�[�Q�b�g�ɂ��鑊��
/// </summary>
public enum TargetState
{
    enPlayer,
    enEnemy
}

public class LockOnManager : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g"), Tooltip("�W���J����")]
    private CinemachineVirtualCameraBase[] Vcam_Default;
    [SerializeField,Tooltip("���b�N�I���J����")]
    private CinemachineVirtualCameraBase[] Vcam_LockOn;
    [SerializeField]
    private LockOnButton RightButton, LeftButton, OKButton, CancelButton;
    [SerializeField]
    private GameObject LockOnCanvas;
    [SerializeField, Header("HP")]
    private GameObject EnemyHPObject;

    private const int VCAM_PRIORITY = 10;           // �J�����g�p���̗D��x
    private const int NUM_MIN = 0;                  // �ŏ��ԍ�

    private BattleManager m_battleManager;
    private TurnManager m_turnManager;
    private PauseManager m_pauseManager;
    private List<EnemyMove> m_enemyMoveList;
    private List<PlayerMove> m_playerMoveList;
    private DrawStatusValue m_drawStatusValue;
    private SkillDataBase m_skillData;
    private EnemyHitPoint m_enemyHitPoint;
    private TargetState m_target;                   // �^�[�Q�b�g�ɂ��鑊��
    private int m_operatingPlayerNumber = 0;              // ���ݑ��삵�Ă���v���C���[
    private int m_selectTargetNumber = 0;           // ���ݑI�����Ă���^�[�Q�b�g�̔ԍ�
    private bool m_isLockOnStart = false;           // ���b�N�I�����J�n���邩�ǂ���
    private bool m_isButtonDown = false;            // �{�^���������ꂽ���ǂ���
    private bool m_isActive = false;                // ��xActive�ɂ������ǂ���

    public bool ButtonDown
    {
        get => m_isButtonDown;
        set => m_isButtonDown = value;
    }

    public bool LockOn
    {
        set => m_isLockOnStart = value;
    }

    public int TargetNumber
    {
        get => m_selectTargetNumber;
    }

    public TargetState TargetState
    {
        get => m_target;
        set => m_target = value;
    }

    // Start is called before the first frame update
    private void Awake()
    {
        ReSetVcamStatus();
        m_drawStatusValue = GetComponent<DrawStatusValue>();
        m_battleManager = GetComponent<BattleManager>();
        m_turnManager = GetComponent<TurnManager>();
        m_pauseManager = GetComponent<PauseManager>();
        m_skillData = m_battleManager.SkillDataBase;
        SetInteractable(false);
    }

    private void Start()
    {
        m_enemyHitPoint = EnemyHPObject.GetComponent<EnemyHitPoint>();
        m_operatingPlayerNumber = (int)m_battleManager.OperatingPlayer;
        SetCinemachineVirtualCameraPriority(m_operatingPlayerNumber, false, null);
        LockOnCanvas.SetActive(false);
        // �G�l�~�[�̃��X�g
        m_enemyMoveList = m_battleManager.EnemyMoveList;
        m_drawStatusValue.EnemyName = m_enemyMoveList[m_selectTargetNumber].MyNumber;
        // �v���C���[�̃��X�g
        m_playerMoveList = m_battleManager.PlayerMoveList;
        m_playerMoveList.Sort((a, b) => a.MyNumber.CompareTo(b.MyNumber));
        m_drawStatusValue.PlayerName = m_playerMoveList[m_selectTargetNumber].MyNumber;

        DrawPlayers(false);
        m_playerMoveList[m_operatingPlayerNumber].gameObject.SetActive(true);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // �|�[�Y���Ȃ珈���𒆒f
        if (m_pauseManager.PauseFlag == true)
        {
            return;
        }
        // ���b�N�I�����J�n���Ă��Ȃ��Ȃ���s���Ȃ�
        if(m_isLockOnStart == false)
        {
            return;
        }
        LockOnStart();
    }
    
    /// <summary>
    /// �v���C���[��Active��ݒ肷��
    /// </summary>
    /// <param name="flag">true�Ȃ�`��Bfalse�Ȃ�`�悵�Ȃ�</param>
    private void DrawPlayers(bool flag)
    {
        // ���o�����s���Ȃ疳��
        if(m_battleManager.StagingStartFlag == true)
        {
            return;
        }
        for(int i = 0; i < m_playerMoveList.Count; i++)
        {
            m_playerMoveList[i].gameObject.SetActive(flag);
        }
    }

    /// <summary>
    /// �J������؂�ւ���
    /// </summary>
    public void ResetCinemachine()
    {
        m_operatingPlayerNumber = (int)m_battleManager.OperatingPlayer;
        LockOnEnd();
    }

    /// <summary>
    /// ���b�N�I�����J�n���鏈��
    /// </summary>
    private void LockOnStart()
    {
        // ���Ɏ��s���Ă���Ȃ���s�͂��Ȃ�
        if (m_isActive == true)
        {
            return;
        }
        // �ԍ���␳
        m_selectTargetNumber = 0;
        // �I�u�W�F�N�g�̕\���A��\����؂�ւ���
        LockOnCanvas.SetActive(true);
        m_playerMoveList[m_operatingPlayerNumber].gameObject.SetActive(true);
        // HP�o�[���Đݒ肷��
        m_enemyHitPoint.SetFillAmount();
        // �{�^���������邩�ǂ����ݒ肷��
        SetInteractable(true);
        // �J������ݒ肷��
        SetCinemachineVirtualCameraPriority(m_operatingPlayerNumber, true, SetLookAtTarget());
        m_isActive = true;
    }

    /// <summary>
    /// ���b�N�I�����I�����鏈��
    /// </summary>
    public void LockOnEnd()
    {
        // �J���������ɖ߂�
        SetCinemachineVirtualCameraPriority(m_operatingPlayerNumber, false, null);
        // �t���O�����Z�b�g
        SetInteractable(false);
        LockOn = false;
        m_battleManager.ResetBattleButton();
        m_isActive = false;
        // �v���C���[�̕\����؂�ւ���
        DrawPlayers(false);
        m_playerMoveList[m_operatingPlayerNumber].gameObject.SetActive(true);
    }

    /// <summary>
    /// 1�E�����b�N�I������
    /// </summary>
    public void LookAtRightTarget()
    {
        int max = m_enemyMoveList.Count - 1;
        // �^�[�Q�b�g���v���C���[�Ȃ�Đݒ�
        if (TargetState == TargetState.enPlayer)
        {
            max = m_playerMoveList.Count - 1;
            m_selectTargetNumber--;
            // 0�ȉ��Ȃ�␳
            if (m_selectTargetNumber < NUM_MIN)
            {
                m_selectTargetNumber = max;
            }
        }
        else
        {
            m_selectTargetNumber++;
            // �ő�l�ȏ�Ȃ�␳
            if (m_selectTargetNumber > max)
            {
                m_selectTargetNumber = NUM_MIN;
            }
        }
        m_enemyHitPoint.SetFillAmount();
        SetCinemachineVirtualCameraPriority(m_operatingPlayerNumber, true, SetLookAtTarget());
    }

    /// <summary>
    /// 1�������b�N�I������
    /// </summary>
    public void LookAtLeftTarget()
    {
        int max = m_enemyMoveList.Count - 1;
        // �^�[�Q�b�g���v���C���[�Ȃ�Đݒ�
        if (TargetState == TargetState.enPlayer)
        {
            max = m_playerMoveList.Count - 1;
            m_selectTargetNumber++;
            // �ő�l�ȏ�Ȃ�␳
            if (m_selectTargetNumber > max)
            {
                m_selectTargetNumber = NUM_MIN;
            }
        }
        else
        {
            m_selectTargetNumber--;
            // 0�ȏ�Ȃ�␳
            if (m_selectTargetNumber < NUM_MIN)
            {
                m_selectTargetNumber = max;
            }
        }
        m_enemyHitPoint.SetFillAmount();
        SetCinemachineVirtualCameraPriority(m_operatingPlayerNumber, true, SetLookAtTarget());
    }

    /// <summary>
    /// �eUI�{�^���������邩�ǂ����̃t���O��ݒ肷��
    /// </summary>
    /// <param name="flag">true�Ȃ牟����Bfalse�Ȃ牟���Ȃ�</param>
    private void SetInteractable(bool flag)
    {
        RightButton.GetComponent<Button>().interactable = flag;
        LeftButton.GetComponent<Button>().interactable = flag;
        OKButton.GetComponent<Button>().interactable = flag;
        CancelButton.GetComponent<Button>().interactable = flag;
    }

    /// <summary>
    /// ���z�J�����̗D��x��ݒ肷��
    /// </summary>
    /// <param name="number">���ݑ��삵�Ă���v���C���[�̔ԍ�</param>
    /// <param name="isLockOn">true�Ȃ烍�b�N�I�����J�n����Bfalse�Ȃ烍�b�N�I�����Ȃ�</param>
    /// <param name="gameObject">���b�N�I�����鑊��̃I�u�W�F�N�g</param>
    private void SetCinemachineVirtualCameraPriority(int number, bool isLockOn, GameObject gameObject)
    {
        if (m_battleManager.GameState != GameState.enPlay)
        {
            return;
        }
        ReSetVcamStatus();
        if (isLockOn == true)
        {
            // �^�[�Q�b�g��ݒ肷��
            Vcam_LockOn[0].Priority = VCAM_PRIORITY;
            Vcam_LockOn[0].LookAt = gameObject.transform;
            return;
        }
        // �D��x��ݒ�
        Vcam_Default[number].Priority = VCAM_PRIORITY;
    }

    /// <summary>
    /// �����J�����̗D��x�����Z�b�g����
    /// </summary>
    private void ReSetVcamStatus()
    {
        Vcam_LockOn[0].Priority = 0;

        for (int i = 0; i < Vcam_Default.Length; i++)
        {
            Vcam_Default[i].Priority = 0;
        }
    }

    /// <summary>
    /// ���ݑI�����Ă���G�l�~�[�̔ԍ�������������
    /// </summary>
    private void ResetSelectEnemyNumber()
    {
        // �␳
        if (m_selectTargetNumber >= m_enemyMoveList.Count)
        {
            m_selectTargetNumber = m_enemyMoveList.Count - 1;
        }
        // �^�[�Q�b�g���Ђ񎀂łȂ��Ȃ���s���Ȃ�
        if (m_enemyMoveList[m_selectTargetNumber].ActorHPState != ActorHPState.enDie)
        {
            return;
        }
        m_enemyMoveList.Remove(m_enemyMoveList[m_selectTargetNumber]);
        for (int i = 0; i < m_enemyMoveList.Count; i++)
        {
            // �G�l�~�[���Ђ񎀂łȂ��Ȃ�
            if (m_enemyMoveList[i].ActorHPState != ActorHPState.enDie)
            {
                // �l��ݒ肷��
                m_selectTargetNumber = i;
                break;
            }
        }
    }

    /// <summary>
    /// ���ݑI�����Ă���v���C���[�̔ԍ������Z�b�g����
    /// </summary>
    private void ResetSelectPlayerNumber()
    {
        // �␳
        if (m_selectTargetNumber >= m_playerMoveList.Count)
        {
            m_selectTargetNumber = m_playerMoveList.Count - 1;
        }
        // �^�[�Q�b�g���Ђ񎀂łȂ��Ȃ���s���Ȃ�
        if (m_playerMoveList[m_selectTargetNumber].ActorHPState != ActorHPState.enDie)
        {
            return;
        }
        m_playerMoveList.Remove(m_playerMoveList[m_selectTargetNumber]);
        for (int i = 0; i < m_playerMoveList.Count; i++)
        {
            // �v���C���[���Ђ񎀂łȂ��Ȃ�
            if (m_playerMoveList[i].ActorHPState != ActorHPState.enDie)
            {
                // �l��ݒ肷��
                m_selectTargetNumber = i;
                break;
            }
        }
    }

    /// <summary>
    /// ���b�N�I���̑Ώۂ�ݒ肷��
    /// </summary>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    public void SetTargetState(int skillNumber, ActionType actionType)
    {
        switch (m_turnManager.TurnStatus)
        {
            case TurnStatus.enPlayer:
                LockOn = true;
                TargetState = TargetState.enEnemy;
                // ���̍s�����U���Ȃ炱��ȉ��̏����͎��s���Ȃ�
                if (actionType == ActionType.enAttack)
                {
                    return;
                }
                switch (m_skillData.skillDataList[skillNumber].SkillType)
                {
                    case SkillType.enHeal:
                    case SkillType.enResurrection:
                    case SkillType.enBuff:
                        TargetState = TargetState.enPlayer;
                        break;
                    default:
                        break;
                }
                break;
            case TurnStatus.enEnemy:
                LockOn = false;
                TargetState = TargetState.enPlayer;
                // ���̍s�����U���Ȃ炱��ȉ��̏����͎��s���Ȃ�
                if (actionType == ActionType.enAttack)
                {
                    return;
                }
                switch (m_skillData.skillDataList[skillNumber].SkillType)
                {
                    case SkillType.enHeal:
                    case SkillType.enResurrection:
                    case SkillType.enBuff:
                        TargetState = TargetState.enEnemy;
                        break;
                    default:
                        break;
                }
                break;
        }
    }

    /// <summary>
    /// ���肵���^�[�Q�b�g��ݒ肷��
    /// </summary>
    /// <returns>�^�[�Q�b�g�̎q�I�u�W�F�N�g</returns>
    private GameObject SetLookAtTarget()
    {
        if(TargetState == TargetState.enPlayer)
        {
            ResetSelectPlayerNumber();
            m_drawStatusValue.PlayerName = m_playerMoveList[m_selectTargetNumber].MyNumber;
            DrawPlayers(true);
            return m_playerMoveList[m_selectTargetNumber].transform.GetChild(0).gameObject;
        }
        ResetSelectEnemyNumber();
        m_playerMoveList[m_operatingPlayerNumber].gameObject.SetActive(false);
        m_drawStatusValue.EnemyName = m_enemyMoveList[m_selectTargetNumber].MyNumber;
        return m_enemyMoveList[m_selectTargetNumber].transform.GetChild(0).gameObject;
    }
}