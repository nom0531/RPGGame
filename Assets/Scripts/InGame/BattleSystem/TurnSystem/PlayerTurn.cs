using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerTurn : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト")]
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
    /// プレイヤーの行動
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    async public void PlayerAction(int myNumber)
    {
        // 演出が開始されたなら実行しない
        if (m_stagingManager.StangingState == StagingState.enStangingStart)
        {
            return;
        }
        // 既に行動しているなら行動はしない
        if (m_battleManager.PlayerMoveList[(int)m_battleManager.OperatingPlayer].ActionEndFlag == true)
        {
            // 次のプレイヤーを設定する
            m_battleManager.OperatingPlayer = m_battleManager.NextOperatingPlayer();
            return;
        }

        // もしいずれかのボタンが押されたら以下の処理を実行する
        if (BattleButton[0].ButtonDown == true || BattleButton[1].ButtonDown == true || BattleButton[2].ButtonDown == true)
        {
            var skillNumber = m_battleManager.PlayerMoveList[myNumber].SelectSkillNumber;
            var targetNumber = 0;

            // ガード以外のコマンド  かつ　単体攻撃なら
            if (m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].EffectRange != EffectRange.enAll
                && m_battleManager.PlayerMoveList[myNumber].NextActionType != ActionType.enGuard)
            {
                m_lockOnManager.SetTargetState(m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].ID,
                    m_battleManager.PlayerMoveList[myNumber].NextActionType);
                // 攻撃対象が選択されたら以下の処理を実行する
                await UniTask.WaitUntil(() => m_lockOnManager.ButtonDown == true);
                // 対象を再設定する
                skillNumber = m_battleManager.PlayerMoveList[myNumber].SelectSkillNumber;
                targetNumber = m_lockOnManager.TargetNumber;
            }
            PlayerAction_Move(myNumber, skillNumber, targetNumber);
        }
    }

    /// <summary>
    /// プレイヤーの行動
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="targetNumber">ターゲットの番号</param>
    async private void PlayerAction_Move(int myNumber, int skillNumber, int targetNumber)
    {
        m_battleManager.PlayerMoveList[myNumber].CalculationAbnormalState();
        PlayerAction_Command(myNumber, targetNumber, m_battleManager.PlayerMoveList[myNumber].NextActionType, skillNumber);
        // 演出を開始する
        m_stagingManager.ActionType = m_battleManager.PlayerMoveList[myNumber].NextActionType;
        m_stagingManager.RegistrationTargets(m_turnManager.TurnStatus, targetNumber, myNumber, m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].ID,
            m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].EffectRange);
        // 行動を終了する
        m_battleManager.PlayerMoveList[myNumber].ActionEnd(m_battleManager.PlayerMoveList[myNumber].NextActionType, skillNumber);
        // 演出が終了したなら以下の処理を実行する
        await UniTask.WaitUntil(() => m_stagingManager.StangingState == StagingState.enStangingEnd);
        m_battleManager.PlayerMoveList[myNumber].DecrementHP(m_battleManager.PlayerMoveList[myNumber].PoisonDamage);
        // 次のプレイヤーを設定する
        m_battleManager.OperatingPlayer = m_battleManager.NextOperatingPlayer();
        // ロックオンの設定を初期化・再設定する
        m_lockOnManager.ButtonDown = false;
        m_lockOnManager.ResetCinemachine();
    }

    /// <summary>
    /// 行動処理
    /// </summary>
    private void PlayerAction_Command(int myNumber, int targetNumber, ActionType actionType, int skillNumber)
    {
        // 既に行動しているなら実行しない
        if (m_battleManager.PlayerMoveList[myNumber].ActionEndFlag == true)
        {
            return;
        }
        // 行動
        switch (actionType)
        {
            case ActionType.enAttack:
                var DEF = m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.DEF;
                // 混乱状態ならターゲットを再設定する
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
                        // 効果範囲が全体のとき
                        if (m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int enemyNumber = 0; enemyNumber < m_battleManager.EnemySum; enemyNumber++)
                            {
                                m_battleManager.PlayerMoveList[myNumber].PlayerAction_SkillAttack(
                                    skillNumber,                                                        // スキルの番号
                                    enemyNumber,                                                        // ターゲットの番号
                                    m_battleManager.EnemyMoveList[enemyNumber].EnemyStatus.DEF,         // 防御力
                                    m_battleManager.EnemyMoveList[enemyNumber].MyNumber                 // エネミーの番号
                                    );
                            }
                            break;
                        }
                        m_battleManager.PlayerMoveList[myNumber].PlayerAction_SkillAttack(
                            skillNumber,                                                                // スキルの番号
                            targetNumber,                                                               // ターゲットの番号
                            m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.DEF,                // 防御力
                            m_battleManager.EnemyMoveList[targetNumber].MyNumber                        // エネミーの番号
                            );
                        break;
                    case SkillType.enBuff:
                        // 効果範囲が全体のとき
                        if (m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_battleManager.PlayerMoveList.Count; playerNumber++)
                            {
                                value = m_battleManager.PlayerMoveList[myNumber].PlayerAction_Buff(
                                    skillNumber,                                                        // スキルの番号
                                    m_battleManager.PlayerMoveList[playerNumber].PlayerStatus.ATK,      // 攻撃力
                                    m_battleManager.PlayerMoveList[playerNumber].PlayerStatus.DEF,      // 防御力
                                    m_battleManager.PlayerMoveList[playerNumber].PlayerStatus.SPD       // 素早さ
                                    );
                                // 値を設定する
                                m_battleManager.PlayerMoveList[targetNumber].SetBuffStatus(
                                    m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].BuffType,
                                    value,
                                    skillNumber,
                                    true);
                            }
                            break;
                        }
                        value = m_battleManager.PlayerMoveList[myNumber].PlayerAction_Buff(
                            skillNumber,                                                // スキルの番号
                            m_battleManager.PlayerMoveList[targetNumber].PlayerStatus.ATK,              // 攻撃力
                            m_battleManager.PlayerMoveList[targetNumber].PlayerStatus.DEF,              // 防御力
                            m_battleManager.PlayerMoveList[targetNumber].PlayerStatus.SPD               // 素早さ
                            );
                        // 値を設定する
                        m_battleManager.PlayerMoveList[targetNumber].SetBuffStatus(
                            m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].BuffType,
                            value,
                            skillNumber,
                            true);
                        break;
                    case SkillType.enDeBuff:
                        // 効果範囲が全体のとき
                        if (m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int enemyNumber = 0; enemyNumber < m_battleManager.EnemyMoveList.Count; enemyNumber++)
                            {
                                value = m_battleManager.PlayerMoveList[myNumber].PlayerAction_Buff(
                                    skillNumber,                                                    // スキルの番号
                                    m_battleManager.EnemyMoveList[enemyNumber].EnemyStatus.ATK,     // 攻撃力
                                    m_battleManager.EnemyMoveList[enemyNumber].EnemyStatus.DEF,     // 防御力
                                    m_battleManager.EnemyMoveList[enemyNumber].EnemyStatus.SPD      // 素早さ
                                    );
                                // 値を設定する
                                m_battleManager.EnemyMoveList[enemyNumber].SetBuffStatus(
                                    m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].BuffType,
                                    value,
                                    skillNumber,
                                    false);
                            }
                            break;
                        }
                        value = m_battleManager.PlayerMoveList[myNumber].PlayerAction_Buff(
                            skillNumber,                                                            // スキルの番号
                            m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.ATK,            // 攻撃力
                            m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.DEF,            // 防御力
                            m_battleManager.EnemyMoveList[targetNumber].EnemyStatus.SPD             // 素早さ
                            );
                        // 値を設定する
                        m_battleManager.EnemyMoveList[targetNumber].SetBuffStatus(
                            m_battleManager.PlayerDataBase.playerDataList[myNumber].skillDataList[skillNumber].BuffType,
                            value,
                            skillNumber,
                            false);
                        break;
                    case SkillType.enHeal:
                    case SkillType.enResurrection:
                        // 効果範囲が全体のとき
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
