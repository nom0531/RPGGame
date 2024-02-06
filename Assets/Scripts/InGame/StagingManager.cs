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

/// <summary>
/// �e�L�X�g�\���p�̃f�[�^
/// </summary>
public struct TextData
{
    public int value;
    public bool isHit;
    public GameObject gameObject;
}

public class StagingManager : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private SkillDataBase SkillData;
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject CommandWindow;
    [SerializeField, Header("���o�I�����ɑҋ@���鎞��(�b)")]
    private float WaitTime = 1.5f;

    private StagingSystem m_stangingSystem;    // ���o�̃V�X�e��
    private LockOnSystem m_lockOnSystem;        // ���b�N�I���V�X�e��
    private BattleManager m_battleManager;      // �o�g���}�l�[�W���[
    private List<EnemyMove> m_enemyMoveList;
    private List<PlayerMove> m_playerMoveList;
    private List<TextData> m_testDataList;
    private StagingState m_stangingState = StagingState.enStangingWaiting;
    private ActionType m_actionType = ActionType.enNull;

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

    public List<TextData> TextData
    {
        get => m_testDataList;
    }

    /// <summary>
    /// �e�L�X�g�f�[�^��ǉ�����
    /// </summary>
    /// <param name="value">�l</param>
    /// <param name="isHit">�U�����q�b�g�������ǂ���</param>
    public void AddTextData(int value, bool isHit, GameObject gameObject)
    {
        var textData = new TextData() { value = value, isHit = isHit, gameObject = gameObject};
        m_testDataList.Add(textData);
    }

    private void Awake()
    {
        m_lockOnSystem = gameObject.GetComponent<LockOnSystem>();
        m_stangingSystem = gameObject.GetComponent<StagingSystem>();
        m_battleManager = gameObject.GetComponent<BattleManager>();
    }

    private void Start()
    {
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
        m_enemyMoveList[number].gameObject.SetActive(false);    // ��\��
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
        m_playerMoveList[number].gameObject.SetActive(false);   // ��\��
        m_playerMoveList.Remove(m_playerMoveList[number]);      // ���X�g����폜
    }

    /// <summary>
    /// ���o���J�n����
    /// </summary>
    /// <param name="effectRange">�s���̌��ʔ͈�</param>
    /// <param name="turnStatus">�^�[�����񂷑�</param>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="myNumber">���g�̔ԍ�</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    public void RegistrationTargets(TurnStatus turnStatus, int targetNumber, int myNumber, int skillNumber=0, EffectRange effectRange=EffectRange.enOne)
    {
        var number = targetNumber;
        // �^�[�Q�b�g�̔ԍ����g�p���Ȃ��s�����I������Ă���ꍇ
        if (ActionType != ActionType.enAttack && ActionType != ActionType.enSkillAttack)
        {
            if (turnStatus == TurnStatus.enPlayer)
            {
                m_lockOnSystem.TargetState = TargetState.enPlayer;
            }
            else
            {
                m_lockOnSystem.TargetState = TargetState.enEnemy;
            }
            // �g�p����ԍ������g�̔ԍ��ɐ؂�ւ���
            number = myNumber;
        }
        // �I�u�W�F�N�g���J�����̃^�[�Q�b�g�Ƃ��Đݒ肷��
        switch (effectRange)
        {
            // �P�̍U��
            case EffectRange.enOne:
                if (m_lockOnSystem.TargetState == TargetState.enPlayer)
                {
                    m_stangingSystem.SetCameraTarget(m_playerMoveList[number].gameObject);
                    break;
                }
                m_stangingSystem.SetCameraTarget(m_enemyMoveList[number].gameObject);
                break;
            // �S�̍U��
            case EffectRange.enAll:
                if (SkillData.skillDataList[skillNumber].TargetState == TargetState.enPlayer)
                {
                    m_stangingSystem.SetCameraTarget(m_playerMoveList[0].gameObject);   // �ēx�^�[�Q�b�g��ݒ肷��
                    for (int i = 1; i < m_playerMoveList.Count; i++)
                    {
                        m_stangingSystem.AddTarget(m_playerMoveList[i].gameObject);
                    }
                    break;
                }
                m_stangingSystem.SetCameraTarget(m_enemyMoveList[0].gameObject);        // �ēx�^�[�Q�b�g��ݒ肷��
                for (int i = 1; i < m_enemyMoveList.Count; i++)
                {
                    m_stangingSystem.AddTarget(m_enemyMoveList[i].gameObject);
                }
                break;
        }
        // ���o���J�n
        StangingStart(skillNumber, effectRange, number);
    }

    /// <summary>s
    /// ���o���J�n����
    /// </summary>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="effectRange">�X�L���̌��ʔ͈�</param>
    /// <param name="number">�^�[�Q�b�g�̔ԍ�</param>
    private void StangingStart(int skillNumber, EffectRange effectRange, int number)
    {
        m_stangingState = StagingState.enStangingStart;
        CommandWindow.SetActive(false);
        DrawPlayers();
        m_stangingSystem.ChangeVcam((int)effectRange);
        m_stangingSystem.PlayEffect(ActionType, skillNumber);
        //m_stangingSystem.DrawValue(m_testDataList);
        StangingEnd(number);
    }

    /// <summary>
    /// �v���C���[��`�悷��
    /// </summary>
    private void DrawPlayers()
    {
        for(int i= 0; i < m_playerMoveList.Count; i++)
        {
            m_playerMoveList[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// ���o���I������
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
        m_stangingState = StagingState.enStangingEnd;
        CommandWindow.SetActive(true);
    }
}
