using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAbnormalCalculation : MonoBehaviour
{
   private enum StateNumber
    {
        enPoison = 7,
        enParalysis,
        enSilence,
        enConfusion,
    }

    [SerializeField, Header("参照データ")]
    private StateAbnormalDataBase StateAbnormalData;
    private BattleSystem m_battleSystem;

    private const int SPENT_PROBABILITY = 80;   // 状態異常にかかる確率
    private const int RECOVER_PROBABILITY = 60; // 状態異常から復活する確率
    private const int ADDVALUE = 10;            // 加算値

    private int m_recoverProbability = 0;       // 復活する確率
    private int m_count = 0;                    // 状態異常から復活しなかった回数

    private void Start()
    {
        m_battleSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleSystem>();
    }

    /// <summary>
    /// 状態異常になるかどうかの判定
    /// </summary>
    /// <returns>trueならかかった。falseならかからなかった</returns>
    public bool SpentToStateAbnormal()
    {
        var rand = m_battleSystem.GetRandomValue(0, 100);
        // 一定以下ならtrueを返す
        if(rand > SPENT_PROBABILITY)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 状態異常から復活するかどうかの判定
    /// </summary>
    /// <param name="actorAbnormalState">現在の状態異常</param>
    /// <returns>trueなら復活。falseなら復活しない</returns>
    public bool RecoverToAbnormal(ActorAbnormalState actorAbnormalState)
    {
        // 一定ターンが経過しているなら強制解除
        if(StateAbnormalData.stateAbnormalList[(int)actorAbnormalState].EffectTime <= m_count)
        {
            return true;
        }
        var rand = m_battleSystem.GetRandomValue(0, 100);
        // 一定以下ならtrueを返す
        if (rand > m_recoverProbability)
        {
            m_recoverProbability = RECOVER_PROBABILITY;
            return true;
        }
        m_count++;
        m_recoverProbability += ADDVALUE;   // 確率を上昇させる
        return false;
    }

    /// <summary>
    /// 毒状態の処理
    /// </summary>
    /// <param name="AttuckerHP">攻撃を受ける側のHP</param>
    /// <returns>ダメージ量。小数点以下は切り捨て</returns>
    public int Poison(int AttuckerHP)
    {
        var damage = AttuckerHP * (StateAbnormalData.stateAbnormalList[(int)StateNumber.enPoison].POW * 0.01f);
        return (int)damage;
    }

    /// <summary>
    /// 麻痺状態の処理
    /// </summary>
    /// <returns>trueなら行動不可。falseなら行動可能</returns>
    public bool Paralysis()
    {
        var rand = m_battleSystem.GetRandomValue(0, 100);
        // 一定以下なら行動不可
        if (rand <= StateAbnormalData.stateAbnormalList[(int)StateNumber.enParalysis].POW)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 混乱状態の処理
    /// </summary>
    /// <returns>trueならターゲットを変更。falseなら変更しない</returns>
    public bool Confusion()
    {
        var rand = m_battleSystem.GetRandomValue(0, 100);
        // 一定以下ならターゲットを変更
        if (rand <= StateAbnormalData.stateAbnormalList[(int)StateNumber.enConfusion].POW)
        {
            return true;
        }
        return false;
    }
}
