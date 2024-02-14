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

/// <summary>
/// テキスト表示用のデータ
/// </summary>
public struct TextData
{
    public int value;
    public bool isHit;
    public GameObject gameObject;
}

public class StagingManager : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト")]
    private GameObject CommandWindow;
    [SerializeField, Header("演出終了時に待機する時間(秒)")]
    private float WaitTime = 1.5f;

    private StagingSystem m_stangingSystem;    // 演出のシステム
    private LockOnSystem m_lockOnSystem;        // ロックオンシステム
    private BattleManager m_battleManager;      // バトルマネージャー
    private List<EnemyMove> m_enemyMoveList;
    private List<PlayerMove> m_playerMoveList;
    private List<TextData> m_testDataList;
    private SkillDataBase m_skillData;
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
    /// テキストデータを追加する
    /// </summary>
    /// <param name="value">値</param>
    /// <param name="isHit">攻撃がヒットしたかどうか</param>
    public void AddTextData(int value, bool isHit, GameObject gameObject)
    {
        var textData = new TextData() { value = value, isHit = isHit, gameObject = gameObject};
        m_testDataList.Add(textData);
    }

    private void Awake()
    {
        m_lockOnSystem = GetComponent<LockOnSystem>();
        m_stangingSystem = GetComponent<StagingSystem>();
        m_battleManager = GetComponent<BattleManager>();
        m_skillData = m_battleManager.SkillDataBase;
        m_stangingSystem.SkillDataBase = m_skillData;
    }

    private void Start()
    {
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
        m_enemyMoveList[number].gameObject.SetActive(false);    // 非表示
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
        m_playerMoveList[number].gameObject.SetActive(false);   // 非表示
        m_playerMoveList.Remove(m_playerMoveList[number]);      // リストから削除
    }

    /// <summary>
    /// 演出を開始する
    /// </summary>
    /// <param name="effectRange">行動の効果範囲</param>
    /// <param name="turnStatus">ターンを回す側</param>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <param name="myNumber">自身の番号</param>
    /// <param name="skillNumber">スキルの番号</param>
    public void RegistrationTargets(TurnStatus turnStatus, int targetNumber, int myNumber, int skillNumber=0, EffectRange effectRange=EffectRange.enOne)
    {
        var number = targetNumber;
        // ターゲットの番号を使用しない行動が選択されている場合
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
            // 使用する番号を自身の番号に切り替える
            number = myNumber;
        }
        // オブジェクトをカメラのターゲットとして設定する
        switch (effectRange)
        {
            // 単体攻撃
            case EffectRange.enOne:
                if (m_lockOnSystem.TargetState == TargetState.enPlayer)
                {
                    m_stangingSystem.SetCameraTarget(m_playerMoveList[number].transform.GetChild(0).gameObject);
                    break;
                }
                m_stangingSystem.SetCameraTarget(m_enemyMoveList[number].transform.GetChild(0).gameObject);
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
        StangingStart(skillNumber, effectRange, number);
    }

    /// <summary>s
    /// 演出を開始
    /// </summary>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="effectRange">スキルの効果範囲</param>
    /// <param name="number">ターゲットの番号</param>
    private void StangingStart(int skillNumber, EffectRange effectRange, int number)
    {
        m_battleManager.StagingStartFlag = true;
        m_stangingState = StagingState.enStangingStart;
        // 演出を開始する
        CommandWindow.SetActive(false);
        DrawPlayers();
        m_stangingSystem.ChangeVcam((int)effectRange);
        m_stangingSystem.PlayEffect(ActionType, skillNumber);
        StangingEnd(number);
    }

    /// <summary>
    /// プレイヤーを描画する
    /// </summary>
    private void DrawPlayers()
    {
        for(int i= 0; i < m_playerMoveList.Count; i++)
        {
            m_playerMoveList[i].gameObject.SetActive(true);
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
        m_stangingState = StagingState.enStangingEnd;
        CommandWindow.SetActive(true);
        m_battleManager.StagingStartFlag = false;
    }
}
