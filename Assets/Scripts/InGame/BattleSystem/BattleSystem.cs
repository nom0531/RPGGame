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
    private bool m_isOnemore = false;   // 再度行動できるかどうか

    public bool OneMore
    {
        get => m_isOnemore;
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
        int rand = GetRandomValue(0, 9);    // 補正値

        float damage =
            (attackATK * 0.5f) - (attackedDEF * 0.25f) + rand;

        damage = Mathf.Max(0.0f, damage);
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
        int rand = GetRandomValue(0, 5);    // 補正値

        float damage =
            (attackATK + skillPOW * 0.01f) - attackedDEF + rand;

        damage = Mathf.Max(0.0f, damage);
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

        recoveryQuantity = Mathf.Max(0.0f, recoveryQuantity);
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

        statusFloatingValue = Mathf.Max(0.0f, statusFloatingValue);
        return (int)statusFloatingValue;
    }

    /// <summary>
    /// スキル攻撃時の属性耐性を考慮したダメージを計算する
    /// </summary>
    /// <param name="playerData">プレイヤーデータ</param>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="skillElement">スキルの属性</param>
    /// <param name="damage">属性を考慮しないダメージ</param>
    /// <returns>ダメージ量。小数点以下は切り捨て</returns>
    public int PlayerElementResistance(PlayerData playerData, int skillNumber, int skillElement, int damage)
    {
        float finalDamage = damage;
        switch(playerData.PlayerElement[skillElement])
        {
            case global::ElementResistance.enNormal:
                break;
            case global::ElementResistance.enWeak:
                Debug.Log("ONE MORE!");
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

        finalDamage = Mathf.Max(0.0f, finalDamage);
        return (int)finalDamage;
    }

    /// <summary>
    /// スキル攻撃時の属性耐性を考慮したダメージを計算する
    /// </summary>
    /// <param name="enemyData">エネミーデータ</param>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="skillElement">スキルの属性</param>
    /// <param name="damage">属性を考慮しないダメージ</param>
    /// <returns>ダメージ量。小数点以下は切り捨て</returns>
    public int EnemyElementResistance(EnemyData enemyData, int skillNumber, int skillElement, int damage)
    {
        float finalDamage = damage;
        switch (enemyData.EnemyElement[skillElement])
        {
            case global::ElementResistance.enNormal:
                break;
            case global::ElementResistance.enWeak:
                Debug.Log("ONE MORE!");
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
        int rand = GetRandomValue(0, 100);
        bool flag = false;

        // 乱数がLUCKのパラメータ以下なら
        if (rand <= LUCK)
        {
            flag = true;
        }

        return flag;
    }
}
