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
        int rand = m_battleSystem.GetRandomValue(0, 100);
        // 一定以下ならtrueを返す
        if(rand > SPENT_PROBABILITY)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 毒状態の処理
    /// </summary>
    /// <param name="nowAbnormalState">現在の状態異常</param>
    /// <param name="AttuckerHP">攻撃を受ける側のHP</param>
    /// <returns>ダメージ量。小数点以下は切り捨て</returns>
    public int Poison(ActorAbnormalState nowAbnormalState, int AttuckerHP)
    {
        float damage = 0.0f;

        // 毒状態でないならダメージは無し
        if (nowAbnormalState != ActorAbnormalState.enPoison)
        {
            return (int)damage;
        }

        damage = AttuckerHP * (StateAbnormalData.stateAbnormalList[(int)StateNumber.enPoison].POW * 0.01f);
        return (int)damage;
    }

    /// <summary>
    /// 麻痺状態の処理
    /// <param name="nowAbnormalState">現在の状態異常</param>
    /// </summary>
    /// <returns>trueなら行動不可。falseなら行動可能</returns>
    public bool Paralysis(ActorAbnormalState nowAbnormalState)
    {
        // 麻痺状態でないなら行動可能
        if(nowAbnormalState != ActorAbnormalState.enParalysis)
        {
            return false;
        }

        int rand = m_battleSystem.GetRandomValue(0, 100);
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
    /// <param name="nowAbnormalState">現在の状態異常</param>
    /// <returns>trueならターゲットを変更。falseなら変更しない</returns>
    public bool Confusion(ActorAbnormalState nowAbnormalState)
    {
        // 麻痺状態でないなら行動可能
        if (nowAbnormalState != ActorAbnormalState.enConfusion)
        {
            return false;
        }

        int rand = m_battleSystem.GetRandomValue(0, 100);
        // 一定以下ならターゲットを変更
        if (rand <= StateAbnormalData.stateAbnormalList[(int)StateNumber.enConfusion].POW)
        {
            return true;
        }
        return false;
    }
}
