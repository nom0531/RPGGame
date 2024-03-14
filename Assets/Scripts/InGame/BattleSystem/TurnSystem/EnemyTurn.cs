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
    /// エネミーの行動
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    public void EnemyAction(int myNumber)
    {
        // 演出が開始されたなら実行しない
        if (m_stagingManager.StangingState == StagingState.enStangingStart)
        {
            return;
        }
        // ひん死なら実行しない
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
    /// エネミーの行動
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    /// <param name="actionType">行動パターン</param>
    /// <param name="skillNumber">スキルの番号</param>
    async private void EnemyAction_Move(int myNumber, ActionType actionType, int skillNumber)
    {
        m_battleManager.EnemyMoveList[myNumber].CalculationAbnormalState();
        // ターゲットの番号を取得する
        var targetNumber = m_battleManager.EnemyMoveList[myNumber].SelectTargetPlayer();
        EnemyAction_Command(myNumber, actionType, skillNumber, targetNumber);
        // 演出を開始する
        m_stagingManager.ActionType = actionType;
        m_stagingManager.RegistrationTargets(m_turnManager.TurnStatus, false, targetNumber, myNumber);
        m_battleManager.EnemyMoveList[myNumber].ActionEnd(actionType, skillNumber);
        // 演出が終了したなら以下の処理を実行する
        await UniTask.WaitUntil(() => m_stagingManager.StangingState == StagingState.enStangingEnd);
        // 毒状態時のダメージを与える
        m_battleManager.EnemyMoveList[myNumber].DecrementHP(m_battleManager.EnemyMoveList[myNumber].PoisonDamage);
    }

    /// <summary>
    /// 行動処理
    /// </summary>
    private void EnemyAction_Command(int myNumber, ActionType actionType, int skillNumber, int targetNumber)
    {
        // 既に行動しているなら実行しない
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
                    // タイプ：攻撃
                    case SkillType.enAttack:
                        // 効果範囲が全体のとき
                        if (m_battleManager.EnemyDataBase.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_battleManager.PlayerMoveList.Count; playerNumber++)
                            {
                                m_battleManager.EnemyMoveList[myNumber].EnemyAction_SkillAttack(
                                    skillNumber,                                                            // スキルの番号
                                    playerNumber,                                                           // ターゲットの番号
                                    m_battleManager.PlayerMoveList[playerNumber].PlayerStatus.DEF,          // 防御力
                                    m_battleManager.PlayerMoveList[playerNumber].MyNumber                   // プレイヤーの番号
                                    );
                            }
                            break;
                        }
                        m_battleManager.EnemyMoveList[myNumber].EnemyAction_SkillAttack(
                            skillNumber,                                                                    // スキルの番号
                            targetNumber,                                                                   // ターゲットの番号
                            m_battleManager.PlayerMoveList[targetNumber].PlayerStatus.DEF,                  // 防御力
                            m_battleManager.PlayerMoveList[targetNumber].MyNumber                           // プレイヤーの番号
                            );
                        break;
                    // タイプ：バフ
                    case SkillType.enBuff:
                        // 効果範囲が全体のとき
                        if (m_battleManager.EnemyDataBase.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int enemyNumber = 0; enemyNumber < m_battleManager.EnemyMoveList.Count; enemyNumber++)
                            {
                                value = m_battleManager.EnemyMoveList[myNumber].EnemyAction_Buff(
                                    skillNumber,                                                            // スキルの番号
                                    m_battleManager.EnemyMoveList[enemyNumber].EnemyStatus.ATK,             // 攻撃力
                                    m_battleManager.EnemyMoveList[enemyNumber].EnemyStatus.DEF,             // 防御力
                                    m_battleManager.EnemyMoveList[enemyNumber].EnemyStatus.SPD              // 素早さ
                                    );
                                m_battleManager.EnemyMoveList[enemyNumber].SetBuffStatus(
                                    m_battleManager.EnemyDataBase.enemyDataList[myNumber].skillDataList[skillNumber].BuffType,
                                    value,
                                    skillNumber,
                                    true);
                            }
                            break;
                        }
                        // ターゲットを再選択
                        targetNumber = m_battleManager.EnemyMoveList[myNumber].SelectTargetEnemy(m_battleManager.EnemyMoveList.Count);
                        value = m_battleManager.EnemyMoveList[myNumber].EnemyAction_Buff(
                            skillNumber,                                                                    // スキルの番号
                            m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.ATK,                    // 攻撃力
                            m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.DEF,                    // 防御力
                            m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.SPD                     // 素早さ
                            );
                        m_battleManager.EnemyMoveList[targetNumber].SetBuffStatus(
                            m_battleManager.EnemyDataBase.enemyDataList[myNumber].skillDataList[skillNumber].BuffType,
                            value,
                            skillNumber,
                            true);
                        break;
                    // タイプ：デバフ
                    case SkillType.enDeBuff:
                        // 効果範囲が全体のとき
                        if (m_battleManager.EnemyDataBase.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_battleManager.EnemyMoveList.Count; playerNumber++)
                            {
                                value = m_battleManager.EnemyMoveList[myNumber].EnemyAction_Buff(
                                    skillNumber,                                                            // スキルの番号
                                    m_battleManager.PlayerMoveList[playerNumber].PlayerStatus.ATK,          // 攻撃力
                                    m_battleManager.PlayerMoveList[playerNumber].PlayerStatus.DEF,          // 防御力
                                    m_battleManager.PlayerMoveList[playerNumber].PlayerStatus.SPD           // 素早さ
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
                            skillNumber,                                                                    // スキルの番号
                            m_battleManager.PlayerMoveList[targetNumber].PlayerStatus.ATK,                  // 攻撃力
                            m_battleManager.PlayerMoveList[targetNumber].PlayerStatus.DEF,                  // 防御力
                            m_battleManager.PlayerMoveList[targetNumber].PlayerStatus.SPD                   // 素早さ
                            );
                        m_battleManager.PlayerMoveList[targetNumber].SetBuffStatus(
                            m_battleManager.EnemyDataBase.enemyDataList[myNumber].skillDataList[skillNumber].BuffType,
                            value,
                            skillNumber,
                            false);
                        break;
                    // タイプ：回復
                    case SkillType.enHeal:
                    case SkillType.enResurrection:
                        // ターゲットを選択
                        targetNumber = m_battleManager.EnemyMoveList[myNumber].SelectTargetEnemy(m_battleManager.EnemyMoveList.Count);
                        m_battleManager.EnemyMoveList[myNumber].EnemyAction_HPResurrection(skillNumber, targetNumber);
                        m_battleManager.EnemyMoveList[myNumber].EnemyAction_HPRecover(m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.HP, skillNumber);
                        // 効果範囲が全体のとき
                        if (m_battleManager.EnemyDataBase.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            EnemyAction_AllRecover(m_battleManager.EnemyMoveList[myNumber].BasicValue);
                            return;
                        }
                        // HPを回復させる
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
    /// 全体を回復させる
    /// </summary>
    /// <param name="recoverValue">回復量</param>
    public void EnemyAction_AllRecover(int recoverValue)
    {
        for (int enemyNumber = 0; enemyNumber < m_battleManager.EnemyMoveList.Count; enemyNumber++)
        {
            m_battleManager.EnemyMoveList[enemyNumber].RecoverHP(recoverValue);
        }
    }
}
