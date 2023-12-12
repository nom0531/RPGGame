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

public class LockOnSystem : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private SkillDataBase SkillData;
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g"), Tooltip("�W���J����")]
    private CinemachineVirtualCameraBase[] Vcam_Default;
    [SerializeField,Tooltip("���b�N�I���J����")]
    private CinemachineVirtualCameraBase[] Vcam_LockOn;
    [SerializeField, Tooltip("�v���C���[�̃I�u�W�F�N�g")]
    private GameObject[] PlayerObject;
    [SerializeField]
    private LockOnButton RightButton, LeftButton, OKButton, CancelButton;
    [SerializeField]
    private GameObject LockOnButtons;
    [SerializeField]
    private GameObject CommandWindow;
    [SerializeField]
    private GameObject TargetNameWindow;

    private const int VCAM_PRIORITY = 10;           // �J�����g�p���̗D��x
    private const int NUM_MIN = 0;                  // �ŏ��ԍ�

    private BattleManager m_battleManager;
    private List<EnemyMove> m_enemyMoveList;
    private List<PlayerMove> m_playerMoveList;
    private DrawStatusValue m_drawStatusValue;
    private TargetState m_target;                   // �^�[�Q�b�g�ɂ��鑊��
    private int m_operatingPlayer = 0;              // ���ݑ��삵�Ă���v���C���[
    private int m_selectTargetNumber = 0;           // ���ݑI�����Ă���^�[�Q�b�g�̔ԍ�
    private int m_selectPlayerNumber = 0;           // ���ݑI�����Ă���v���C���[�̔ԍ�
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

    // Start is called before the first frame update
    private void Awake()
    {
        ReSetVcamStatus();

        m_drawStatusValue = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<DrawStatusValue>();
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        m_operatingPlayer = m_battleManager.OperatingPlayerNumber;

        m_selectTargetNumber = 0;

        SetCinemachineVirtualCameraPriority(m_operatingPlayer, false, null);
        SetInteractable(false);
    }

    private void Start()
    {
        TargetNameWindow.SetActive(false);

        // enemyMove��l�����p��
        var enemyMoveList = FindObjectsOfType<EnemyMove>();
        m_enemyMoveList = new List<EnemyMove>(enemyMoveList);
        // ���O��ݒ肷��
        m_drawStatusValue.EnemyName = m_enemyMoveList[m_selectTargetNumber].MyNumber;
        // playerMove��l�����p��
        var playerMoveList = FindObjectsOfType<PlayerMove>();
        m_playerMoveList = new List<PlayerMove>(playerMoveList);
        // �\�[�g
        m_playerMoveList.Sort((a, b) => a.MyNumber.CompareTo(b.MyNumber));
        // ���O��ݒ肷��
        m_drawStatusValue.PlayerName = m_playerMoveList[m_selectTargetNumber].MyNumber;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (m_battleManager.GameState != GameState.enPlay)
        {
            return;
        }

        // �|�[�Y���Ȃ珈���𒆒f
        if (m_battleManager.PauseFlag == true)
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
    /// �J������؂�ւ���
    /// </summary>
    public void ResetCinemachine()
    {
        m_operatingPlayer = m_battleManager.OperatingPlayerNumber;
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

        // �I�u�W�F�N�g�̕\���A��\����؂�ւ���
        CommandWindow.SetActive(false);
        LockOnButtons.SetActive(true);
        TargetNameWindow.SetActive(true);
        // �{�^���������邩�ǂ����ݒ肷��
        SetInteractable(true);
        // �J������ݒ肷��
        SetCinemachineVirtualCameraPriority(m_operatingPlayer, true, SetLookAtTarget());
        m_isActive = true;
    }

    /// <summary>
    /// ���b�N�I�����I�����鏈��
    /// </summary>
    public void LockOnEnd()
    {
        // �J���������ɖ߂�
        SetCinemachineVirtualCameraPriority(m_operatingPlayer, false, null);
        // �I�u�W�F�N�g�̕\���A��\����؂�ւ���
        CommandWindow.SetActive(true);
        LockOnButtons.SetActive(false);
        TargetNameWindow.SetActive(false);
        PlayerObject[m_operatingPlayer].SetActive(true);
        // �t���O�����Z�b�g
        SetInteractable(false);
        LockOn = false;
        m_battleManager.PlayerAction_ResetBattleButton();
        m_isActive = false;
    }

    /// <summary>
    /// 1�E�����b�N�I������
    /// </summary>
    public void LookAtRightTarget()
    {
        // �ő�l��ݒ肷��
        int max = m_enemyMoveList.Count - 1;
        // �^�[�Q�b�g���v���C���[�Ȃ�Đݒ�
        if (m_target == TargetState.enPlayer)
        {
            max = m_playerMoveList.Count - 1;
        }

        m_selectTargetNumber++;

        // ���ȉ��Ȃ�␳
        if (m_selectTargetNumber > max)
        {
            // �ő�l��I������
            m_selectTargetNumber = NUM_MIN;
        }

        SetCinemachineVirtualCameraPriority(m_operatingPlayer, true, SetLookAtTarget());
    }

    /// <summary>
    /// 1�������b�N�I������
    /// </summary>
    public void LookAtLeftTarget()
    {
        // �ő�l��ݒ肷��
        int max = m_enemyMoveList.Count - 1;
        // �^�[�Q�b�g���v���C���[�Ȃ�Đݒ�
        if (m_target == TargetState.enPlayer)
        {
            max = m_playerMoveList.Count - 1;
        }

        m_selectTargetNumber--;

        // ���ȏ�Ȃ�␳
        if (m_selectTargetNumber < NUM_MIN)
        {
            // �ŏ��l��I������
            m_selectTargetNumber = max;
        }

        SetCinemachineVirtualCameraPriority(m_operatingPlayer, true, SetLookAtTarget());
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
            Vcam_LockOn[number].Priority = VCAM_PRIORITY;
            Vcam_LockOn[number].LookAt = gameObject.transform;
            return;
        }

        Vcam_Default[number].Priority = VCAM_PRIORITY;
    }

    /// <summary>
    /// �����J�����̗D��x�����Z�b�g����
    /// </summary>
    private void ReSetVcamStatus()
    {
        for(int i = 0; i < Vcam_Default.Length; i++)
        {
            Vcam_Default[i].Priority = 0;
            Vcam_LockOn[i].Priority = 0;
        }
    }

    /// <summary>
    /// ���ݑI�����Ă���G�l�~�[�̔ԍ�������������
    /// </summary>
    private void ResetSelectEnemyNumber()
    {
        // �I�����Ă����G�l�~�[���Ђ񎀂łȂ��Ȃ���s���Ȃ�
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
        // �����v�f���ȏ�Ȃ�␳����
        if(m_selectTargetNumber >= m_playerMoveList.Count)
        {
            m_selectTargetNumber = 0;
        }

        // �I�����Ă����G�l�~�[���Ђ񎀂łȂ��Ȃ���s���Ȃ�
        if (m_playerMoveList[m_selectTargetNumber].PlayerStatus.HPState != ActorHPState.enDie)
        {
            return;
        }

        m_playerMoveList.Remove(m_playerMoveList[m_selectTargetNumber]);

        for (int i = 0; i < m_playerMoveList.Count; i++)
        {
            // �v���C���[���Ђ񎀂łȂ��Ȃ�
            if (m_playerMoveList[i].PlayerStatus.HPState != ActorHPState.enDie)
            {
                // �l��ݒ肷��
                m_selectTargetNumber = i;
                break;
            }
        }
    }

    /// <summary>
    /// �^�[�Q�b�g���ݒ肷��
    /// </summary>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    public void SetTargetState(int skillNumber, ActionType actionType)
    {
        m_target = TargetState.enEnemy;

        // ���̍s�����U���Ȃ炱��ȉ��̏����͎��s���Ȃ�
        if(actionType == ActionType.enAttack)
        {
            return;
        }

        switch (SkillData.skillDataList[skillNumber].SkillType)
        {
            case SkillType.enHeal:
            case SkillType.enResurrection:
            case SkillType.enBuff:
                m_target = TargetState.enPlayer;
                break;
        }
    }

    /// <summary>
    /// ���肵���^�[�Q�b�g��ݒ肷��
    /// </summary>
    /// <returns>�I�u�W�F�N�g</returns>
    private GameObject SetLookAtTarget()
    {
        if(m_target == TargetState.enPlayer)
        {
            ResetSelectPlayerNumber();
            m_drawStatusValue.PlayerName = m_playerMoveList[m_selectTargetNumber].MyNumber;
            PlayerObject[m_operatingPlayer].SetActive(true);
            return m_playerMoveList[m_selectTargetNumber].gameObject;
        }

        ResetSelectEnemyNumber();
        m_drawStatusValue.EnemyName = m_enemyMoveList[m_selectTargetNumber].MyNumber;
        PlayerObject[m_operatingPlayer].SetActive(false);
        return m_enemyMoveList[m_selectTargetNumber].gameObject;
    }
}
