using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バフ・デバフ
/// </summary>
public enum BuffStatus
{
    enBuff_ATK,
    enBuff_DEF,
    enBuff_SPD,
    enDeBuff_ATK,
    enDeBuff_DEF,
    enDeBuff_SPD,
    enNum
}

public class BuffCalculation : MonoBehaviour
{
    private BattleManager m_battleManager;
    private bool[] m_buffFlag =
        new bool[(int)BuffStatus.enNum];    // バフがかかっているかどうか
    private bool[] m_effectEndFlag =
        new bool[(int)BuffStatus.enNum];    // 効果時間が終了しているかどうか
    private int[] m_buffTargetParam =
       new int[(int)BuffStatus.enNum];      // パラメータの上昇・下降値
    private int[] m_buffEffectTime =
        new int[(int)BuffStatus.enNum];     // バフの効果時間
    private int m_turnSum = 0;              // ターンの総数

    /// <summary>
    /// 上昇値・下降値を取得する
    /// </summary>
    /// <param name="buffStatus">バフのタイプ</param>
    /// <returns>変動後の値</returns>
    public int GetBuffParam(BuffStatus buffStatus)
    {
        int param = m_buffTargetParam[(int)buffStatus];
        return param;
    }

    /// <summary>
    /// 上昇値・下降値を設定する
    /// </summary>
    /// <param name="buffStatus">バフのタイプ</param>
    /// <param name="value">変動後の値</param>
    public void SetBuffParam(BuffStatus buffStatus, int value)
    {
        m_buffTargetParam[(int)buffStatus] = value;
    }

    /// <summary>
    /// バフが掛かっているかどうかのフラグを設定する
    /// </summary>
    /// <param name="buffStatus">バフのタイプ</param>
    /// <param name="flag">trueならかかっている。falseならかかっていない</param>
    private void SetBuffFlag(BuffStatus buffStatus, bool flag)
    {
        m_buffFlag[(int)buffStatus] = flag;
    }

    /// <summary>
    /// バフがかかっているかどうかのフラグを取得する
    /// </summary>
    /// <param name="buffStatus">バフのタイプ</param>
    /// <returns>tureならかかっている。falseならかかっていない</returns>
    public bool GetBuffFlag(BuffStatus buffStatus)
    {
        return m_buffFlag[(int)buffStatus];
    }

    /// <summary>
    /// 効果時間が終了しているかどうかのフラグを設定する
    /// </summary>
    /// <param name="buffStatus">バフのタイプ</param>
    /// <param name="flag">tureなら終了している。falseなら終了していない</param>
    public void SetEffectEndFlag(BuffStatus buffStatus, bool flag)
    {
        m_effectEndFlag[(int)buffStatus] = flag;
    }

    /// <summary>
    /// 効果時間が終了しているかどうかのフラグを取得する
    /// </summary>
    /// <param name="buffStatus">バフのタイプ</param>
    /// <returns>tureなら終了している。falseなら終了していない</returns>
    public bool GetEffectEndFlag(BuffStatus buffStatus)
    {
        return m_effectEndFlag[(int)buffStatus];
    }

    /// <summary>
    /// 効果時間を設定する
    /// </summary>
    /// <param name="buffStatus">バフのタイプ</param>
    /// <param name="effectTime">効果時間</param>
    private void SetEffectTime(BuffStatus buffStatus, int effectTime)
    {
        m_buffEffectTime[(int)buffStatus] += effectTime;
    }

    private void Awake()
    {
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 0; i < (int)BuffStatus.enNum; i++)
        {
            // 値を初期化する
            m_buffFlag[i] = false;
            m_buffTargetParam[i] = 0;
            m_buffEffectTime[i] = 0;
        }
    }

    private void FixedUpdate()
    {
        // プレイ中でないなら中断
        if(m_battleManager.GameState != GameState.enPlay)
        {
            return;
        }
        if(m_battleManager.PauseFlag == true)
        {
            return;
        }

        // 保持している経過ターン数が現在の経過ターン数と同じなら中断
        if(m_turnSum == m_battleManager.TurnSum)
        {
            return;
        }
        // 値を更新する
        m_turnSum = m_battleManager.TurnSum;

        for (int i = 0; i< (int)BuffStatus.enNum; i++)
        {
            if(m_buffFlag[i] == false)
            {
                // バフがかかっていないなら次
                continue;
            }

            Decrement((BuffStatus)i);
        }
    }

    /// <summary>
    /// 効果時間を減らす
    /// </summary>
    /// <param name="buffStatus">バフのタイプ</param>
    private void Decrement(BuffStatus buffStatus)
    {
        m_buffEffectTime[(int)buffStatus]--;

        // もし効果時間が0以下なら
        if (m_buffEffectTime[(int)buffStatus] <= 0)
        {
            SetEffectEndFlag(buffStatus, true);
            gameObject.GetComponent<DrawCommandText>().ReSetStatusText();
        }
    }

    /// <summary>
    /// バフがかかったときのステータスを変更する
    /// </summary>
    /// <param name="buffStatus">バフのタイプ</param>
    /// <param name="statusFloatingValue">増加値</param>
    /// <param name="originalValue">元の値</param>
    /// <param name="skillNumber">スキルの番号</param>
    /// <returns>変動後の値</returns>
    public int CalcBuff(BuffStatus buffStatus, int statusFloatingValue, int originalValue, int effectTime)
    {
        // 既にかかっているなら
        if (m_buffFlag[(int)buffStatus] == true)
        {
            SetEffectTime(buffStatus, effectTime);  // 効果時間を追加

            Debug.Log("効果時間が" + effectTime + "伸びた");

            return originalValue;
        }

        SetBuffParam(buffStatus, statusFloatingValue);
        SetBuffFlag(buffStatus, true);
        SetEffectTime(buffStatus, effectTime);

        Debug.Log(buffStatus + "が上昇");

        return originalValue + statusFloatingValue;
    }

    /// <summary>
    /// デバフがかかったときのステータスを変更する
    /// </summary>
    /// <param name="buffStatus">バフのタイプ</param>
    /// <param name="statusFloatingValue">減少値</param>
    /// <param name="originalValue">元の値</param>
    ///     /// <param name="skillNumber">スキルの番号</param>
    /// <returns>変動後の値</returns>
    public int CalcDebuff(BuffStatus buffStatus, int statusFloatingValue, int originalValue, int effectTime)
    {
        // 既にかかっているなら
        if (m_buffFlag[(int)buffStatus] == true)
        {
            SetEffectTime(buffStatus, effectTime);   // 効果時間を追加
            Debug.Log("効果時間が" + effectTime + "伸びた");

            return originalValue;
        }

        SetBuffParam(buffStatus, statusFloatingValue);
        SetBuffFlag(buffStatus, true);
        SetEffectTime(buffStatus, effectTime);

        Debug.Log(buffStatus + "が減少");

        return originalValue - statusFloatingValue;
    }

    /// <summary>
    /// ステータスを戻す
    /// </summary>
    public int ResetStatus(BuffStatus buffStatus, int originalValue, bool isBuff)
    {
        if(isBuff == true)
        {
            return originalValue - m_buffTargetParam[(int)buffStatus];
        }

        return originalValue + m_buffTargetParam[(int)buffStatus];
    }
}
