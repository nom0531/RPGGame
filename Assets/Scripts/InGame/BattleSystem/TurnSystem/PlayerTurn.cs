using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerTurn : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private BattleButton[] BattleButton;

    private BattleManager m_battleManager;
    private BattleSystem m_battleSystem;
    private StagingManager m_stagingManager;
    private LockOnManager m_lockOnManager;
    private TurnManager m_turnManager;

    private void Start()
    {
        m_battleManager = GetComponent<BattleManager>();
        m_battleSystem = GetComponent<BattleSystem>();
        m_stagingManager = GetComponent<StagingManager>();
        m_lockOnManager = GetComponent<LockOnManager>();
        m_turnManager = GetComponent<TurnManager>();
    }

    /// <summary>
    /// �v���C���[�̍s��
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    async public void PlayerAction(int myNumber)
    {
        // ���o���J�n���ꂽ�Ȃ���s���Ȃ�
        if (m_stagingManager.StangingState == StagingState.enStangingStart)
        {
            return;
        }
        // ���ɍs�����Ă���Ȃ�s���͂��Ȃ�
        if (m_battleManager.PlayerMoveList[(int)m_battleManager.OperatingPlayer].ActionEndFlag == true)
        {
            // ���̃v���C���[��ݒ肷��
            m_battleManager.OperatingPlayer = m_battleManager.NextOperatingPlayer();
            return;
        }

        // ���������ꂩ�̃{�^���������ꂽ��ȉ��̏��������s����
        if (BattleButton[0].ButtonDown == true || BattleButton[1].ButtonDown == true || BattleButton[2].ButtonDown == true)
        {
            var skillNumber = m_battleManager.PlayerMoveList[myNumber].SelectSkillNumber;
            var targetNumber = 0;

            // �K�[�h�ȊO�̃R�}���h  ���@�P�̍U���Ȃ�
            if (m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].EffectRange != EffectRange.enAll
                && m_battleManager.PlayerMoveList[myNumber].NextActionType != ActionType.enGuard)
            {
                m_lockOnManager.SetTargetState(m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].ID,
                    m_battleManager.PlayerMoveList[myNumber].NextActionType);
                // �U���Ώۂ��I�����ꂽ��ȉ��̏��������s����
                await UniTask.WaitUntil(() => m_lockOnManager.ButtonDown == true);
                // �Ώۂ��Đݒ肷��
                skillNumber = m_battleManager.PlayerMoveList[myNumber].SelectSkillNumber;
                targetNumber = m_lockOnManager.TargetNumber;
            }
            PlayerAction_Move(myNumber, skillNumber, targetNumber);
        }
    }

    /// <summary>
    /// �v���C���[�̍s��
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    async private void PlayerAction_Move(int myNumber, int skillNumber, int targetNumber)
    {
        m_battleManager.PlayerMoveList[myNumber].CalculationAbnormalState();
        PlayerAction_Command(myNumber, targetNumber, m_battleManager.PlayerMoveList[myNumber].NextActionType, skillNumber);
        // ���o���J�n����
        m_stagingManager.ActionType = m_battleManager.PlayerMoveList[myNumber].NextActionType;
        m_stagingManager.RegistrationTargets(m_turnManager.TurnStatus, targetNumber, myNumber, m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].ID,
            m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].EffectRange);
        // �s�����I������
        m_battleManager.PlayerMoveList[myNumber].ActionEnd(m_battleManager.PlayerMoveList[myNumber].NextActionType, skillNumber);
        // ���o���I�������Ȃ�ȉ��̏��������s����
        await UniTask.WaitUntil(() => m_stagingManager.StangingState == StagingState.enStangingEnd);
        m_battleManager.PlayerMoveList[myNumber].DecrementHP(m_battleManager.PlayerMoveList[myNumber].PoisonDamage);
        // ���̃v���C���[��ݒ肷��
        m_battleManager.OperatingPlayer = m_battleManager.NextOperatingPlayer();
        // ���b�N�I���̐ݒ���������E�Đݒ肷��
        m_lockOnManager.ButtonDown = false;
        m_lockOnManager.ResetCinemachine();
    }

    /// <summary>
    /// �s������
    /// </summary>
    private void PlayerAction_Command(int myNumber, int targetNumber, ActionType actionType, int skillNumber)
    {
        // ���ɍs�����Ă���Ȃ���s���Ȃ�
        if (m_battleManager.PlayerMoveList[myNumber].ActionEndFlag == true)
        {
            return;
        }
        // �s��
        switch (actionType)
        {
            case ActionType.enAttack:
                var DEF = m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.DEF;
                // ������ԂȂ�^�[�Q�b�g���Đݒ肷��
                if (m_battleManager.PlayerMoveList[myNumber].ConfusionFlag == true)
                {
                    targetNumber = m_battleSystem.GetRandomValue(0, m_battleManager.PlayerMoveList.Count);
                    DEF = m_battleManager.PlayerMoveList[targetNumber].PlayerStatus.DEF;
                }
                m_battleManager.PlayerMoveList[myNumber].PlayerAction_Attack(targetNumber, DEF);
                break;
            case ActionType.enSkillAttack:
                var value = 0;
                switch (m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].SkillType)
                {
                    case SkillType.enAttack:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int enemyNumber = 0; enemyNumber < m_battleManager.EnemySum; enemyNumber++)
                            {
                                m_battleManager.PlayerMoveList[myNumber].PlayerAction_SkillAttack(
                                    skillNumber,                                                        // �X�L���̔ԍ�
                                    enemyNumber,                                                        // �^�[�Q�b�g�̔ԍ�
                                    m_battleManager.EnemyMoveList[enemyNumber].EnemyStatus.DEF,         // �h���
                                    m_battleManager.EnemyMoveList[enemyNumber].MyNumber                 // �G�l�~�[�̔ԍ�
                                    );
                            }
                            break;
                        }
                        m_battleManager.PlayerMoveList[myNumber].PlayerAction_SkillAttack(
                            skillNumber,                                                                // �X�L���̔ԍ�
                            targetNumber,                                                               // �^�[�Q�b�g�̔ԍ�
                            m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.DEF,                // �h���
                            m_battleManager.EnemyMoveList[targetNumber].MyNumber                        // �G�l�~�[�̔ԍ�
                            );
                        break;
                    case SkillType.enBuff:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_battleManager.PlayerMoveList.Count; playerNumber++)
                            {
                                value = m_battleManager.PlayerMoveList[myNumber].PlayerAction_Buff(
                                    skillNumber,                                                        // �X�L���̔ԍ�
                                    m_battleManager.PlayerMoveList[playerNumber].PlayerStatus.ATK,      // �U����
                                    m_battleManager.PlayerMoveList[playerNumber].PlayerStatus.DEF,      // �h���
                                    m_battleManager.PlayerMoveList[playerNumber].PlayerStatus.SPD       // �f����
                                    );
                                // �l��ݒ肷��
                                m_battleManager.PlayerMoveList[targetNumber].SetBuffStatus(
                                    m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].BuffType,
                                    value,
                                    skillNumber,
                                    true);
                            }
                            break;
                        }
                        value = m_battleManager.PlayerMoveList[myNumber].PlayerAction_Buff(
                            skillNumber,                                                // �X�L���̔ԍ�
                            m_battleManager.PlayerMoveList[targetNumber].PlayerStatus.ATK,              // �U����
                            m_battleManager.PlayerMoveList[targetNumber].PlayerStatus.DEF,              // �h���
                            m_battleManager.PlayerMoveList[targetNumber].PlayerStatus.SPD               // �f����
                            );
                        // �l��ݒ肷��
                        m_battleManager.PlayerMoveList[targetNumber].SetBuffStatus(
                            m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].BuffType,
                            value,
                            skillNumber,
                            true);
                        break;
                    case SkillType.enDeBuff:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int enemyNumber = 0; enemyNumber < m_battleManager.EnemyMoveList.Count; enemyNumber++)
                            {
                                value = m_battleManager.PlayerMoveList[myNumber].PlayerAction_Buff(
                                    skillNumber,                                                    // �X�L���̔ԍ�
                                    m_battleManager.EnemyMoveList[enemyNumber].EnemyStatus.ATK,     // �U����
                                    m_battleManager.EnemyMoveList[enemyNumber].EnemyStatus.DEF,     // �h���
                                    m_battleManager.EnemyMoveList[enemyNumber].EnemyStatus.SPD      // �f����
                                    );
                                // �l��ݒ肷��
                                m_battleManager.EnemyMoveList[enemyNumber].SetBuffStatus(
                                    m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].BuffType,
                                    value,
                                    skillNumber,
                                    false);
                            }
                            break;
                        }
                        value = m_battleManager.PlayerMoveList[myNumber].PlayerAction_Buff(
                            skillNumber,                                                            // �X�L���̔ԍ�
                            m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.ATK,            // �U����
                            m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.DEF,            // �h���
                            m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.SPD             // �f����
                            );
                        // �l��ݒ肷��
                        m_battleManager.EnemyMoveList[targetNumber].SetBuffStatus(
                            m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].BuffType,
                            value,
                            skillNumber,
                            false);
                        break;
                    case SkillType.enHeal:
                    case SkillType.enResurrection:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_battleManager.PlayerMoveList.Count; playerNumber++)
                            {
                                m_battleManager.PlayerMoveList[myNumber].PlayerAction_HPRecover(playerNumber, skillNumber);
                                m_battleManager.PlayerMoveList[playerNumber].RecoverHP(m_battleManager.PlayerMoveList[myNumber].BasicValue);
                            }
                            break;
                        }
                        m_battleManager.PlayerMoveList[myNumber].PlayerAction_HPRecover(targetNumber, skillNumber);
                        m_battleManager.PlayerMoveList[targetNumber].RecoverHP(m_battleManager.PlayerMoveList[myNumber].BasicValue);
                        break;
                }
                break;
            case ActionType.enGuard:
                m_battleManager.PlayerMoveList[myNumber].PlayerAction_Guard();
                break;
            case ActionType.enNull:
                break;
        }
    }
}
