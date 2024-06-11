using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllOutAttackSystem : MonoBehaviour
{
    [SerializeField, Header("煙のエフェクト")]
    private GameObject SmokeEffect;
    [SerializeField, Header("ボタン")]
    private GameObject Button;
    [SerializeField, Header("表示する一枚絵"),Tooltip("表示するCanvas")]
    private Canvas Canvas;
    [SerializeField, Tooltip("一枚絵")]
    private Sprite[] Sprite;

    /// <summary>
    /// エネミー全体の状態
    /// </summary>
    private enum EnemyState
    {
        enNotDeath, // 死んでいない
        enDeath     // 死んでいる
    }

    private BattleSystem m_battleSystem;
    private StagingSystem m_stagingSystem;
    private EnemyState m_enemyState;
    private bool m_canStart = false;        // trueなら開始できる
    private bool m_isStartAllOut = false;   // trueなら総攻撃を開始する
    private int m_spriteNumber = 0;         // 表示する一枚絵の番号

    public bool CanStartFlag
    {
        get => m_canStart;
        set => m_canStart = value;
    }

    public bool StartAllOutFlag
    {
        get => m_isStartAllOut;
        set => m_isStartAllOut = value;
    }

    private void Start()
    {
        m_battleSystem = GetComponent<BattleSystem>();
        m_stagingSystem = GetComponent<StagingSystem>();
    }

    private void Update()
    {
        aaaTask();
    }

    /// <summary>
    /// 総攻撃ができるかどうかの判定を行うタスク
    /// </summary>
    private void aaaTask()
    {
        // 総攻撃ができるなら…
        if(m_canStart == true)
        {
            Button.SetActive(true);     // ボタンをアクティブにする
        }
    }

    /// <summary>
    /// 総攻撃を開始する処理
    /// </summary>
    public void StartAllOutAttack()
    {
        AllOutAttack();
    }

    /// <summary>
    /// 総攻撃の処理
    /// </summary>
    private void AllOutAttack()
    {
        //SmokeEffect.SetActive(true);
        int damage = m_battleSystem.AllOutAttack();     // ダメージ量を計算
        // エネミーの状態で処理を分岐
        switch (m_enemyState)
        {
            case EnemyState.enNotDeath:
                NotDeath();
                break;
            case EnemyState.enDeath:
                Death(m_spriteNumber);
                break;
        }
        EndAllOutAttack();
    }

    /// <summary>
    /// 死んでいない時の処理
    /// </summary>
    private void NotDeath()
    {
        Debug.Log("総攻撃終了!エネミーは全滅しなかった");
    }

    /// <summary>
    /// 死んだときの処理
    /// </summary>
    private void Death(int number)
    {
        Debug.Log("総攻撃終了!エネミーは全滅");
        Canvas.gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Sprite[number];
    }

    /// <summary>
    /// 総攻撃を終了する処理
    /// </summary>
    private void EndAllOutAttack()
    {
        Button.SetActive(false);     // ボタンを非アクティブにする
        StartAllOutFlag = false;
    }
}
