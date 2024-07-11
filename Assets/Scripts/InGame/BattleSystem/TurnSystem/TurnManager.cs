using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// ターンを回す側
/// </summary>
public enum TurnStatus
{
    enPlayer,   // プレイヤー
    enEnemy,    // エネミー
}

public class TurnManager : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト")]
    private GameObject ResultObject;
    [SerializeField, Header("ターン開始時の先行側")]
    private TurnStatus m_turnStatus = TurnStatus.enPlayer;
    [SerializeField, Header("リザルトデータ"), Tooltip("UIを表示するまでの待機時間")]
    private float WaitTime = 1.0f;

    private BattleManager m_battleManager;
    private LockOnManager m_lockOnManager;
    private StagingManager m_stagingManager;
    private AllOutAttackSystem m_allOutAttackSystem;
    private DrawBattleResult m_drawBattleResult;                // リザルト演出
    private int m_turnSum = 1;                                  // 総合ターン数
    private bool m_isAllOutAttack = false;                      // trueなら総攻撃イベント

    public bool AllOutAttackFlag
    {
        get => m_isAllOutAttack;
        set => m_isAllOutAttack = value;
    }

    public int TurnSum
    {
        get => m_turnSum;
    }

    public TurnStatus TurnStatus
    {
        get => m_turnStatus;
        set => m_turnStatus = value;
    }

    private void Start()
    {
        m_allOutAttackSystem = GetComponent<AllOutAttackSystem>();
        m_battleManager = GetComponent<BattleManager>();
        m_lockOnManager = GetComponent<LockOnManager>();
        m_drawBattleResult = ResultObject.GetComponent<DrawBattleResult>();
        m_stagingManager = GetComponent<StagingManager>();

        GameClearTask().Forget();
        GameOverTask().Forget();
    }

    private void Update()
    {
        if (m_battleManager.GameState != GameState.enPlay)
        {
            return;
        }
        IsGameClear();
        IsGameOver();
    }

    /// <summary>
    /// ターンを終了しているか判定する
    /// </summary>
    public void IsTurnEnd()
    {
        // 全員の行動が終了していないなら実行しない
        for (int playerNumber = 0; playerNumber < m_battleManager.PlayerMoveList.Count; playerNumber++)
        {
            if (m_battleManager.PlayerMoveList[playerNumber].ActionEndFlag == false)
            {
                return;
            }
        }
        for (int enemyNumber = 0; enemyNumber < m_battleManager.EnemyMoveList.Count; enemyNumber++)
        {
            if (m_battleManager.EnemyMoveList[enemyNumber].ActorHPState == ActorHPState.enDie)
            {
                continue;
            }
            if (m_battleManager.EnemyMoveList[enemyNumber].ActionEndFlag == false)
            {
                return;
            }
        }
        TurnEnd();
    }

    /// <summary>
    /// 場のステータスをリセットして、次のターンに移行する
    /// </summary>
    async private void TurnEnd()
    {
        await UniTask.WaitUntil(() => m_stagingManager.StangingState == StagingState.enStangingEnd);
        // 次の操作キャラクターを決定、カメラを再設定する
        m_battleManager.InitValue();
        m_lockOnManager.ResetCinemachine();
        m_battleManager.ResetGameStatus();
        TurnStatus = TurnStatus.enPlayer;
        m_turnSum++;
    }

    /// <summary>
    /// ゲームクリアか取得する
    /// </summary>
    private void IsGameClear()
    {
        var sumEP = 0;  // 獲得EPの総量
        for (int i = 0; i < m_battleManager.EnemyMoveList.Count; i++)
        {
            // 相手が1体でも生存しているならゲームクリアではない
            if (m_battleManager.EnemyMoveList[i].ActorHPState != ActorHPState.enDie)
            {
                return;
            }
            // 生存していないならEPを加算
            sumEP += m_battleManager.EnemyDataBase.enemyDataList[m_battleManager.EnemyMoveList[i].MyNumber].EnhancementPoint;
        }
        m_drawBattleResult.EP = sumEP;
        m_battleManager.GameState = GameState.enBattleWin;
        // アニメーションを再生
        for (int i = 0; i < m_battleManager.PlayerMoveList.Count; i++)
        {
            m_battleManager.PlayerMoveList[i].PlayerAnimation.PlayAnimation(AnimationState.enWin);
        }
    }

    /// <summary>
    /// ゲームオーバーか取得する
    /// </summary>
    private void IsGameOver()
    {
        for (int i = 0; i < m_battleManager.PlayerMoveList.Count; i++)
        {
            // 1体でも生存しているならゲームオーバーではない
            if (m_battleManager.PlayerMoveList[i].ActorHPState != ActorHPState.enDie)
            {
                return;
            }
        }

        m_battleManager.GameState = GameState.enBattleLose;
        // アニメーションを再生
        for (int i = 0; i < m_battleManager.PlayerMoveList.Count; i++)
        {
            m_battleManager.PlayerMoveList[i].PlayerAnimation.PlayAnimation(AnimationState.enLose);
        }
    }

    /// <summary>
    /// ゲームクリア演出のタスク
    /// </summary>
    async UniTask GameClearTask()
    {
        await UniTask.WaitUntil(() => m_battleManager.GameState == GameState.enBattleWin);
        if (AllOutAttackFlag == false)
        {
            await UniTask.WaitUntil(() => m_stagingManager.StangingState == StagingState.enStangingEnd);
        }
        else
        {
            await UniTask.WaitUntil(() => m_allOutAttackSystem.AllOutAttackState == AllOutAttackState.enAllOutAttackEnd && m_allOutAttackSystem.AllEnemyDieFlag == true);
        }
        await UniTask.Delay(TimeSpan.FromSeconds(WaitTime));
        m_drawBattleResult.GameClearStaging();
    }

    /// <summary>
    /// ゲームオーバー演出のタスク
    /// </summary>
    async UniTask GameOverTask()
    {
        await UniTask.WaitUntil(() => m_battleManager.GameState == GameState.enBattleLose
        && m_stagingManager.StangingState == StagingState.enStangingEnd);
        await UniTask.Delay(TimeSpan.FromSeconds(WaitTime));
        m_drawBattleResult.GameOverStaging();
    }
}
