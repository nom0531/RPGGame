using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

/// <summary>
/// 演出のステート
/// </summary>
public enum AllOutAttackState
{
    enAllOutAttackWaiting,  // 演出の開始待ち
    enAllOutAttackStart,    // 演出開始
    enAllOutAttackEnd,      // 演出終了
}

public class AllOutAttackSystem : MonoBehaviour
{
    [SerializeField, Header("総攻撃データ"),Tooltip("Timeline")]
    private PlayableDirector PlayableDirector;
    [SerializeField, Tooltip("一枚絵を表示するCanvas")]
    private Canvas Canvas;
    [SerializeField, Tooltip("ボタン")]
    private GameObject Button;

    private BattleManager m_battleManager;
    private TurnManager m_turnManager;
    private AllOutAttackState m_allOutAttackState = AllOutAttackState.enAllOutAttackWaiting;
    private bool m_canStart = false;        // trueなら開始できる
    private bool m_isAllEnemyDie = false;   // 全てのエネミーが倒されたかどうか

    public AllOutAttackState AllOutAttackState
    {
        get => m_allOutAttackState;
        set => m_allOutAttackState = value;
    }

    public bool AllEnemyDieFlag
    {
        get => m_isAllEnemyDie;
        set => m_isAllEnemyDie = value;
    }

    public bool CanStartFlag
    {
        get => m_canStart;
        set => m_canStart = value;
    }

    private void Start()
    {
        m_turnManager = GetComponent<TurnManager>();
        m_battleManager = GetComponent<BattleManager>();
    }

    private void Update()
    {
        CanAllOutAttack();
    }

    /// <summary>
    /// 総攻撃ができるかどうかの判定を行うタスク
    /// </summary>
    private void CanAllOutAttack()
    {
        // 総攻撃ができるなら
        if(m_canStart == true)
        {
            Button.GetComponent<Button>().interactable = true;
        }
        else
        {
            Button.GetComponent<Button>().interactable = false;
        }
    }

    /// <summary>
    /// エネミーの死亡判定
    /// </summary>
    public void IsAllEnemyDie()
    {
        // 生存判定
        for (int i = 0; i < m_battleManager.EnemyMoveList.Count; i++)
        {
            // 相手が1体でも生存しているならreturn
            if (m_battleManager.EnemyMoveList[i].ActorHPState != ActorHPState.enDie)
            {
                Debug.Log("生存してるエネミーがいる");
                return;
            }
            m_battleManager.EnemyMoveList[i].gameObject.SetActive(false);
        }
        Debug.Log("生存してるエネミーはいないよ");
        AllEnemyDieFlag = true;
    }

    /// <summary>
    /// 総攻撃を開始する処理
    /// </summary>
    public void StartAllOutAttack()
    {
        m_turnManager.AllOutAttackFlag = true;      // 総攻撃イベントを開始したことを教える
        AllOutAttackState = AllOutAttackState.enAllOutAttackStart;
        PlayableDirector.Play();
    }

    /// <summary>
    /// 総攻撃を終了する処理
    /// </summary>
    public void EndAllOutAttack()
    {
        CanStartFlag = false;
        Button.GetComponent<AllOutAttackGauge>().ResetPoint();

        // もしエネミーが全て死亡していないなら
        if (AllEnemyDieFlag == false)
        {
            Canvas.GetComponent<Animator>().SetTrigger("NotActive");
            Canvas.gameObject.transform.GetChild(1).GetComponent<Animator>().SetTrigger("NotActive");
        }
        PlayableDirector.Stop();

        AllOutAttackState = AllOutAttackState.enAllOutAttackEnd;
    }
}