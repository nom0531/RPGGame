using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// 演出のステート
/// </summary>
public enum StagingState
{
    enStangingWaiting,  // 演出の開始待ち
    enStangingStart,    // 演出開始
    enStangingEnd,      // 演出終了
}

public class StagingManager : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト")]
    private GameObject CommandWindow;
    [SerializeField]
    private GameObject CutInObject;
    [SerializeField, Header("演出終了時に待機する時間(秒)")]
    private float WaitTime = 1.5f;

    private StagingSystem m_stangingSystem;     // 演出のシステム
    private LockOnManager m_lockOnSystem;       // ロックオンシステム
    private BattleManager m_battleManager;      // バトルマネージャー
    private BattleSystem m_battleSystem;        // バトルシステム
    private TurnManager m_turnManager;
    private CutInManager m_cutInManager;        // カットイン演出のシステム
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
    /// 選択しているエネミーをリストから削除するかどうか判定する
    /// </summary>
    /// <param name="number">エネミーの番号</param>
    private void ShouldRemoveEnemyList(int number)
    {
        if(m_lockOnSystem.TargetState != TargetState.enEnemy)
        {
            return;
        }
        // ターゲットがひん死でないなら実行しない
        if (m_enemyMoveList[number].ActorHPState != ActorHPState.enDie)
        {
            return;
        }
        m_enemyMoveList.Remove(m_enemyMoveList[number]);        // リストから削除
    }

    /// <summary>
    /// 選択しているプレイヤーをリストから削除するかどうか判定する
    /// </summary>
    /// <param name="number">プレイヤーの番号</param>
    private void ShouldRemovePlayerList(int number)
    {
        if (m_lockOnSystem.TargetState != TargetState.enPlayer)
        {
            return;
        }
        // ターゲットがひん死でないなら実行しない
        if (m_playerMoveList[number].ActorHPState != ActorHPState.enDie)
        {
            return;
        }
        m_playerMoveList.Remove(m_playerMoveList[number]);      // リストから削除
    }

    /// <summary>
    /// ターゲットを設定
    /// </summary>
    /// <param name="effectRange">行動の効果範囲</param>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <param name="myNumber">自身の番号</param>
    /// <param name="skillNumber">スキルの番号</param>
    public void RegistrationTargets(int targetNumber, int myNumber, int damage, int skillNumber=0, EffectRange effectRange=EffectRange.enOne)
    {
        m_targetNumber = targetNumber;
        m_stangingSystem.Damage = damage;
        // ターゲットの番号を使用しない行動が選択されている場合
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
            // 使用する番号を自身の番号に切り替える
            m_targetNumber = myNumber;
        }
        // オブジェクトをカメラのターゲットとして設定する
        switch (effectRange)
        {
            // 単体攻撃
            case EffectRange.enOne:
                if (m_lockOnSystem.TargetState == TargetState.enPlayer)
                {
                    m_stangingSystem.SetCameraTarget(m_playerMoveList[m_targetNumber].transform.GetChild(0).gameObject);
                    break;
                }
                m_stangingSystem.SetCameraTarget(m_enemyMoveList[m_targetNumber].transform.GetChild(0).gameObject);
                break;
            // 全体攻撃
            case EffectRange.enAll:
                if (m_skillData.skillDataList[skillNumber].TargetState == TargetState.enPlayer)
                {
                    // ターゲットを設定して追加する
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
    /// 演出を開始
    /// </summary>
    private void StangingStart(int skillNumber, EffectRange effectRange, int number)
    {
        m_stangingState = StagingState.enStangingStart;
        m_battleManager.StagingStartFlag = true;

        NormalNormalStaging(skillNumber, effectRange, number);
    }

    /// <summary>
    /// 通常演出
    /// </summary>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="effectRange">スキルの効果範囲</param>
    /// <param name="number">ターゲットの番号</param>
    async private void NormalNormalStaging(int skillNumber, EffectRange effectRange, int number)
    {
        DrawPlayers(true);
        m_stangingSystem.ChangeVcam((int)effectRange);
        // 行動が防御時の処理
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
        // 演出を開始する
        if (ActionType != ActionType.enGuard)
        {
            m_uIAnimation.ButtonDown_NotActive();
        }
        m_stangingSystem.PlayStaging(ActionType, skillNumber);
        StangingEnd(number);
    }

    /// <summary>
    /// プレイヤーを描画する
    /// </summary>
    private void DrawPlayers(bool flag)
    {
        // 防御時は実行しない
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
    /// 演出を終了
    /// </summary>
    /// <param name="number">ターゲットの番号</param>
    async private void StangingEnd(int number)
    {
        if (ActionType != ActionType.enGuard)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(WaitTime));
        }
        ShouldRemoveEnemyList(number);
        ShouldRemovePlayerList(number);
        // 設定をリセットする
        m_stangingSystem.ResetPriority();
        m_uIAnimation.ButtonDown_Active();
        m_battleManager.StagingStartFlag = false;
        m_battleSystem.WeakFlag = false;
        m_battleSystem.HitFlag = false;
        DrawPlayers(false);
        m_stangingState = StagingState.enStangingEnd;
    }

    /// <summary>
    /// カットインの演出を開始する
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
    /// Weakフラグを設定する
    /// </summary>
    private void SetWeakFlag()
    {
        m_enemyMoveList[m_targetNumber].WeakFlag = true;
#if UNITY_EDITOR
        Debug.Log("weak!");
#endif
    }
}
