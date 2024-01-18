using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// ���o�̃X�e�[�g
/// </summary>
public enum StangingState
{
    enStangingWaiting,  // ���o�̊J�n�҂�
    enStangingStart,    // ���o�J�n
    enStangingEnd,      // ���o�I��
}

public class StagingManager : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject CommandWindow;
    [SerializeField, Header("���o�I�����ɑҋ@���鎞��(�b)")]
    private float WaitTime = 1.5f;

    private StangingState m_stangingState = StangingState.enStangingWaiting;
    private StangingSystem m_stangingSystem;    // ���o�̃V�X�e��
    private LockOnSystem m_lockOnSystem;        // ���b�N�I���V�X�e��
    private BattleManager m_battleManager;      // �o�g���}�l�[�W���[
    private BattleSystem m_battleSystem;        // �o�g���V�X�e��
    private List<EnemyMove> m_enemyMoveList;
    private List<PlayerMove> m_playerMoveList;


    public StangingState StangingState
    {
        get => m_stangingState;
        set => m_stangingState = value;
    }

    private void Awake()
    {
        m_lockOnSystem = gameObject.GetComponent<LockOnSystem>();
        m_stangingSystem = gameObject.GetComponent<StangingSystem>();
        m_battleManager = gameObject.GetComponent<BattleManager>();
        m_battleSystem = gameObject.GetComponent<BattleSystem>();
    }

    private void Start()
    {
        m_enemyMoveList = m_battleManager.EnemyMoveList;
        m_playerMoveList = m_battleManager.PlayerMoveList;
    }

    /// <summary>
    /// ���X�g����폜���邩�ǂ���
    /// </summary>
    /// <param name="number">�G�l�~�[�̔ԍ�</param>
    private void RemoveSelectEnemy(int number)
    {
        // �^�[�Q�b�g���Ђ񎀂łȂ��Ȃ���s���Ȃ�
        if (m_enemyMoveList[number].ActorHPState != ActorHPState.enDie)
        {
            return;
        }
        // ���X�g����폜
        m_enemyMoveList.Remove(m_enemyMoveList[number]);
    }

    /// <summary>
    /// ���o���J�n����
    /// </summary>
    /// <param name="effectRange">�s���̌��ʔ͈�</param>
    /// <param name="actionType">�s���p�^�[��</param>
    /// <param name="turnStatus">�^�[�����񂷑�</param>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="myNumber">���g�̔ԍ�</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    public void RegistrationTargets(ActionType actionType, TurnStatus turnStatus, int targetNumber, int myNumber, int skillNumber=0, EffectRange effectRange=EffectRange.enOne)
    {
        var number = targetNumber;
        // �^�[�Q�b�g�̔ԍ����g�p���Ȃ��s�����I������Ă���ꍇ
        if (actionType != ActionType.enAttack && actionType != ActionType.enSkillAttack)
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
        // �^�[�Q�b�g���G�l�~�[�̂Ƃ�
        if(m_lockOnSystem.TargetState == TargetState.enEnemy)
        {
            RemoveSelectEnemy(number);
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
                //if (m_lockOnSystem.TargetState == TargetState.enPlayer)
                //{
                //    for (int i = 0; i > m_playerMoveList.Count; i++)
                //    {
                //        m_stangingSystem.AddTargetList(m_playerMoveList[i].gameObject);
                //    }
                //    break;
                //}
                //for (int i = 0; i > m_enemyMoveList.Count; i++)
                //{
                //    m_stangingSystem.AddTargetList(m_playerMoveList[i].gameObject);
                //}
                break;
        }
        // ���o���J�n
        StangingStart(actionType, skillNumber, effectRange);
    }

    /// <summary>s
    /// ���o���J�n����
    /// </summary>
    private void StangingStart(ActionType actionType, int skillNumber, EffectRange effectRange)
    {
        m_stangingState = StangingState.enStangingStart;
        CommandWindow.SetActive(false);
        DrawTargets();
        // �J�������ړ�����
        m_stangingSystem.ChangeVcam((int)effectRange);
        // �G�t�F�N�g���Đ�����
        m_stangingSystem.PlayEffect(actionType, skillNumber);
        StangingEnd(actionType);
    }

    /// <summary>
    /// �I�u�W�F�N�g��`�悷��
    /// </summary>
    private void DrawTargets()
    {
        for(int i= 0; i < m_playerMoveList.Count; i++)
        {
            m_playerMoveList[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// ���o���I������
    /// </summary>
    async private void StangingEnd(ActionType actionType)
    {
        if (actionType != ActionType.enGuard)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(WaitTime));
        }
        // �ݒ�����Z�b�g����
        m_stangingSystem.ResetPriority();
        m_stangingState = StangingState.enStangingEnd;
        CommandWindow.SetActive(true);
    }
}
