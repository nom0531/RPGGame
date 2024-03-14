using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class EnemyTurn : MonoBehaviour
{
    private BattleManager m_battleManager;
    private LockOnManager m_lockOnManager;
    private StagingManager m_stagingManager;
    private TurnManager m_turnManager;

    // Start is called before the first frame update
    private void Start()
    {
        m_battleManager = GetComponent<BattleManager>();
        m_lockOnManager = GetComponent<LockOnManager>();
        m_stagingManager = GetComponent<StagingManager>();
        m_turnManager = GetComponent<TurnManager>();
    }

    /// <summary>
    /// �G�l�~�[�̍s��
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    public void EnemyAction(int myNumber)
    {
        // ���o���J�n���ꂽ�Ȃ���s���Ȃ�
        if (m_stagingManager.StangingState == StagingState.enStangingStart)
        {
            return;
        }
        // �Ђ񎀂Ȃ���s���Ȃ�
        if (m_battleManager.EnemyMoveList[myNumber].ActorHPState == ActorHPState.enDie)
        {
            return;
        }
        var actionType = m_battleManager.EnemyMoveList[myNumber].SelectAttackType();
        var skillNumber = m_battleManager.EnemyMoveList[myNumber].SelectSkill();

        m_lockOnManager.SetTargetState(0, actionType);
        EnemyAction_Move(myNumber, actionType, skillNumber);
    }

    /// <summary>
    /// �G�l�~�[�̍s��
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    /// <param name="actionType">�s���p�^�[��</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    async private void EnemyAction_Move(int myNumber, ActionType actionType, int skillNumber)
    {
        m_battleManager.EnemyMoveList[myNumber].CalculationAbnormalState();
        // �^�[�Q�b�g�̔ԍ����擾����
        var targetNumber = m_battleManager.EnemyMoveList[myNumber].SelectTargetPlayer();
        EnemyAction_Command(myNumber, actionType, skillNumber, targetNumber);
        // ���o���J�n����
        m_stagingManager.ActionType = actionType;
        m_stagingManager.RegistrationTargets(m_turnManager.TurnStatus, false, targetNumber, myNumber);
        m_battleManager.EnemyMoveList[myNumber].ActionEnd(actionType, skillNumber);
        // ���o���I�������Ȃ�ȉ��̏��������s����
        await UniTask.WaitUntil(() => m_stagingManager.StangingState == StagingState.enStangingEnd);
        // �ŏ�Ԏ��̃_���[�W��^����
        m_battleManager.EnemyMoveList[myNumber].DecrementHP(m_battleManager.EnemyMoveList[myNumber].PoisonDamage);
    }

    /// <summary>
    /// �s������
    /// </summary>
    private void EnemyAction_Command(int myNumber, ActionType actionType, int skillNumber, int targetNumber)
    {
        // ���ɍs�����Ă���Ȃ���s���Ȃ�
        if (m_battleManager.EnemyMoveList[myNumber].ActionEndFlag == true)
        {
            return;
        }

        switch (actionType)
        {
            case ActionType.enAttack:
                m_battleManager.EnemyMoveList[myNumber].EnemyAction_Attack(targetNumber, m_battleManager.PlayerMoveList[targetNumber].PlayerStatus.DEF);
                break;
            case ActionType.enSkillAttack:
                var value = 0;
                switch (m_battleManager.EnemyDataBase.enemyDataList[myNumber].skillDataList[skillNumber].SkillType)
                {
                    // �^�C�v�F�U��
                    case SkillType.enAttack:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (m_battleManager.EnemyDataBase.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_battleManager.PlayerMoveList.Count; playerNumber++)
                            {
                                m_battleManager.EnemyMoveList[myNumber].EnemyAction_SkillAttack(
                                    skillNumber,                                                            // �X�L���̔ԍ�
                                    playerNumber,                                                           // �^�[�Q�b�g�̔ԍ�
                                    m_battleManager.PlayerMoveList[playerNumber].PlayerStatus.DEF,          // �h���
                                    m_battleManager.PlayerMoveList[playerNumber].MyNumber                   // �v���C���[�̔ԍ�
                                    );
                            }
                            break;
                        }
                        m_battleManager.EnemyMoveList[myNumber].EnemyAction_SkillAttack(
                            skillNumber,                                                                    // �X�L���̔ԍ�
                            targetNumber,                                                                   // �^�[�Q�b�g�̔ԍ�
                            m_battleManager.PlayerMoveList[targetNumber].PlayerStatus.DEF,                  // �h���
                            m_battleManager.PlayerMoveList[targetNumber].MyNumber                           // �v���C���[�̔ԍ�
                            );
                        break;
                    // �^�C�v�F�o�t
                    case SkillType.enBuff:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (m_battleManager.EnemyDataBase.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int enemyNumber = 0; enemyNumber < m_battleManager.EnemyMoveList.Count; enemyNumber++)
                            {
                                value = m_battleManager.EnemyMoveList[myNumber].EnemyAction_Buff(
                                    skillNumber,                                                            // �X�L���̔ԍ�
                                    m_battleManager.EnemyMoveList[enemyNumber].EnemyStatus.ATK,             // �U����
                                    m_battleManager.EnemyMoveList[enemyNumber].EnemyStatus.DEF,             // �h���
                                    m_battleManager.EnemyMoveList[enemyNumber].EnemyStatus.SPD              // �f����
                                    );
                                m_battleManager.EnemyMoveList[enemyNumber].SetBuffStatus(
                                    m_battleManager.EnemyDataBase.enemyDataList[myNumber].skillDataList[skillNumber].BuffType,
                                    value,
                                    skillNumber,
                                    true);
                            }
                            break;
                        }
                        // �^�[�Q�b�g���đI��
                        targetNumber = m_battleManager.EnemyMoveList[myNumber].SelectTargetEnemy(m_battleManager.EnemyMoveList.Count);
                        value = m_battleManager.EnemyMoveList[myNumber].EnemyAction_Buff(
                            skillNumber,                                                                    // �X�L���̔ԍ�
                            m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.ATK,                    // �U����
                            m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.DEF,                    // �h���
                            m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.SPD                     // �f����
                            );
                        m_battleManager.EnemyMoveList[targetNumber].SetBuffStatus(
                            m_battleManager.EnemyDataBase.enemyDataList[myNumber].skillDataList[skillNumber].BuffType,
                            value,
                            skillNumber,
                            true);
                        break;
                    // �^�C�v�F�f�o�t
                    case SkillType.enDeBuff:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (m_battleManager.EnemyDataBase.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_battleManager.EnemyMoveList.Count; playerNumber++)
                            {
                                value = m_battleManager.EnemyMoveList[myNumber].EnemyAction_Buff(
                                    skillNumber,                                                            // �X�L���̔ԍ�
                                    m_battleManager.PlayerMoveList[playerNumber].PlayerStatus.ATK,          // �U����
                                    m_battleManager.PlayerMoveList[playerNumber].PlayerStatus.DEF,          // �h���
                                    m_battleManager.PlayerMoveList[playerNumber].PlayerStatus.SPD           // �f����
                                    );
                                m_battleManager.PlayerMoveList[playerNumber].SetBuffStatus(
                                    m_battleManager.EnemyDataBase.enemyDataList[myNumber].skillDataList[skillNumber].BuffType,
                                    value,
                                    skillNumber,
                                    false);
                            }
                            break;
                        }
                        m_battleManager.EnemyMoveList[myNumber].EnemyAction_Buff(
                            skillNumber,                                                                    // �X�L���̔ԍ�
                            m_battleManager.PlayerMoveList[targetNumber].PlayerStatus.ATK,                  // �U����
                            m_battleManager.PlayerMoveList[targetNumber].PlayerStatus.DEF,                  // �h���
                            m_battleManager.PlayerMoveList[targetNumber].PlayerStatus.SPD                   // �f����
                            );
                        m_battleManager.PlayerMoveList[targetNumber].SetBuffStatus(
                            m_battleManager.EnemyDataBase.enemyDataList[myNumber].skillDataList[skillNumber].BuffType,
                            value,
                            skillNumber,
                            false);
                        break;
                    // �^�C�v�F��
                    case SkillType.enHeal:
                    case SkillType.enResurrection:
                        // �^�[�Q�b�g��I��
                        targetNumber = m_battleManager.EnemyMoveList[myNumber].SelectTargetEnemy(m_battleManager.EnemyMoveList.Count);
                        m_battleManager.EnemyMoveList[myNumber].EnemyAction_HPResurrection(skillNumber, targetNumber);
                        m_battleManager.EnemyMoveList[myNumber].EnemyAction_HPRecover(m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.HP, skillNumber);
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (m_battleManager.EnemyDataBase.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            EnemyAction_AllRecover(m_battleManager.EnemyMoveList[myNumber].BasicValue);
                            return;
                        }
                        // HP���񕜂�����
                        m_battleManager.EnemyMoveList[targetNumber].RecoverHP(m_battleManager.EnemyMoveList[myNumber].BasicValue);
                        break;
                }
                break;
            case ActionType.enGuard:
                m_battleManager.EnemyMoveList[myNumber].EnemyAction_Guard();
                break;
            case ActionType.enEscape:
                m_battleManager.EnemyMoveList[myNumber].EnemyAction_Escape();
                break;
            case ActionType.enNull:
                break;
        }
    }

    /// <summary>
    /// �S�̂��񕜂�����
    /// </summary>
    /// <param name="recoverValue">�񕜗�</param>
    public void EnemyAction_AllRecover(int recoverValue)
    {
        for (int enemyNumber = 0; enemyNumber < m_battleManager.EnemyMoveList.Count; enemyNumber++)
        {
            m_battleManager.EnemyMoveList[enemyNumber].RecoverHP(recoverValue);
        }
    }
}
