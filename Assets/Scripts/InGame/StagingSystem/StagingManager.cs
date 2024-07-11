using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// ���o�̃X�e�[�g
/// </summary>
public enum StagingState
{
    enStangingWaiting,  // ���o�̊J�n�҂�
    enStangingStart,    // ���o�J�n
    enStangingEnd,      // ���o�I��
}

public class StagingManager : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject CommandWindow;
    [SerializeField]
    private GameObject CutInObject;
    [SerializeField, Header("���o�I�����ɑҋ@���鎞��(�b)")]
    private float WaitTime = 1.5f;

    private StagingSystem m_stangingSystem;     // ���o�̃V�X�e��
    private LockOnManager m_lockOnSystem;       // ���b�N�I���V�X�e��
    private BattleManager m_battleManager;      // �o�g���}�l�[�W���[
    private BattleSystem m_battleSystem;        // �o�g���V�X�e��
    private TurnManager m_turnManager;
    private CutInManager m_cutInManager;        // �J�b�g�C�����o�̃V�X�e��
    private UIAnimation m_uIAnimation;
    private List<EnemyMove> m_enemyMoveList;
    private List<PlayerMove> m_playerMoveList;
    private SkillDataBase m_skillData;
    private StagingState m_stangingState = StagingState.enStangingWaiting;
    private ActionType m_actionType = ActionType.enNull;
    private int m_targetNumber = 0;

    public StagingState StangingState
    {
        get => m_stangingState;
        set => m_stangingState = value;
    }

    public ActionType ActionType
    {
        get => m_actionType;
        set => m_actionType = value;
    }

    private void Awake()
    {
        m_lockOnSystem = GetComponent<LockOnManager>();
        m_stangingSystem = GetComponent<StagingSystem>();
        m_battleManager = GetComponent<BattleManager>();
        m_battleSystem = GetComponent<BattleSystem>();
        m_turnManager = GetComponent<TurnManager>();
        m_cutInManager = CutInObject.GetComponent<CutInManager>();
        m_skillData = m_battleManager.SkillDataBase;
        m_stangingSystem.SkillDataBase = m_skillData;
    }

    private void Start()
    {
        m_uIAnimation = CommandWindow.GetComponent<UIAnimation>();
        m_enemyMoveList = m_battleManager.EnemyMoveList;
        m_playerMoveList = m_battleManager.PlayerMoveList;
    }

    /// <summary>
    /// �I�����Ă���G�l�~�[�����X�g����폜���邩�ǂ������肷��
    /// </summary>
    /// <param name="number">�G�l�~�[�̔ԍ�</param>
    private void ShouldRemoveEnemyList(int number)
    {
        if(m_lockOnSystem.TargetState != TargetState.enEnemy)
        {
            return;
        }
        // �^�[�Q�b�g���Ђ񎀂łȂ��Ȃ���s���Ȃ�
        if (m_enemyMoveList[number].ActorHPState != ActorHPState.enDie)
        {
            return;
        }
        m_enemyMoveList.Remove(m_enemyMoveList[number]);        // ���X�g����폜
    }

    /// <summary>
    /// �I�����Ă���v���C���[�����X�g����폜���邩�ǂ������肷��
    /// </summary>
    /// <param name="number">�v���C���[�̔ԍ�</param>
    private void ShouldRemovePlayerList(int number)
    {
        if (m_lockOnSystem.TargetState != TargetState.enPlayer)
        {
            return;
        }
        // �^�[�Q�b�g���Ђ񎀂łȂ��Ȃ���s���Ȃ�
        if (m_playerMoveList[number].ActorHPState != ActorHPState.enDie)
        {
            return;
        }
        m_playerMoveList.Remove(m_playerMoveList[number]);      // ���X�g����폜
    }

    /// <summary>
    /// �^�[�Q�b�g��ݒ�
    /// </summary>
    /// <param name="effectRange">�s���̌��ʔ͈�</param>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="myNumber">���g�̔ԍ�</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    public void RegistrationTargets(int targetNumber, int myNumber, int damage, int skillNumber=0, EffectRange effectRange=EffectRange.enOne)
    {
        m_targetNumber = targetNumber;
        m_stangingSystem.Damage = damage;
        // �^�[�Q�b�g�̔ԍ����g�p���Ȃ��s�����I������Ă���ꍇ
        if (ActionType != ActionType.enAttack && ActionType != ActionType.enSkillAttack)
        {
            if (m_turnManager.TurnStatus == TurnStatus.enPlayer)
            {
                m_lockOnSystem.TargetState = TargetState.enPlayer;
            }
            else
            {
                m_lockOnSystem.TargetState = TargetState.enEnemy;
            }
            // �g�p����ԍ������g�̔ԍ��ɐ؂�ւ���
            m_targetNumber = myNumber;
        }
        // �I�u�W�F�N�g���J�����̃^�[�Q�b�g�Ƃ��Đݒ肷��
        switch (effectRange)
        {
            // �P�̍U��
            case EffectRange.enOne:
                if (m_lockOnSystem.TargetState == TargetState.enPlayer)
                {
                    m_stangingSystem.SetCameraTarget(m_playerMoveList[m_targetNumber].transform.GetChild(0).gameObject);
                    break;
                }
                m_stangingSystem.SetCameraTarget(m_enemyMoveList[m_targetNumber].transform.GetChild(0).gameObject);
                break;
            // �S�̍U��
            case EffectRange.enAll:
                if (m_skillData.skillDataList[skillNumber].TargetState == TargetState.enPlayer)
                {
                    // �^�[�Q�b�g��ݒ肵�Ēǉ�����
                    m_stangingSystem.SetCameraTarget(m_playerMoveList[0].transform.GetChild(0).gameObject);
                    for (int i = 1; i < m_playerMoveList.Count; i++)
                    {
                        m_stangingSystem.AddTarget(m_playerMoveList[i].transform.GetChild(0).gameObject);
                    }
                    break;
                }
                m_stangingSystem.SetCameraTarget(m_enemyMoveList[0].transform.GetChild(0).gameObject);
                for (int i = 1; i < m_enemyMoveList.Count; i++)
                {
                    m_stangingSystem.AddTarget(m_enemyMoveList[i].transform.GetChild(0).gameObject);
                }
                break;
        }
        StangingStart(skillNumber, effectRange, m_targetNumber);
    }

    /// <summary>s
    /// ���o���J�n
    /// </summary>
    private void StangingStart(int skillNumber, EffectRange effectRange, int number)
    {
        m_stangingState = StagingState.enStangingStart;
        m_battleManager.StagingStartFlag = true;

        NormalNormalStaging(skillNumber, effectRange, number);
    }

    /// <summary>
    /// �ʏ퉉�o
    /// </summary>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="effectRange">�X�L���̌��ʔ͈�</param>
    /// <param name="number">�^�[�Q�b�g�̔ԍ�</param>
    async private void NormalNormalStaging(int skillNumber, EffectRange effectRange, int number)
    {
        DrawPlayers(true);
        m_stangingSystem.ChangeVcam((int)effectRange);
        // �s�����h�䎞�̏���
        if (ActionType != ActionType.enGuard)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        }
        if (m_battleSystem.WeakFlag == true)
        {
            CutInStart();
            await UniTask.Delay(TimeSpan.FromSeconds(1.3f));
        }
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        // ���o���J�n����
        if (ActionType != ActionType.enGuard)
        {
            m_uIAnimation.ButtonDown_NotActive();
        }
        m_stangingSystem.PlayStaging(ActionType, skillNumber);
        StangingEnd(number);
    }

    /// <summary>
    /// �v���C���[��`�悷��
    /// </summary>
    private void DrawPlayers(bool flag)
    {
        // �h�䎞�͎��s���Ȃ�
        if(ActionType == ActionType.enGuard)
        {
            return;
        }
        for(int i= 0; i < m_playerMoveList.Count; i++)
        {
            if(i == (int)m_battleManager.OperatingPlayer)
            {
                continue;
            }
            m_playerMoveList[i].gameObject.SetActive(flag);
        }
    }

    /// <summary>
    /// ���o���I��
    /// </summary>
    /// <param name="number">�^�[�Q�b�g�̔ԍ�</param>
    async private void StangingEnd(int number)
    {
        if (ActionType != ActionType.enGuard)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(WaitTime));
        }
        ShouldRemoveEnemyList(number);
        ShouldRemovePlayerList(number);
        // �ݒ�����Z�b�g����
        m_stangingSystem.ResetPriority();
        m_uIAnimation.ButtonDown_Active();
        m_battleManager.StagingStartFlag = false;
        m_battleSystem.WeakFlag = false;
        m_battleSystem.HitFlag = false;
        DrawPlayers(false);
        m_stangingState = StagingState.enStangingEnd;
    }

    /// <summary>
    /// �J�b�g�C���̉��o���J�n����
    /// </summary>
    private void CutInStart()
    {
        if(m_turnManager.TurnStatus == TurnStatus.enEnemy)
        {
            return;
        }
        if(m_battleSystem.HitFlag == false)
        {
            return;
        }
        m_cutInManager.CutIn();
        SetWeakFlag();
    }

    /// <summary>
    /// Weak�t���O��ݒ肷��
    /// </summary>
    private void SetWeakFlag()
    {
        m_enemyMoveList[m_targetNumber].WeakFlag = true;
#if UNITY_EDITOR
        Debug.Log("weak!");
#endif
    }
}
