using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 行動パターン
/// </summary>
public enum ActionType
{ 
    enAttack,               // 通常攻撃
    enSkillAttack,          // スキル攻撃
    enGuard,                // 防御
    enEscape,               // 逃走
    enNull,                 // 何もしない
}

/// <summary>
/// HPの状態
/// </summary>
public enum ActorHPState
{
    enMaxHP,                // HPが最大
    enFewHP,                // HPが少ない
    enDie,                  // ひん死
    enNull,                 // 指定なし
}

/// <summary>
/// 状態異常
/// </summary>
public enum ActorAbnormalState
{
    enNormal,               // 通常状態
    enPoison,               // 毒
    enParalysis,            // 麻痺
    enSilence,              // 沈黙
    enConfusion,            // 混乱
    enNull,                 // 指定なし
}

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

public class BattleSystem : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private EnemyDataBase EnemyData;

    private bool m_isOnemore = false;   // 再度行動できるかどうか
    private bool m_isHit = false;       // 攻撃が当たるかどうか

    private const int NORMAL_ATTACK_PROBABILITY = 95;
    private const int SKILL_ATTACK_PROBABILITY = 95;
    private const int SKILL_HEAL_PROBABILITY = 100;
    private const int SKILL_BUFF_PROBABILITY = 95;

    public bool OneMore
    {
        get => m_isOnemore;
    }

    public bool Hit
    {
        get => m_isHit;
    }

    /// <summary>
    /// 乱数を生成する関数
    /// </summary>
    /// <param name="min">最小値</param>
    /// <param name="max">最大値</param>
    /// <param name="isShouldAdd">intの仕様を考慮した計算を行うかどうか</param>
    /// <returns>最小値から最大値の間で値を返す</returns>
    public int GetRandomValue(int min, int max, bool isShouldAdd=true)
    {
        if (isShouldAdd)
        {
            return UnityEngine.Random.Range(min, max + 1);
        }

        return UnityEngine.Random.Range(min, max);
    }

    /// <summary>
    /// 通常攻撃の処理
    /// </summary>
    /// <param name="attackATK">攻撃側の攻撃力</param>
    /// <param name="attackedDEF">防御側の防御力</param>
    /// <returns>ダメージ量。小数点以下は切り捨て</returns>
    public int NormalAttack(int attackATK, int attackedDEF)
    {
        var rand = GetRandomValue(0, 9);    // 補正値

        float damage =
            (attackATK * 0.5f) - (attackedDEF * 0.25f) + rand;
        // 補正
        damage = Mathf.Max(0.0f, damage);
        damage = AttackHit(damage, NORMAL_ATTACK_PROBABILITY);
        return (int)damage;
    }

    /// <summary>
    /// スキルでの攻撃の処理
    /// </summary>
    /// <param name="attackATK">攻撃側の攻撃力</param>
    /// <param name="skillPOW">スキルの基本値</param>
    /// <param name="attackedDEF">防御側の防御力</param>
    /// <returns>ダメージ量。小数点以下は切り捨て</returns>
    public int SkillAttack(int attackATK, int skillPOW, int attackedDEF)
    {
        var rand = GetRandomValue(0, 5);    // 補正値

        float damage =
            (attackATK + skillPOW * 0.01f) - attackedDEF + rand;
        // 補正
        damage = Mathf.Max(0.0f, damage);
        damage = AttackHit(damage, SKILL_ATTACK_PROBABILITY);
        return (int)damage;
    }

    /// <summary>
    /// スキルでの回復の処理
    /// </summary>
    /// <param name="attackedMaxHP">使用される側の最大HP</param>
    /// <param name="skillPOW">スキルの基本値</param>
    /// <returns>回復量。小数点以下は切り捨て</returns>
    public int SkillHeal(int attackedMaxHP, int skillPOW)
    {
        float recoveryQuantity = attackedMaxHP * (skillPOW * 0.01f);
        // 補正
        recoveryQuantity = Mathf.Max(0.0f, recoveryQuantity);
        recoveryQuantity = AttackHit(recoveryQuantity, SKILL_HEAL_PROBABILITY);
        return (int)recoveryQuantity;
    }

    /// <summary>
    /// スキルでのバフ・デバフの処理
    /// </summary>
    /// <param name="attackedParam">使用される側のパラメータ</param>
    /// <param name="skillPOW">スキルの基本値</param>
    /// <returns>変動後の値。小数点以下は切り捨て</returns>
    public int SkillBuff(int attackedParam,int skillPOW)
    {
        float statusFloatingValue = attackedParam * (skillPOW * 0.01f);
        // 補正
        statusFloatingValue = Mathf.Max(0.0f, statusFloatingValue);
        statusFloatingValue = AttackHit(statusFloatingValue, SKILL_BUFF_PROBABILITY);
        return (int)statusFloatingValue;
    }

    /// <summary>
    /// スキル攻撃時の属性耐性を考慮したダメージを計算する
    /// </summary>
    /// <param name="playerNumber">プレイヤーの番号</param>
    /// <param name="skillElement">スキルの属性</param>
    /// <param name="damage">属性を考慮しないダメージ</param>
    /// <returns>ダメージ量。小数点以下は切り捨て</returns>
    public int PlayerElementResistance(int playerNumber, int skillElement, int damage)
    {
        float finalDamage = damage;
        switch(EnemyData.enemyDataList[playerNumber].EnemyElement[skillElement])
        {
            case global::ElementResistance.enNormal:
                break;
            case global::ElementResistance.enWeak:
                finalDamage *= 2.0f;

                // 再度行動できるかどうかのフラグ
                if (m_isOnemore == true)
                {
                    // 既にtrueになっているならフラグを戻す
                    m_isOnemore = false;
                }
                else
                {
                    // そうでないなら再度行動できるようにする
                    m_isOnemore = true;
                }
                break;
            case global::ElementResistance.enResist:
                finalDamage *= 0.5f;
                break;
        }
        // 補正
        finalDamage = Mathf.Max(0.0f, finalDamage);
        return (int)finalDamage;
    }

    /// <summary>
    /// スキル攻撃時の属性耐性を考慮したダメージを計算する
    /// </summary>
    /// <param name="enemyNumber">エネミーの番号</param>
    /// <param name="skillElement">スキルの属性</param>
    /// <param name="damage">属性を考慮しないダメージ</param>
    /// <returns>ダメージ量。小数点以下は切り捨て</returns>
    public int EnemyElementResistance(int enemyNumber, int skillElement, int damage)
    {
        float finalDamage = damage;
        switch (EnemyData.enemyDataList[enemyNumber].EnemyElement[skillElement])
        {
            case global::ElementResistance.enNormal:
                break;
            case global::ElementResistance.enWeak:
                finalDamage *= 2.0f;

                // 再度行動できるかどうかのフラグ
                if (m_isOnemore == true)
                {
                    // 既にtrueになっているならフラグを戻す
                    m_isOnemore = false;
                }
                else
                {
                    // そうでないなら再度行動できるようにする
                    m_isOnemore = true;
                }
                break;
            case global::ElementResistance.enResist:
                finalDamage *= 0.5f;
                break;
        }
        // 補正
        finalDamage = Mathf.Max(0.0f, finalDamage);
        return (int)finalDamage;
    }

    /// <summary>
    /// 防御処理
    /// </summary>
    /// <param name="attackDEF">使用する側のDEF</param>
    /// <returns>防御力。小数点以下は切り捨て</returns>
    public int Guard(int attackDEF)
    {
        float defensePower = attackDEF + attackDEF * 0.01f;
        return (int)defensePower;
    }

    /// <summary>
    /// 逃走処理
    /// </summary>
    /// <param name="LUCK">自身の運</param>
    /// <returns>trueなら成功。falseなら失敗</returns>
    public bool Escape(int LUCK)
    {
        var rand = GetRandomValue(0, 100);
        var flag = false;

        // 乱数がLUCKのパラメータ以下なら
        if (rand <= LUCK)
        {
            flag = true;
        }

        return flag;
    }

    /// <summary>
    /// 攻撃が当たるかどうかの判定
    /// </summary>
    /// <param name="value">ダメージ</param>
    /// <param name="probability">攻撃が当たる確率</param>
    /// <returns>最終ダメージ</returns>
    private float AttackHit(float value, int probability)
    {
        var rand = GetRandomValue(0, 100);
        // 乱数が確率以下なら
        if (rand <= probability)
        {
            m_isHit = true;
            return value;
        }
        m_isHit = false;
        return 0;
    }
}
