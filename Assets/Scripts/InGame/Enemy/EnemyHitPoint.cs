using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHitPoint : MonoBehaviour
{
    /// <summary>
    /// HPの減少処理
    /// </summary>
    private enum DecreaseProcess
    {
        enStart,    // 開始
        enEnd,      // 終了
    }


    /// <summary>
    /// 配列の添え字
    /// </summary>
    private enum ImageState
    {
        enGreen,
        enRed,
        enNum
    }

    [SerializeField, Header("参照データ")]
    private EnemyDataBase EnemyData;
    [SerializeField, Header("HPゲージ")]
    private GameObject Data_HPBarGreen;
    [SerializeField]
    private GameObject Data_HPBarRed;

    private BattleManager m_battleManager;
    private DrawStatusValue m_drawStatusValue;
    private LockOnManager m_lockOnManager;
    private Image[] m_barImages = new Image[(int)ImageState.enNum];
    private DecreaseProcess m_decreaseProcess = DecreaseProcess.enEnd;  // バーの減少処理のステート
    private bool m_isInit = false;                                      // 初期化を実行したならtrue

    // Start is called before the first frame update
    void Start()
    {
        SetFillAmount();
    }

    private void Update()
    {
        Decrease();
    }

    /// <summary>
    /// 割合を設定する
    /// </summary>
    public void SetFillAmount()
    {
        Init();
        var number = m_battleManager.EnemyMoveList[m_lockOnManager.TargetNumber].MyNumber;
        for (int i = 0; i < (int)ImageState.enNum; i++)
        {
            // 割合を設定
            m_barImages[i].fillAmount = m_drawStatusValue.CalculateRate(
                m_battleManager.EnemyMoveList[m_lockOnManager.TargetNumber].EnemyStatus.HP, EnemyData.enemyDataList[number].HP);
        }
        m_decreaseProcess = DecreaseProcess.enStart;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Init()
    {
        if(m_isInit == true)
        {
            return;
        }
        m_drawStatusValue = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<DrawStatusValue>();
        m_barImages[(int)ImageState.enGreen] = Data_HPBarGreen.GetComponent<Image>();
        m_barImages[(int)ImageState.enRed] = Data_HPBarRed.GetComponent<Image>();
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        m_lockOnManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<LockOnManager>();
        m_isInit = false;
    }

    /// <summary>
    /// HPの減少処理
    /// </summary>
    private void Decrease()
    {
        // 減少処理を開始しないなら実行しない
        if (m_decreaseProcess == DecreaseProcess.enEnd)
        {
            return;
        }
        // 計算
        var number = m_battleManager.EnemyMoveList[m_lockOnManager.TargetNumber].MyNumber;
        // 割合を設定
        m_barImages[(int)ImageState.enGreen].fillAmount = m_drawStatusValue.CalculateRate(
            m_battleManager.EnemyMoveList[m_lockOnManager.TargetNumber].EnemyStatus.HP, EnemyData.enemyDataList[number].HP);
        m_barImages[(int)ImageState.enRed].fillAmount = m_drawStatusValue.SetRate(
            m_barImages[(int)ImageState.enRed].fillAmount, m_barImages[(int)ImageState.enGreen].fillAmount);
        m_decreaseProcess = DecreaseProcess.enEnd;
    }
}
