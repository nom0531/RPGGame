using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スキルの種類
/// </summary>
public enum SkillType
{
    enAttack,       // 攻撃
    enBuff,         // バフ
    enDeBuff,       // デバフ
    enHeal,         // 回復
    enResurrection  // 復活
}

/// <summary>
/// スキルの効果範囲
/// </summary>
public enum EffectRange
{
    enOne,      // 単体
    enAll,      // 全体
}

/// <summary>
/// バフの種類
/// </summary>
public enum BuffType
{
    enATK,
    enDEF,
    enSPD,
    enNull,
}

/// <summary>
/// 消費する種類
/// </summary>
public enum NecessaryType
{
    enSP,       // SPを消費する
    enHP,       // HPを消費する
}

/// <summary>
/// スキルの構造体
/// </summary>
[System.Serializable]
public class SkillData
{
    [SerializeField, Header("スキル情報")]
    public string SkillName;                                        // 名前
    public int ID;                                                  // 自身の番号
    public Sprite SkillSprite;                                      // 画像
    public SkillType SkillType;                                     // スキルの種類
    public GameObject SkillEffect;                                  // スキルのエフェクト
    public float EffectScale;                                       // エフェクトのスケール
    [SerializeField, Header("属性")]
    public ElementType SkillElement;                                // スキルの属性
    public StateAbnormalData StateAbnormalData;                     // 追加属性
    [SerializeField, Header("ステータス")]
    public int POW;                                                 // 攻撃、回復の基礎値
    public int SkillNecessary;                                      // 必要SP/HP量
    public NecessaryType Type;                                      // 消費SP/HP
    [SerializeField, Header("スキル詳細")]
    public EffectRange EffectRange;                                 // スキルの効果範囲
    public TargetState TargetState;                                 // スキルの対象
    public BuffType BuffType;                                       // バフのタイプ
    [SerializeField,Header("必要強化ポイント")]
    public int EnhancementPoint;                                    // 必要強化ポイント
    [SerializeField, Header("詳細設定"), Multiline(2)]
    public string SkillDetail;                                      // スキルの詳細設定
}

[CreateAssetMenu(fileName = "SkillDataBase", menuName = "CreateSkillDataBace")]
public class SkillDataBase : ScriptableObject
{
    public List<SkillData> skillDataList = new List<SkillData>();
}
