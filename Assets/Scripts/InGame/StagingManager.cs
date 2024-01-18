using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// 演出のステート
/// </summary>
public enum StangingState
{
    enStangingWaiting,  // 演出の開始待ち
    enStangingStart,    // 演出開始
    enStangingEnd,      // 演出終了
}

public class StagingManager : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト")]
    private GameObject CommandWindow;
    [SerializeField, Header("演出終了時に待機する時間(秒)")]
    private float WaitTime = 1.5f;

    private StangingState m_stangingState = StangingState.enStangingWaiting;
    private StangingSystem m_stangingSystem;    // 演出のシステム
    private LockOnSystem m_lockOnSystem;        // ロックオンシステム
    private BattleManager m_battleManager;      // バトルマネージャー
    private BattleSystem m_battleSystem;        // バトルシステム
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
    /// リストから削除するかどうか
    /// </summary>
    /// <param name="number">エネミーの番号</param>
    private void RemoveSelectEnemy(int number)
    {
        // ターゲットがひん死でないなら実行しない
        if (m_enemyMoveList[number].ActorHPState != ActorHPState.enDie)
        {
            return;
        }
        // リストから削除
        m_enemyMoveList.Remove(m_enemyMoveList[number]);
    }

    /// <summary>
    /// 演出を開始する
    /// </summary>
    /// <param name="effectRange">行動の効果範囲</param>
    /// <param name="actionType">行動パターン</param>
    /// <param name="turnStatus">ターンを回す側</param>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <param name="myNumber">自身の番号</param>
    /// <param name="skillNumber">スキルの番号</param>
    public void RegistrationTargets(ActionType actionType, TurnStatus turnStatus, int targetNumber, int myNumber, int skillNumber=0, EffectRange effectRange=EffectRange.enOne)
    {
        var number = targetNumber;
        // ターゲットの番号を使用しない行動が選択されている場合
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
            // 使用する番号を自身の番号に切り替える
            number = myNumber;
        }
        // ターゲットがエネミーのとき
        if(m_lockOnSystem.TargetState == TargetState.enEnemy)
        {
            RemoveSelectEnemy(number);
        }
        // オブジェクトをカメラのターゲットとして設定する
        switch (effectRange)
        {
            // 単体攻撃
            case EffectRange.enOne:
                if (m_lockOnSystem.TargetState == TargetState.enPlayer)
                {
                    m_stangingSystem.SetCameraTarget(m_playerMoveList[number].gameObject);
                    break;
                }
                m_stangingSystem.SetCameraTarget(m_enemyMoveList[number].gameObject);
                break;
            // 全体攻撃
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
        // 演出を開始
        StangingStart(actionType, skillNumber, effectRange);
    }

    /// <summary>s
    /// 演出を開始する
    /// </summary>
    private void StangingStart(ActionType actionType, int skillNumber, EffectRange effectRange)
    {
        m_stangingState = StangingState.enStangingStart;
        CommandWindow.SetActive(false);
        DrawTargets();
        // カメラを移動する
        m_stangingSystem.ChangeVcam((int)effectRange);
        // エフェクトを再生する
        m_stangingSystem.PlayEffect(actionType, skillNumber);
        StangingEnd(actionType);
    }

    /// <summary>
    /// オブジェクトを描画する
    /// </summary>
    private void DrawTargets()
    {
        for(int i= 0; i < m_playerMoveList.Count; i++)
        {
            m_playerMoveList[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 演出を終了する
    /// </summary>
    async private void StangingEnd(ActionType actionType)
    {
        if (actionType != ActionType.enGuard)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(WaitTime));
        }
        // 設定をリセットする
        m_stangingSystem.ResetPriority();
        m_stangingState = StangingState.enStangingEnd;
        CommandWindow.SetActive(true);
    }
}
